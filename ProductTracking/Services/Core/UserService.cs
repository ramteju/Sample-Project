using ProductTracking.Models;
using ProductTracking.Models.Core;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Microsoft.AspNet.Identity;
using Entities;
using Entities.DTO;
using System.Transactions;
using System.Collections;
using System;

namespace ProductTracking.Services.Core
{
    public class UserService
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public AppSettings GetSettings()
        {
            return db.AppSettings.FirstOrDefault();
        }

        public ApplicationUser SyncLdapUser(string userName, string password, ApplicationUserManager UserManager)
        {
            ApplicationUser user = db.Users.Where(u => u.UserName.CompareTo(userName) == 0).FirstOrDefault();
            //create user if not exists in db.
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = userName + "@excelra.com",
                    AnalystId = 0,
                    BenchMark = 0
                };
                var identityResult = UserManager.Create(user, password);
            }

            //update password if ldap password is different from db password.
            else if (user != null)
            {
                UserManager.RemovePassword(user.Id);
                UserManager.AddPassword(user.Id, password);
            }
            return user;
        }
        public int RoleWiseCount(int role)
        {
            return (from userRoles in db.UserRoles where (int)userRoles.Role == role select userRoles).Count();
        }

        public Dictionary<ApplicationUser, List<Role>> UserWiseRoles()
        {
            return db.UserRoles.GroupBy(ur => ur.ApplicationUser).ToDictionary(d => d.Key, d => d.Select(ur => ur.Role).ToList());
        }
        public Dictionary<int, List<ApplicationUser>> RoleWiseUsers()
        {
            return db.UserRoles.GroupBy(ur => ur.Role).ToList().ToDictionary(d => (int)d.Key, d => d.Select(ur => ur.ApplicationUser).ToList());
        }

        public Dictionary<int, List<UserRole>> WorkflowWiseUserRoles()
        {
            Dictionary<int, List<UserRole>> workflowBasedUserRoles = new Dictionary<int, List<UserRole>>();
            return workflowBasedUserRoles;
        }

        public List<UserDTO> GetAllCurators()
        {
            List<UserDTO> users = new List<UserDTO>();
            var allCurators = db.UserRoles.Where(role => role.Role == Role.Curator).Select(role => role.ApplicationUser);
            foreach (var user in allCurators)
            {
                users.Add(new UserDTO
                {
                    Name = user.UserName,
                    Role = Role.Curator,
                    UserID = user.Id
                });
            }

            var allReviewers = db.UserRoles.Where(role => role.Role == Role.Reviewer).Select(role => role.ApplicationUser);
            foreach (var user in allReviewers)
            {
                users.Add(new UserDTO
                {
                    Name = user.UserName,
                    Role = Role.Reviewer,
                    UserID = user.Id
                });
            }

            var allQCs = db.UserRoles.Where(role => role.Role == Role.QC).Select(role => role.ApplicationUser);
            foreach (var user in allQCs)
            {
                users.Add(new UserDTO
                {
                    Name = user.UserName,
                    Role = Role.QC,
                    UserID = user.Id
                });
            }

            return users;
        }

        public bool AddErrorReport(ErrorReport ErrorReport)
        {
            using (TransactionScope sc = new TransactionScope())
            {
                var existingReport = db.ErrorReport.Where(er => er.Role1Id == ErrorReport.Role1Id && er.Role2Id == ErrorReport.Role2Id && er.TanId == er.TanId).FirstOrDefault();
                if (existingReport != null)
                {
                    existingReport.AddedComments = ErrorReport.AddedComments;
                    existingReport.AddedReactions = ErrorReport.AddedReactions;
                    existingReport.AddedStages = ErrorReport.AddedStages;
                    existingReport.AddedRsns = ErrorReport.AddedRsns;
                    existingReport.AddedProducts = ErrorReport.AddedProducts;
                    existingReport.AddedReactants = ErrorReport.AddedReactants;
                    existingReport.AddedSolvents = ErrorReport.AddedSolvents;
                    existingReport.AddedAgents = ErrorReport.AddedAgents;
                    existingReport.AddedCatalysts = ErrorReport.AddedCatalysts;
                    existingReport.AddedTemperature = ErrorReport.AddedTemperature;
                    existingReport.AddedPressure = ErrorReport.AddedPressure;
                    existingReport.AddedpH = ErrorReport.AddedpH;
                    existingReport.AddedTime = ErrorReport.AddedTime;

                    existingReport.DeletedComments = ErrorReport.DeletedComments;
                    existingReport.DeletedReactions = ErrorReport.DeletedReactions;
                    existingReport.DeletedProducts = ErrorReport.DeletedProducts;
                    existingReport.DeletedReactants = ErrorReport.DeletedReactants;
                    existingReport.DeletedSolvents = ErrorReport.DeletedSolvents;
                    existingReport.DeletedAgents = ErrorReport.DeletedAgents;
                    existingReport.DeletedCatalysts = ErrorReport.DeletedCatalysts;
                    existingReport.DeletedStages = ErrorReport.DeletedStages;
                    existingReport.DeletedRsns = ErrorReport.DeletedRsns;
                    existingReport.DeletedTemperature = ErrorReport.DeletedTemperature;
                    existingReport.DeletedPressure = ErrorReport.DeletedPressure;
                    existingReport.DeletedpH = ErrorReport.DeletedpH;
                    existingReport.DeletedTime = ErrorReport.DeletedTime;

                    existingReport.UpdatedComments = ErrorReport.UpdatedComments;
                    existingReport.UpdatedProducts = ErrorReport.UpdatedProducts;
                    existingReport.UpdatedReactants = ErrorReport.UpdatedReactants;
                    existingReport.UpdatedSolvents = ErrorReport.UpdatedSolvents;
                    existingReport.UpdatedAgents = ErrorReport.UpdatedAgents;
                    existingReport.UpdatedCatalysts = ErrorReport.UpdatedCatalysts;
                    existingReport.UpdatedRsns = ErrorReport.UpdatedRsns;
                    existingReport.UpdatedTemperature = ErrorReport.UpdatedTemperature;
                    existingReport.UpdatedPressure = ErrorReport.UpdatedPressure;
                    existingReport.UpdatedpH = ErrorReport.UpdatedpH;
                    existingReport.UpdatedTime = ErrorReport.UpdatedTime;

                    existingReport.CommonComments = ErrorReport.CommonComments;
                    existingReport.CommonReactions = ErrorReport.CommonReactions;
                    existingReport.CommonProducts = ErrorReport.CommonProducts;
                    existingReport.CommonReactants = ErrorReport.CommonReactants;
                    existingReport.CommonSolvents = ErrorReport.CommonSolvents;
                    existingReport.CommonAgents = ErrorReport.CommonAgents;
                    existingReport.CommonCatalysts = ErrorReport.CommonCatalysts;
                    existingReport.CommonTemperature = ErrorReport.CommonTemperature;
                    existingReport.CommonPressure = ErrorReport.CommonPressure;
                    existingReport.CommonpH = ErrorReport.CommonpH;
                    existingReport.CommonTime = ErrorReport.CommonTime;
                    existingReport.CommonStages = ErrorReport.CommonStages;
                    existingReport.CommonRsns = ErrorReport.CommonRsns;
                }
                else
                {
                    var User1 = db.Users.Where(u => u.UserName == ErrorReport.Role1Name).FirstOrDefault();
                    var User2 = db.Users.Where(u => u.UserName == ErrorReport.Role2Name).FirstOrDefault();
                    var Tan = db.Tans.Where(t => t.tanNumber == ErrorReport.TanNumber).FirstOrDefault();
                    ErrorReport.Tan = Tan;
                    ErrorReport.Role1 = User1;
                    ErrorReport.Role2 = User2;
                    db.ErrorReport.Add(ErrorReport);
                }
                db.SaveChanges();
                sc.Complete();
                return true;
            }
        }

        public ArrayList GetTodayHourWiseCount()
        {
            ArrayList pastFewDaysCount = new ArrayList();
            int[] time = new int[2];
            var todayData = db.TanActionHistories.Where(t => t.UpdatedDate.Day == DateTime.Now.Day && t.TanAction == TanAction.TAN_SUBMITTED).ToList();
            var list = todayData.GroupBy(t => t.UpdatedDate.Hour).ToDictionary(t=>t.Key,t=>t.Count());
            for (int i = 1; i < 25; i++)
            {
                time[0] = i;
                time[1] = list.ContainsKey(i) ? list[i] : 0 ;
                pastFewDaysCount.Add(time);
                time = new int[2];
            }
            return pastFewDaysCount;
        }
    }
}