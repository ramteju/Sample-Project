using DTO;
using Entities;
using Entities.DTO;
using Entities.DTO.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using ProductTracking.Hubs;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using ProductTracking.Store;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Transactions;
using ProductTracking.Logging;

namespace ProductTracking.Services.Core
{
    public class TanService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static readonly int MAX_REPORT_DAYS = 10;

        [Dependency("UserService")]
        public UserService userService { get; set; }
        public string UserId(ClaimsIdentity claimsIdentity)
        {
            return (from c in claimsIdentity.Claims where c.Type == ClaimTypes.WindowsAccountName select c).FirstOrDefault()?.Value;
        }
        public List<RolesDTO> UserRoles(string UserName)
        {
            List<RolesDTO> RolesDto = (from ur in db.UserRoles where ur.ApplicationUser.UserName == UserName select new RolesDTO { Id = (int)ur.Role, Name = ur.Role.ToString() }).OrderBy(r => r.Id).ToList();
            return RolesDto;
        }

        public List<RegulerExpression> GetRegulerExpressions()
        {
            return db.RegulerExpressions.ToList();
        }

        public List<TaskDTO> MyTans(string currentUserId, int currentRoleId, bool pullTasks)
        {
            List<Tan> userTans = new List<Tan>();
            List<TaskDTO> userTaskDTOs = new List<TaskDTO>();
            ApplicationUser user = db.Users.Find(currentUserId);
            userTans = (from t in GetAllTans()
                        where (t.CurrentUser != null && t.CurrentUser.Id == currentUserId && t.CurrentUserRole != null &&
                              (int)t.CurrentUserRole.Role == currentRoleId && !t.MarkedAsQuery && t.TanCategory == TanCategory.Progress && !t.DeliveryBatches.Any())
                        select t).ToList();
            foreach (var tan in userTans)
                userTaskDTOs.Add(GetTaskDTO(tan, currentRoleId, user));
            return userTaskDTOs;
        }

        public object AssignTans(List<int> tanIDs, string toUserId, int currentRoleId, string userId, string comment, Role role = Role.Curator)
        {
            object result = true;
            UserRole currentUserRole = db.UserRoles.Where(ur => ur.ApplicationUser.Id == toUserId && ur.Role == role).FirstOrDefault();
            var sourceUser = db.Users.Find(userId);
            using (TransactionScope scope = new TransactionScope())
            {
                var toUser = db.Users.Find(toUserId);
                foreach (var tanid in tanIDs)
                {
                    var tan = GetAllTans().Where(t => t.Id == tanid).FirstOrDefault();
                    TanMetaDataUpdateHistory tanMetaDataUpdateHistory;
                    if (tan != null)
                    {
                        tanMetaDataUpdateHistory = new TanMetaDataUpdateHistory();
                        tanMetaDataUpdateHistory.Tan = tan;
                        tanMetaDataUpdateHistory.UpdatedDate = DateTime.Now;
                        tanMetaDataUpdateHistory.User = sourceUser;
                        tanMetaDataUpdateHistory.UserComment = comment;
                        if (tan.Curator != null && tan.CurrentUser != null)
                        {
                            tanMetaDataUpdateHistory.TanMetaDataUpdateAction = TanMetaDataUpdateAction.TANMANUALREALLOCATION;
                            tanMetaDataUpdateHistory.ActionMessage = $"Tan Reallocated from {tan.CurrentUser.UserName} to {toUser.UserName} by {sourceUser.UserName}";
                            tan.AllocatedType = TaskAllocatedType.MANUALREALLOCATION;
                            tan.TanState = role == Role.Curator ? TanState.Curation_ReAssigned : role == Role.Reviewer ? TanState.Review_ReAssigned : TanState.QC_ReAssigned;
                        }
                        else
                        {
                            tanMetaDataUpdateHistory.TanMetaDataUpdateAction = TanMetaDataUpdateAction.TANMANUALALLOCATION;
                            tanMetaDataUpdateHistory.ActionMessage = $"Tan Allocated manually to {toUser.UserName} by {sourceUser.UserName}";
                            tan.AllocatedType = TaskAllocatedType.MANUALALLOCATION;
                            tan.TanState = role == Role.Curator ? TanState.Curation_Assigned : role == Role.Reviewer ? TanState.Review_Assigned : TanState.QC_Assigned;
                        }
                        if (role == Role.Curator)
                            tan.Curator = toUser;
                        else if (role == Role.Reviewer)
                            tan.Reviewer = toUser;
                        else if (role == Role.QC)
                            tan.QC = toUser;
                        tan.CurrentUser = toUser;
                        tan.CurrentUserRole = currentUserRole;
                        AddTanAction(db, tan, TanAction.TAN_ASSIGNED, currentUserRole.ApplicationUser, currentUserRole.Role, $"Tan Assigned to {currentUserRole.ApplicationUser.UserName} by {sourceUser.UserName} manually.");
                        db.TanMetaDataUpdateHistory.Add(tanMetaDataUpdateHistory);
                    }
                }
                db.SaveChanges();
                scope.Complete();
            }
            return result;
        }

        private IQueryable<Tan> GetRoleWiseTans(int role, string currentUser = "")
        {
            if (role == (int)Role.Curator)
                return GetAllTans().Where(tan => tan.Curator != null && (!string.IsNullOrEmpty(currentUser) ? tan.Curator.Id == currentUser : true));
            else if (role == (int)Role.Reviewer)
                return GetAllTans().Where(tan => tan.Reviewer != null && (!string.IsNullOrEmpty(currentUser) ? tan.Reviewer.Id == currentUser : true));
            else
                return GetAllTans().Where(tan => tan.QC != null && (!string.IsNullOrEmpty(currentUser) ? tan.QC.Id == currentUser : true));
        }

        private IQueryable<Tan> GetAllTans()
        {
            return db.Tans.Include("Batch").Include("Curator").Include("Reviewer").Include("QC").Include("CurrentUser").Include("CurrentUserRole");
        }

        public bool GetTaskPermissions(ApplicationUser user, int currentRoleId)
        {
            if (currentRoleId == 1)
                return (user.AllowedForCuration.HasValue && user.AllowedForCuration.Value);
            else if (currentRoleId == 2)
                return (user.AllowedForReview.HasValue && user.AllowedForReview.Value);
            else if (currentRoleId == 3)
                return (user.AllowedForQC.HasValue && user.AllowedForQC.Value);
            return false;
        }

        public PullTask PullTask(string currentUserId, int currentRoleId, bool pullTasks = true)
        {
            List<Tan> userTans = new List<Tan>();
            PullTask pullTask = new PullTask();
            ApplicationUser user = db.Users.Find(currentUserId);
            var currentUserTans = GetRoleWiseTans(currentRoleId, currentUserId);
            var RoleWiseTans = GetRoleWiseTans(currentRoleId);
            var currentUserReactions = currentUserTans != null && currentUserTans.Any() ? currentUserTans.Select(u => u.RxnCount).Sum() : 0;

            var currentUserDensity = currentUserTans != null && currentUserTans.Any() ? (float)currentUserReactions / (float)currentUserTans.Count() : 0;
            UserRole currentUserRole = db.UserRoles.Where(ur => ur.ApplicationUser.Id == currentUserId && (int)ur.Role == currentRoleId).FirstOrDefault();
            #region Assign Tans If Not Avalialble
            var AutoAllocation = userService.GetSettings();
            int existingTansCount = (from t in GetAllTans()
                                     where t.CurrentUser != null && t.CurrentUser.Id == currentUserId && t.CurrentUserRole != null && (int)t.CurrentUserRole.Role == currentRoleId && !t.MarkedAsQuery && t.TanCategory == TanCategory.Progress && !t.DeliveryBatches.Any()
                                     select t).Count();
            if (AutoAllocation.Autoallocation && GetTaskPermissions(user, currentRoleId) && (currentRoleId == (int)Role.Curator || currentRoleId == (int)Role.Reviewer) && existingTansCount == 0)
            {
                int rxnRank = 1;
                Dictionary<float, int> rxnDensityRanks = new Dictionary<float, int>();
                var ta = RoleWiseTans.GroupBy(tan =>
                 currentRoleId == (int)Role.Curator ? tan.CuratorId : currentRoleId == (int)Role.Reviewer ? tan.ReviewerId : tan.QCId)
                .ToDictionary(cur => cur.Key, cur => cur.Select(tan => (float)tan.RxnCount).Sum() / (float)cur.Count());
                if (!ta.ContainsKey(currentUserId))
                {
                    if (db.UserDefaultDensities.Where(ud => (int)ud.UserRole.Role == currentRoleId && ud.UserRole.UserId == currentUserId).FirstOrDefault() != null)
                        currentUserDensity = ta[currentUserId] = db.UserDefaultDensities.Where(ud => (int)ud.UserRole.Role == currentRoleId && ud.UserRole.UserId == currentUserId).FirstOrDefault().UserRXNDensity;
                }
                var orderedRxnDensities = ta.Select(u => u.Value).Distinct().OrderByDescending(d => d);
                foreach (var rxnDensity in orderedRxnDensities)
                {
                    rxnDensityRanks[rxnDensity] = rxnRank;
                    rxnRank++;
                }
                foreach (var u in db.Users)
                {
                    if (ta.ContainsKey(u.Id))
                    {
                        pullTask.UserRanks.Add(new RankHolder
                        {
                            Key = u.UserName,
                            Rank = rxnDensityRanks[ta[u.Id]],
                            Score = ta[u.Id]
                        });
                    }
                }
                int userRank = rxnDensityRanks[currentUserDensity];
                Dictionary<int, int> numRanks = new Dictionary<int, int>();
                var availableTans = GetAllTans().Where(t => t.TanStatus != TanStatus.OnHold && (currentRoleId == (int)Role.Curator && t.Curator == null) || (currentRoleId == (int)Role.Reviewer && t.TanState != null && t.TanState == TanState.Curation_Submitted && t.Reviewer == null && t.Curator != null && t.Curator.Id != currentUserId));
                if (availableTans.Any())
                {
                    var orderedNums = availableTans.Select(t => currentRoleId == (int)Role.Curator ? t.NumsCount : t.RxnCount).Distinct().OrderBy(t => t).ToList();
                    int numRank = 1;
                    foreach (var numOrder in orderedNums)
                    {
                        numRanks[numOrder] = numRank;
                        numRank++;
                    }

                    Dictionary<int, int> tanRanks = new Dictionary<int, int>();
                    foreach (var t in availableTans)
                        tanRanks[t.Id] = currentRoleId == (int)Role.Curator ? numRanks[t.NumsCount] : numRanks[t.RxnCount];

                    foreach (var t in availableTans)
                    {
                        pullTask.TanRanks.Add(new RankHolder
                        {
                            Key = t.tanNumber,
                            Rank = currentRoleId == (int)Role.Curator ? numRanks[t.NumsCount] : numRanks[t.RxnCount],
                            Score = currentRoleId == (int)Role.Curator ? t.NumsCount : t.RxnCount
                        });
                    }
                    var maxTanRank = tanRanks.Select(d => d.Value).Max();
                    var finalRank = Math.Min(userRank, maxTanRank);
                    var tansInIncreasingRank = tanRanks.OrderBy(d => d.Value).Where(d => d.Value >= finalRank).FirstOrDefault();
                    if (!tansInIncreasingRank.Equals(default(KeyValuePair<int, int>)))
                    {
                        var tans = (from t in GetAllTans() where t.Id == tansInIncreasingRank.Key select t).Take(AutoAllocation.MaxTasks);
                        using (TransactionScope scope = new TransactionScope())
                        {
                            foreach (var tan in tans)
                            {
                                if (tan != null)
                                {
                                    switch (currentRoleId)
                                    {
                                        case 1:
                                            tan.Curator = user;
                                            break;
                                        case 2:
                                            tan.Reviewer = user;
                                            break;
                                        case 3:
                                            tan.QC = user;
                                            break;
                                    }
                                    tan.TanState = currentRoleId == (int)Role.Curator ? TanState.Curation_Assigned : TanState.Review_Assigned;
                                    tan.CurrentUserRole = currentUserRole;
                                    tan.CurrentUser = user;
                                    tan.AllocatedType = TaskAllocatedType.AUTOALLOCATION;
                                    AddTanAction(db, tan, TanAction.TAN_ASSIGNED, user, currentUserRole.Role, $"Tan Assigned to {user.UserName} by Automation Process");
                                    db.Entry(tan).State = EntityState.Modified;
                                    pullTask.TanId = tan.Id;
                                    pullTask.TanNumber = tan.tanNumber;
                                }
                            }
                            db.SaveChanges();
                            scope.Complete();
                        }
                    }
                }
                //}
            }
            else if (!AutoAllocation.Autoallocation && (currentRoleId == (int)Role.Curator || currentRoleId == (int)Role.Reviewer))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var tasksToAdd = AutoAllocation.MaxTasks - existingTansCount;
                    var tansToBeAllotted = GetAllTans().Where(t => t.TanStatus != TanStatus.OnHold && (currentRoleId == (int)Role.Curator && t.Curator == null) || (currentRoleId == (int)Role.Reviewer && t.TanState == TanState.Curation_Submitted)).Take(tasksToAdd);
                    if (tansToBeAllotted != null && tansToBeAllotted.Count() > 0)
                    {
                        foreach (var tanToBeAllot in tansToBeAllotted)
                        {
                            if (currentRoleId == 1)
                                tanToBeAllot.Curator = user;
                            else if (currentRoleId == 2)
                                tanToBeAllot.Reviewer = user;
                            else if (currentRoleId == 3)
                                tanToBeAllot.QC = user;
                            tanToBeAllot.CurrentUser = user;
                            tanToBeAllot.CurrentUserRole = currentUserRole;
                            tanToBeAllot.AllocatedType = TaskAllocatedType.AUTOALLOCATION;
                            tanToBeAllot.TanState = currentRoleId == (int)Role.Curator ? TanState.Curation_Assigned : TanState.Review_Assigned;
                            AddTanAction(db, tanToBeAllot, TanAction.TAN_ASSIGNED, user, currentUserRole.Role, $"Tan Assigned to {user.UserName} by Automation Process");
                            db.Entry(tanToBeAllot).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        scope.Complete();
                    }
                }
            }
            else if (currentRoleId == (int)Role.QC)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var tansToBeAllotted = GetAllTans()
                                           .Where(t => t.TanState == TanState.Review_Accepted && t.QCRequired && t.Curator != null && t.CuratorId != currentUserId && t.Reviewer != null && t.ReviewerId != currentUserId && t.QC == null).Take(25);
                    var tansFromUniqueCombination = GetAllTans()
                                                    .Where(t => t.TanState == TanState.Review_Accepted && !t.QCRequired && t.Curator != null && t.CuratorId != currentUserId && t.Reviewer != null && t.ReviewerId != currentUserId && t.QC == null)
                                                    .GroupBy(x => new { x.CuratorId, x.ReviewerId }).Select(x => new { id = x.Key, tans = x.ToList() }).ToDictionary(d => d.id, d => d.tans.FirstOrDefault());
                    if (tansToBeAllotted != null && tansToBeAllotted.Count() > 1)
                    {
                        foreach (var tanToBeAllot in tansToBeAllotted)
                        {
                            tanToBeAllot.QC = user;
                            tanToBeAllot.CurrentUser = user;
                            tanToBeAllot.TanState = TanState.QC_Assigned;
                            tanToBeAllot.CurrentUserRole = currentUserRole;
                            AddTanAction(db, tanToBeAllot, TanAction.TAN_ASSIGNED, user, currentUserRole.Role, $"Tan Assigned to {user.UserName} by Automation Process");
                            db.Entry(tanToBeAllot).State = EntityState.Modified;
                        }
                        var tans = tansFromUniqueCombination.Select(x => x.Value).ToList();
                        foreach (var tanToBeAllot in tans)
                        {
                            tanToBeAllot.QC = user;
                            tanToBeAllot.CurrentUser = user;
                            tanToBeAllot.TanState = TanState.QC_Assigned;
                            tanToBeAllot.CurrentUserRole = currentUserRole;
                            db.Entry(tanToBeAllot).State = EntityState.Modified;
                            AddTanAction(db,tanToBeAllot,TanAction.TAN_ASSIGNED,user,currentUserRole.Role,$"Tan Assigned to {user.UserName} by Automation Process");
                        }
                        db.SaveChanges();
                        scope.Complete();
                    }
                }
            }
            #endregion
            pullTask.UserRanks.Sort();
            pullTask.TanRanks.Sort();
            return pullTask;
        }

        private void AddTanAction(ApplicationDbContext dbc,Tan tan, TanAction tanAction, ApplicationUser user,Role role, String Description)
        {
            TanActionHistories tah = new TanActionHistories();
            tah.TanAction = tanAction;
            tah.Tan = tan;
            tah.UpdatedDate = DateTime.Now;
            tah.User = user;
            tah.Role = role;
            tah.Description = Description;
            dbc.TanActionHistories.Add(tah);
        }

        private TaskDTO GetTaskDTO(Tan tan, int currentRoleId, ApplicationUser user)
        {
            var tanData = GetTanData(tan.Id);
            var daystoAdd = user.BenchMark.HasValue && user.BenchMark.Value != 0 ? Math.Round((tanData.TanChemicals.Where(n => n.ChemicalType == ChemicalType.NUM).Select(n => n.NUM).Count() / user.BenchMark.Value) * 8.5) : 0;
            return new TaskDTO
            {
                TanName = tan.tanNumber,
                BatchNo = tan.Batch.Name,
                Id = tan.Id,
                Status = tan.TanState.ToString(),
                NUMsCount = tanData != null ? tanData.TanChemicals.Where(n => n.ChemicalType == ChemicalType.NUM).Select(n => n.NUM).Count() : 0,
                RXNsCount = tanData != null ? tanData.Reactions.Count() : 0,
                Analyst = tan.Curator?.UserName,
                Reviewer = tan.Reviewer?.UserName,
                QC = tan.QC?.UserName,
                Version = tan.Version.HasValue ? tan.Version.Value : 0,
                TanCompletionDate = tanData != null ? System.DateTime.Now.AddDays(daystoAdd).ToString() : string.Empty,
                ProcessingNote = tan.ProcessingNode,
                NearToTargetDate = tan.TargetedDate.HasValue && (tan.TargetedDate.Value - DateTime.Now).TotalDays <= 5 ? true : false
            };
        }

        public int ShipmentCount()
        {
            return db.Batches.Count();
        }

        public int ToatlCuratedTansCount()
        {
            return db.Tans.Where(t => t.TanState.HasValue && t.TanState.Value >= TanState.Curation_Submitted).Count();
        }
        public int TansCuratedCount(int Role)
        {
            Dictionary<DateTime?, int> data = new Dictionary<DateTime?, int>();
            if(Role == 1)
                data = db.TanActionHistories.Where(t => t.TanAction == TanAction.TAN_SUBMITTED).GroupBy(t => DbFunctions.TruncateTime(t.UpdatedDate)).ToDictionary(k => k.Key, k => k.Count());
            else if(Role == 2)
                data = db.TanActionHistories.Where(t => t.TanAction == TanAction.TAN_APPROVED && t.Role == Entities.Role.Reviewer).GroupBy(t => DbFunctions.TruncateTime(t.UpdatedDate)).ToDictionary(k => k.Key, k => k.Count());
            else if (Role == 3)
                data = db.TanActionHistories.Where(t => t.TanAction == TanAction.TAN_APPROVED && t.Role == Entities.Role.QC).GroupBy(t => DbFunctions.TruncateTime(t.UpdatedDate)).ToDictionary(k => k.Key, k => k.Count());
            if (data.ContainsKey(DateTime.Now.Date))
                return data[DateTime.Now.Date];
            return 0;
            //if (Role == 1)
            //    return (from tans in db.Tans where tans.TanState == TanState.Curation_Submitted || tans.TanState == TanState.Review_Assigned || tans.TanState == TanState.Review_InProgress || tans.TanState == TanState.Review_Accepted select tans).Count();
            //else if (Role == 2)
            //    return (from tans in db.Tans where tans.TanState == TanState.Review_Accepted || tans.TanState == TanState.QC_Assigned || tans.TanState == TanState.QC_InProgress select tans).Count();
            //else if (Role == 3)
            //    return (from tans in db.Tans where tans.TanState == TanState.QC_Accepted select tans).Count();
            //return 0;
        }
        /// <summary>
        /// To get all reactions count from all tans
        /// </summary>
        /// <param name="today">to get current day Reactions count</param>
        /// <returns>Returns int value of reactions count</returns>
        public int totalReactions(bool today)
        {
            if (today)
            {
                var dateWiseData = db.DateWiseRXNCount.GroupBy(d => d.UpdatedDate).ToDictionary(r => r.Key, r => r.Sum(k => k.RxnCount));
                //var data = ReactionsCountDateWise();
                if (dateWiseData.ContainsKey(DateTime.Now.Date))
                    return dateWiseData[DateTime.Now.Date];
                else return 0;
            }
            else
                return db.Tans.Sum(t=>t.RxnCount);
        }

        public int TotalCuratedTans()
        {
            return db.Tans.Where(t => t.TanState == TanState.Curation_Submitted).Count();
        }
        public int TotalReviewedTans()
        {
            return db.Tans.Where(t => t.TanState == TanState.QC_Assigned || t.TanState == TanState.QC_InProgress).Count();
        }

        public int TotalQCedTans()
        {
            return db.Tans.Where(t => t.TanState == TanState.QC_Accepted).Count();
        }//TansInprogress

        public int TansInprogress()
        {
            return db.Tans.Where(t => t.TanState.HasValue && StaticStore.InprogressState.Contains(t.TanState.Value)).Count();
        }

        /// <summary>
        /// To Get the all reactions from all tans
        /// </summary>
        /// <param name="tanIds">Optional parameter to get the Reactions from specified tanIds</param>
        /// <returns>It will return all the Reactions from all the tans</returns>
        public List<Reaction> GetAllReactions(List<int> tanIds = null)
        {
            List<Reaction> data = new List<Reaction>();
            var tanDatas = tanIds != null ? from tans in db.TanData where tanIds.Contains(tans.TanId) select tans : db.TanData;
            foreach (var tanData in tanDatas)
            {
                var tandata = JsonConvert.DeserializeObject<Tan>(tanData.Data);
                data.AddRange(tandata.Reactions);
            }
            return data;
        }
        /// <summary>
        /// To get date wise reactions count from all tans
        /// </summary>
        /// <param name="tanIds">To get specified tans data</param>
        /// <returns>Returns dictionary of date wise reactions count</returns>
        public Dictionary<DateTime, int> ReactionsCountDateWise(List<int> tanIds = null, int roleid = 1)
        {
            var data = GetAllReactions(tanIds);
            if (data.Any())
                return data.GroupBy(r => (roleid == 1 && r.CuratorCreatedDate.HasValue) ? r.CuratorCreatedDate.Value.Date : (roleid == 2 && r.ReviewerCreatedDate.HasValue) ? r.ReviewerCreatedDate.Value.Date : r.QCLastUpdatedDate.Date).ToDictionary(da => da.Key, da => da.Count());
            return null;
        }

        public Dictionary<DateTime, int> ReactionsCountDateWiseSummary()
        {
            var data = db.DateWiseRXNCount.GroupBy(d => DbFunctions.TruncateTime(d.UpdatedDate)).ToDictionary(r => r.Key.Value, r => r.Sum(k => k.RxnCount));
            return data;
            
        }

        public Dictionary<DateTime, ReactionsCount> ReactionsCountDateWisewithStages(List<int> tanIds = null, int roleid = 1, bool fromUpdatedDate = false, bool fromCompletedDate = false)
        {
            var data = GetAllReactions(tanIds);
            if (data.Any())
            {
                if (fromUpdatedDate)
                {
                    return data.GroupBy(r => roleid == 1 ? r.LastUpdatedDate.Date : roleid == 2 ? r.ReviewLastUpdatedDate.Date : r.QCLastUpdatedDate.Date).ToDictionary(da => da.Key, da => new ReactionsCount
                    {
                        TotalReactionsCount = da.Count(),
                        AnalogousReactionsCount = da.Where(r => r.AnalogousFromId != null).Count(),
                        StagesCount = da.Select(r => r.Stages.Count()).Sum()
                    });
                }
                else if(fromCompletedDate)
                {
                    return data.GroupBy(r => (roleid == 1 && r.CuratorCompletedDate.HasValue) ? r.CuratorCompletedDate.Value.Date : (roleid == 2 && r.ReviewerCompletedDate.HasValue) ? r.ReviewerCompletedDate.Value.Date : r.QCLastUpdatedDate.Date).ToDictionary(da => da.Key, da => new ReactionsCount
                    {
                        TotalReactionsCount = da.Count(),
                        AnalogousReactionsCount = da.Where(r => r.AnalogousFromId != null).Count(),
                        StagesCount = da.Select(r => r.Stages.Count()).Sum()
                    });
                }
                else
                {
                    return data.GroupBy(r => (roleid == 1 && r.CuratorCreatedDate.HasValue) ? r.CuratorCreatedDate.Value.Date : (roleid == 2 && r.ReviewerCreatedDate.HasValue) ? r.ReviewerCreatedDate.Value.Date : r.QCLastUpdatedDate.Date).ToDictionary(da => da.Key, da => new ReactionsCount
                    {
                        TotalReactionsCount = da.Count(),
                        AnalogousReactionsCount = da.Where(r => r.AnalogousFromId != null).Count(),
                        StagesCount = da.Select(r => r.Stages.Count()).Sum()
                    });
                }
            }
            return null;
        }


        public int TotalTans()
        {
            return db.Tans.Count();
        }

        public int curationProgress()
        {
            return (from tans in db.Tans where tans.TanState == TanState.Curation_InProgress select tans).Count();
        }

        public object GetTan(int tanId, string userId, int currentRole)
        {
            var tan = (from t in GetAllTans() where t.Id == tanId select t).FirstOrDefault();
            var tanWiseKeyWords = db.TanWiseKeywords.Where(tkw => tkw.TanId == tanId).FirstOrDefault();
            var tanData = db.TanData.Where(td => td.TanId == tanId).FirstOrDefault();
            using (TransactionScope scope = new TransactionScope())
            {
                if (tanData == null)
                {
                    var tanJson = JsonConvert.SerializeObject(tan);
                    tanData = new TanData() { TanId = tan.Id, Data = tanJson };
                    db.TanData.Add(tanData);
                }
                //var tanDataToUpdate = JsonConvert.DeserializeObject<Tan>(tanData.Data);
                //tanDataToUpdate.DocumentPath = System.IO.Path.Combine(C.ShipmentSharedPath, tanDataToUpdate.DocumentPath);
                //tanDataToUpdate.TotalDocumentsPath = string.Join(",", tanDataToUpdate.TotalDocumentsPath.Split(',').Select(l => System.IO.Path.Combine(C.ShipmentSharedPath, l)).ToList());
                //foreach (var chemical in tanDataToUpdate.TanChemicals)
                //{
                //    chemical.ImagePath = System.IO.Path.Combine(C.ShipmentSharedPath, chemical.ImagePath);
                //    foreach (var substance in chemical.Substancepaths)
                //        substance.ImagePath = System.IO.Path.Combine(C.ShipmentSharedPath, substance.ImagePath);
                //}
                tan.LastAccessedBy = db.Users.Find(userId);
                tan.LastAccessedTime = DateTime.Now;
                if (currentRole != 4 && currentRole != 5)
                    tan.TanState = GetTanStatus(currentRole, tan);
                if (string.IsNullOrEmpty(tan.DocumentReviwedUser))
                    tan.DocumentReadStartTime = DateTime.Now;
                db.SaveChanges();
                scope.Complete();
                var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
                context.Clients.All.curated(LiveHub.TAN_CURATED);
            }
            return new TanInfoDTO { Tan = tan, TanData = tanData, TanWiseKeyWords = tanWiseKeyWords != null ? tanWiseKeyWords.TanKeywords : null };
        }

        public object GetRoleBasedTanData(int tanId, string userId, int currentRole)
        {
            var tan = (from t in GetAllTans() where t.Id == tanId select t).FirstOrDefault();
            if (currentRole == 1)
                userId = tan.CuratorId;
            else if (currentRole == 2)
                userId = tan.ReviewerId;
            else
                userId = tan.QCId;
            var tanWiseKeyWords = db.TanWiseKeywords.Where(tkw => tkw.TanId == tanId).FirstOrDefault();
            var tanData = db.TanData.Where(td => td.TanId == tanId).FirstOrDefault();
            var tanHistoryData = db.TanHistory.Where(th => th.TanId == tanId && th.UserRoleId == currentRole).OrderByDescending(d => d.Date).FirstOrDefault();
            if (tanHistoryData != null)
                tanData = new TanData
                {
                    Data = tanHistoryData.Data,
                    Date = tanHistoryData.Date,
                    Id = tanHistoryData.id,
                    Tan = tan,
                    User = userId
                };
            else
                throw new Exception("Specified Role data not found");
            return new TanInfoDTO
            {
                Tan = tan,
                TanData = tanData,
                TanWiseKeyWords = tanWiseKeyWords != null ? tanWiseKeyWords.TanKeywords : null
            };
        }

        public TanState GetTanStatus(int role, Tan tan)
        {
            TanState state = tan.TanState.HasValue ? tan.TanState.Value : TanState.Not_Assigned;
            if (role == (int)Role.Curator)
            {
                if (tan.TanState == TanState.Curation_Assigned_Rejected)
                    state = TanState.Curation_Progress_Rejected;
                else if(tan.TanState == TanState.Curation_Assigned || tan.TanState == TanState.Curation_ReAssigned)
                    state = TanState.Curation_InProgress;
            }
            else if (role == (int)Role.Reviewer)
            {
                if (tan.TanState == TanState.Review_Assigned_Rejected)
                    state = TanState.Review_Progress_Rejected;
                else if (tan.TanState == TanState.Review_Assigned || tan.TanState == TanState.Review_ReAssigned || tan.TanState == TanState.Curation_Submitted)
                    state = TanState.Review_InProgress;
            }
            else if (role == (int)Role.QC)
                if (tan.TanState == TanState.QC_Assigned || tan.TanState == TanState.QC_ReAssigned)
                    state = TanState.QC_InProgress;
            return state;
        }

        public List<DateWiseRXNCount> GetdateWiseRXNCount(Tan userTanData, Tan dbTan, ApplicationUser user, Role role)
        {
            List<DateWiseRXNCount> returnData = new List<DateWiseRXNCount>();
            var DateWisedata = userTanData.Reactions.Where(r => role == Role.Curator ? r.CuratorCompletedDate.HasValue : role == Role.Reviewer ? r.ReviewerCompletedDate.HasValue : r.QCCompletedDate.HasValue)
                                                    .GroupBy(r => role == Role.Curator ? r.CuratorCompletedDate.Value.Date : role == Role.Reviewer ? r.ReviewerCompletedDate.Value.Date : r.QCCompletedDate.Value.Date)
                                                    .ToDictionary(t => t.Key, t => t.ToList());
            var existing = db.DateWiseRXNCount.Where(d => d.TanId == dbTan.Id && d.UserId == user.Id && d.Role == role).ToList();
            if (!existing.Any())
            {
                foreach (var date in DateWisedata)
                {
                    if (date.Key != DateTime.MinValue)
                    {
                        DateWiseRXNCount dwr = new DateWiseRXNCount();
                        dwr.Role = role;
                        dwr.Tan = dbTan;
                        dwr.User = user;
                        dwr.UpdatedDate = date.Key;
                        dwr.RxnCount = date.Value.Count();
                        returnData.Add(dwr);
                    }
                }
            }
            else
            {
                foreach (var item in existing)
                {
                    item.RxnCount = DateWisedata.ContainsKey(item.UpdatedDate.Date) ? DateWisedata[item.UpdatedDate.Date].Count() : 0;
                }
            }
            return returnData;
        }

        public Role GetUserRole(int currentRole)
        {
            if (currentRole == 1)
                return Role.Curator;
            else if (currentRole == 2)
                return Role.Reviewer;
            else if (currentRole == 3)
                return Role.QC;
            else if (currentRole == 4)
                return Role.ToolManager;
            else
                return Role.ProjectManger;
        }

        public string SaveTan(string userId, OfflineDTO tanData, int currentUserRole, string ip, string UserName)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                LogSteps.Log($"Tan Saving from User: {UserName} and Role: {currentUserRole}");
                ApplicationUser user = db.Users.Find(userId);
                var userTan = JsonConvert.DeserializeObject<TanData>(tanData.TanData);
                var userTanData = JsonConvert.DeserializeObject<Tan>(userTan.Data);
                var dbTan = (from t in GetAllTans() where t.Id == userTan.TanId select t).FirstOrDefault();
                db.DateWiseRXNCount.AddRange(GetdateWiseRXNCount(userTanData, dbTan, user, GetUserRole(currentUserRole)));

                if (dbTan.CurrentUserRole.ApplicationUser.Id == userId && (int)dbTan.CurrentUserRole.Role == currentUserRole)
                {
                    dbTan.RxnCount = userTanData.Reactions.Count();
                    if (dbTan.Version == null)
                        dbTan.Version = 0;
                    dbTan.Version += 1;

                    if (currentUserRole == (int)Role.Curator &&
                        tanData.DocumentReadStartTime != null &&
                        tanData.DocumentReadStartTime != DateTime.MinValue &&
                        tanData.DocumentReadEndTime != null &&
                        tanData.DocumentReadEndTime != DateTime.MinValue)
                    {
                        dbTan.DocumentReviwedUser = tanData.DocumentReviewedUserId;
                        dbTan.DocumentReadStartTime = tanData.DocumentReadStartTime;
                        dbTan.DocumentReadCompletedTime = tanData.DocumentReadEndTime;
                    }
                    dbTan.DocumentPath = tanData.TanDocumentKeyPath;
                    dbTan.TotalDocumentsPath = tanData.TotalPaths;
                    dbTan.DocumentCurrentPage = userTanData.DocumentCurrentPage;
                    var dbTanData = db.TanData.Where(td => td.TanId == userTan.TanId).First();
                    bool newData = false;
                    TanHistory tanHistory = db.TanHistory.Where(th => th.TanId == dbTanData.TanId && th.UserId == userId && th.UserRoleId == currentUserRole).FirstOrDefault();
                    if (tanHistory == null)
                    {
                        tanHistory = new TanHistory();
                        newData = true;
                    }
                    tanHistory.TanId = dbTanData.TanId;
                    tanHistory.Data = dbTanData.Data;
                    tanHistory.Date = DateTime.Now;
                    tanHistory.Ip = ip;
                    tanHistory.UserId = userId;
                    tanHistory.UserRoleId = currentUserRole;
                    dbTanData.Data = userTan.Data;
                    dbTanData.Date = DateTime.Now;
                    dbTanData.User = userId;
                    dbTanData.Ip = ip;
                    if (newData)
                        db.TanHistory.Add(tanHistory);
                    db.SaveChanges();
                    scope.Complete();
                    var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
                    context.Clients.All.curated(LiveHub.TAN_CURATED);
                    LogSteps.Log($"Tan Saved Success from User: {UserName} and Role: {currentUserRole}");
                    return "Tan Saved Successfully";
                }
                else
                    throw new Exception("Current user is not allowed to save TAN");
            }
        }

        public object SaveException(ActivityTracing trace)
        {
            var dbLog = new ApplicationDbContext();
            dbLog.ActivityTracing.Add(trace);
            dbLog.SaveChanges();
            return true;
        }

        public List<SolventBoilingPoints> GetSolventBoilingPoints()
        {
            return db.SolventBoilingPoints.ToList();
        }
        public List<NamePriorities> GetNamePriorities()
        {
            return db.NamePriorities.ToList();
        }


        public List<CVT> GetCVTs()
        {
            return db.CVT.ToList();
        }
        public List<FreeText> GetFreetexts()
        {
            return db.FreeText.ToList();
        }


        public UserInfoDTO GetUserInfo(string userId, int currentRoleId)
        {
            UserInfoDTO userinfoDto = new UserInfoDTO();
            userinfoDto.canApprove = currentRoleId == (int)Role.Curator ? false : true;
            userinfoDto.canSubmit = currentRoleId == (int)Role.Curator ? true : false;
            userinfoDto.canReject = currentRoleId == (int)Role.Curator ? false : true;
            return userinfoDto;
        }
        public bool ApproveTan(int TanId, int currentRole, string userId, bool QCRequired)
        {
            Tan tan = null;
            var user = db.Users.Find(userId);
            if (TanId != 0)
                tan = GetAllTans().Where(t => t.Id == TanId).SingleOrDefault();
            if (tan != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var dbTanData = db.TanData.Where(td => td.TanId == TanId).First();
                    TanHistory tanHistory = new TanHistory();
                    tanHistory.TanId = dbTanData.TanId;
                    tanHistory.Data = dbTanData.Data;
                    tanHistory.Date = DateTime.Now;
                    tanHistory.UserId = userId;
                    tanHistory.UserRoleId = currentRole;
                    db.TanHistory.Add(tanHistory);
                    Tan tanData = GetTanData(TanId);
                    tan.TanState = currentRole == (int)Role.Reviewer ? TanState.Review_Accepted : TanState.QC_Accepted;

                    TanActionHistories tah = new TanActionHistories();
                    tah.Role = GetUserRole(currentRole);
                    tah.Tan = tan;
                    tah.TanAction = TanAction.TAN_APPROVED;
                    tah.UpdatedDate = DateTime.Now;
                    tah.User = user;
                    tah.Description = $"Tan Approved from {user.UserName}.";
                    db.TanActionHistories.Add(tah);

                    if (currentRole == (int)Role.Reviewer && tan.QC != null)
                    {
                        tan.CurrentUser = tan.QC;
                        tan.CurrentUserRole = db.UserRoles.Where(ur => ur.ApplicationUser.Id == tan.QC.Id && ur.Role == Role.QC).FirstOrDefault();
                    }
                    else
                    {
                        tan.CurrentUser = null;
                        tan.CurrentUserRole = null;
                    }
                    if (currentRole == (int)Role.Reviewer && QCRequired)
                    {
                        tan.QCRequired = true;
                        TanMetaDataUpdateHistory history = new TanMetaDataUpdateHistory();
                        history.Tan = tan;
                        history.TanMetaDataUpdateAction = TanMetaDataUpdateAction.QCREQUIREDSTATUSUPDATION;
                        history.UpdatedDate = DateTime.Now;
                        history.User = db.Users.Find(userId);
                        db.TanMetaDataUpdateHistory.Add(history);
                    }
                    db.SaveChanges();
                    scope.Complete();
                    var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
                    context.Clients.All.accepted(LiveHub.TAN_ACCEPTED);
                    return true;
                }
            }
            return false;
        }
        bool checkOrder(int a, int b, bool ascendeingOrder)
        {
            return ascendeingOrder ? a > b : a < b;
        }

        public bool RejectTan(int TanId, int currentRole, string userId)
        {
            Tan tan = null;
            var user = db.Users.Find(userId);
            if (TanId != 0)
                tan = GetAllTans().Where(t => t.Id == TanId).SingleOrDefault();
            if (tan != null)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var dbTanData = db.TanData.Where(td => td.TanId == TanId).First();
                    TanHistory tanHistory = new TanHistory();
                    tanHistory.TanId = dbTanData.TanId;
                    tanHistory.Data = dbTanData.Data;
                    tanHistory.Date = DateTime.Now;
                    tanHistory.UserId = userId;
                    tanHistory.UserRoleId = currentRole;
                    db.TanHistory.Add(tanHistory);

                    TanActionHistories tah = new TanActionHistories();
                    tah.Role = currentRole == (int)Role.Reviewer ? Role.Reviewer : Role.QC;
                    tah.Tan = tan;
                    tah.TanAction = TanAction.TAN_REJECTED;
                    tah.UpdatedDate = DateTime.Now;
                    tah.User = user;
                    tah.Description = $"Tan Rejected from {user.UserName}.";
                    db.TanActionHistories.Add(tah);

                    //Tan tanData = GetTanData(TanId);
                    var previousUser = currentRole == (int)Role.Reviewer ? tan.Curator : tan.Reviewer;
                    tan.TanState = currentRole == (int)Role.Reviewer ? TanState.Curation_Assigned_Rejected : TanState.Review_Assigned_Rejected;
                    tan.CurrentUserRole = currentRole == (int)Role.Reviewer ? db.UserRoles.Where(ur => ur.ApplicationUser.Id == tan.Curator.Id && ur.Role == Role.Curator).FirstOrDefault() :
                                          db.UserRoles.Where(ur => ur.ApplicationUser.Id == tan.Reviewer.Id && ur.Role == Role.Reviewer).FirstOrDefault();
                    tan.CurrentUser = currentRole == (int)Role.Reviewer ? tan.Curator : tan.Reviewer;
                    db.SaveChanges();
                    scope.Complete();
                    return true;
                }
            }
            return false;
        }
        public bool SubmitTan(int TanId, int currentRole, string userId)
        {
            Tan tan = null;
            var user = db.Users.Find(userId);
            if (TanId != 0)
                tan = GetAllTans().Where(t => t.Id == TanId).SingleOrDefault();
            if (tan != null)
            {
                //Tan tanData = GetTanData(TanId);
                UserRole currentUserRole = null;
                if (tan.Reviewer != null)
                    currentUserRole = db.UserRoles.Where(ur => ur.ApplicationUser.Id == tan.Reviewer.Id && ur.Role == Role.Reviewer).FirstOrDefault();
                using (TransactionScope scope = new TransactionScope())
                {

                    var dbTanData = db.TanData.Where(td => td.TanId == TanId).First();
                    TanHistory tanHistory = new TanHistory();
                    tanHistory.TanId = dbTanData.TanId;
                    tanHistory.Data = dbTanData.Data;
                    tanHistory.Date = DateTime.Now;
                    tanHistory.UserId = userId;
                    tanHistory.UserRoleId = currentRole;
                    db.TanHistory.Add(tanHistory);
                    TanActionHistories tah = new TanActionHistories();
                    tah.Role = Role.Curator;
                    tah.Tan = tan;
                    tah.TanAction = TanAction.TAN_SUBMITTED;
                    tah.UpdatedDate = DateTime.Now;
                    tah.User = user;
                    tah.Description = $"Tan Submitted from {user.UserName}.";
                    db.TanActionHistories.Add(tah);
                    tan.TanState = TanState.Curation_Submitted;
                    tan.CurrentUserRoleId = currentUserRole != null ? currentUserRole.Id : (int?)null;
                    tan.CurrentUserId = tan.Reviewer != null ? tan.Reviewer.Id : null;
                    db.SaveChanges();
                    scope.Complete();
                    var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
                    context.Clients.All.curated(LiveHub.TAN_CURATED);
                    return true;
                }
            }
            return false;
        }

        public Tan GetTanData(int tanId)
        {
            var data = (from t in db.TanData where t.TanId == tanId select t.Data).FirstOrDefault();
            Tan tanData = null;
            if (data != null)
                tanData = JsonConvert.DeserializeObject<Tan>(data);
            return tanData;
        }

        public List<TanHistoryDTO> GetVersions(string tanNumber)
        {
            List<TanHistoryDTO> versions = new List<TanHistoryDTO>();
            using (var db = new ApplicationDbContext())
            {
                var histories = db.TanHistory.Where(th => th.Tan.tanNumber == tanNumber).OrderByDescending(th => th.Date).ToList();
                foreach (var history in histories)
                {
                    versions.Add(new TanHistoryDTO
                    {
                        Id = history.id,
                        Text = String.Join(" - ", history.User.UserName, history.Date.Value.ToString("dd-MM-yy hh:mm:ss tt"))
                    });
                }
            }
            return versions;
        }

        public ArrayList PastFewDaysCount(int productRoleStateId)
        {
            ArrayList pastFewDaysCount = new ArrayList();
            ArrayList series = new ArrayList();
            ArrayList ticks = new ArrayList();
            DateTime today = DateTime.Now;
            DateTime pastDay = DateTime.Now.AddDays(-MAX_REPORT_DAYS);
            var list = ReactionsCountDateWiseSummary();
            for (int i = MAX_REPORT_DAYS; i >= 0; i--)
            {
                DateTime beforeDate = DateTime.Now.AddDays(-i).Date;
                int count = list.ContainsKey(beforeDate) ? list[beforeDate] : 0;
                series.Add(count);
                ticks.Add(beforeDate.ToString("dd-MM"));
            }
            pastFewDaysCount.Add(series);
            pastFewDaysCount.Add(ticks);
            return pastFewDaysCount;
        }

        public List<UserReportsDTO> UserReactionsReports(string userId, int currentRoleId)
        {
            List<UserReportsDTO> userReports = new List<UserReportsDTO>();
            var SingleUserList = ReactionsCountDateWise(GetTanNumbers(currentRoleId, userId, true));
            if (SingleUserList != null && SingleUserList.Any())
            {
                for (int i = MAX_REPORT_DAYS; i >= 0; i--)
                {
                    DateTime beforeDate = DateTime.Now.AddDays(-i).Date;
                    UserReportsDTO reportsDto = new UserReportsDTO();
                    reportsDto.Date = beforeDate.ToString("dd-MM");
                    reportsDto.ReactionsCount = SingleUserList.ContainsKey(beforeDate) ? SingleUserList[beforeDate] : 0;
                    reportsDto.SingleUser = true;
                    userReports.Add(reportsDto);
                }
            }
            var AllUserList = ReactionsCountDateWise(GetTanNumbers(currentRoleId, userId, false));
            if (AllUserList != null && AllUserList.Any())
            {
                for (int i = MAX_REPORT_DAYS; i >= 0; i--)
                {
                    DateTime beforeDate = DateTime.Now.AddDays(-i).Date;
                    UserReportsDTO reportsDto = new UserReportsDTO();
                    reportsDto.Date = beforeDate.ToString("dd-MM");
                    reportsDto.ReactionsCount = AllUserList.ContainsKey(beforeDate) ? AllUserList[beforeDate] : 0;
                    reportsDto.SingleUser = false;
                    userReports.Add(reportsDto);
                }
            }
            return userReports;
        }
        /// <summary>
        /// To get the tan ids for perticuler user or other users
        /// </summary>
        /// <param name="currentRoleId">to get the tans based on the current role</param>
        /// <param name="userId">to get the specified user or other than current user</param>
        /// <param name="singleUser">to specify the current user or not</param>
        /// <returns></returns>
        public List<int> GetTanNumbers(int currentRoleId, string userId, bool singleUser)
        {
            if (currentRoleId == (int)Role.Curator)
            {
                var tans = GetAllTans();
                var ids = from tan in tans
                          where tan.CuratorId != null && (singleUser ? tan.CuratorId == userId : tan.CuratorId != userId)
                          select tan.Id;
                return ids.ToList();
            }
            else if (currentRoleId == (int)Role.Reviewer)
            {
                return (from tan in GetAllTans()
                        where tan.Reviewer != null && (singleUser ? tan.Reviewer.Id == userId : tan.Reviewer.Id != userId)
                        select tan.Id).ToList();
            }
            else
            {
                return (from tan in GetAllTans()
                        where tan.QC != null && (singleUser ? tan.QC.Id == userId : tan.QC.Id != userId)
                        select tan.Id).ToList();
            }
        }

        public Dictionary<string,List<int>> UserWiseTanNumbers(Role role, List<string> TanStates)
        {
            var tans = GetAllTans();
            var FilteredTans = !TanStates.Contains(TanState.ALL.ToString()) ?  tans.Where(t => t.TanState.HasValue && TanStates.Contains(t.TanState.Value.ToString())) : tans;
            Dictionary<string, List<int>> ids = new Dictionary<string, List<int>>();
            if (role == Role.Curator)
                ids = FilteredTans.Where(t => t.CuratorId != null).GroupBy(t => t.CuratorId).ToDictionary(t => t.Key, t => t.Select(v => v.Id).ToList());
            else if(role == Role.Reviewer)
                ids = FilteredTans.Where(t => t.ReviewerId != null).GroupBy(t => t.ReviewerId).ToDictionary(t => t.Key, t => t.Select(v => v.Id).ToList());
            else if (role == Role.QC)
                ids = FilteredTans.Where(t => t.QCId != null).GroupBy(t => t.QCId).ToDictionary(t => t.Key, t => t.Select(v => v.Id).ToList());
            return ids;
        }


        public ErrorReportDto GetErrorReport(ErrorReportDto reportDto)
        {
            if (reportDto != null && reportDto.TanID != 0)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var SecondRoleData = db.TanHistory.Where(th => th.TanId == reportDto.TanID && th.UserRoleId == reportDto.SecondRoleId).OrderByDescending(d => d.Date).FirstOrDefault();
                    if(SecondRoleData != null)
                    {
                        var FirstRoleData = db.TanHistory.Where(th => th.TanId == reportDto.TanID && th.UserRoleId == reportDto.FirstRoleId).OrderByDescending(d => d.Date).FirstOrDefault();
                        reportDto.FirstRoleTanData = FirstRoleData.Data;
                        reportDto.SecondRoleTanData = db.TanData.Where(t=>t.TanId == reportDto.TanID).FirstOrDefault().Data;
                        reportDto.FirstUserName = FirstRoleData.User.UserName;
                        reportDto.SecondUserName = SecondRoleData.User.UserName;
                    }
                }
            }
            return reportDto;
        }

        public ErrorPercentageDto GetErrorPercReport(ErrorPercentageDto ErrorPerDto)
        {
            if (ErrorPerDto != null && ErrorPerDto.Date != null)
            {
                var firstRoleData = from firstth in db.TanHistory.Where(th => th.Date == ErrorPerDto.Date && th.UserRoleId == ErrorPerDto.RoleID).GroupBy(th => th.TanId, (key, g) => g.OrderByDescending(e => e.Date).First())
                                    join secondth in db.TanHistory.Where(th => th.Date == ErrorPerDto.Date && th.UserRoleId == ErrorPerDto.RoleID + 1).GroupBy(th => th.TanId, (key, g) => g.OrderByDescending(e => e.Date).First())
                                    on firstth.TanId equals secondth.TanId
                                    select new { firstth.Data };
                if (firstRoleData != null)
                    ErrorPerDto.FirstRoleTanData = firstRoleData.Select(x => x.Data).ToList();
                var secondRoleData = db.TanHistory.Where(th => th.Date == ErrorPerDto.Date && th.UserRoleId == ErrorPerDto.RoleID).GroupBy(th => th.TanId, (key, g) => g.OrderByDescending(e => e.Date).First()).ToList();
                if (secondRoleData != null)
                    ErrorPerDto.SecondRoleTanData = secondRoleData.Select(x => x.Data).ToList();
            }
            return ErrorPerDto;
        }

    }
}