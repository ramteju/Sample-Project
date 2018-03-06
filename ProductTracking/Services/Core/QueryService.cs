using Entities;
using Entities.DTO;
using Microsoft.AspNet.SignalR;
using ProductTracking.Hubs;
using ProductTracking.Models;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.IO;
using System.Web;
using ProductTracking.Store;

namespace ProductTracking.Services.Core
{
    public class QueryService
    {
        public MyQueriesDTO UserQueries(string userId, bool allUsers)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var queryWorkflowUserIds = db.QueryWorkflowUsers.Where(qwu => !allUsers ? qwu.UserId == userId : true).Select(qwu => qwu.Id).ToList();
                var dbQueries = db.Queries.Where(q => queryWorkflowUserIds.Contains(q.QueryWorkFlowUserId)).OrderByDescending(q => q.Updated);
                var tanIds = dbQueries.Select(q => q.TanId).ToList();
                Dictionary<int, Tan> idWiseTans = db.Tans.Where(t => tanIds.Contains(t.Id)).GroupBy(t => t.Id).ToDictionary(d => d.Key, d => d.First());
                List<QueryDTO> queries = new List<QueryDTO>();
                foreach (var q in dbQueries)
                {
                    if (idWiseTans.ContainsKey(q.TanId))
                        queries.Add(new QueryDTO
                        {
                            Id = q.Id,
                            Title = q.Title,
                            Comment = q.Comment,
                            Page = q.Page,
                            QueryType = q.QueryType,
                            TanId = q.TanId,
                            TanNumber = idWiseTans[q.TanId].tanNumber,
                            DocumentPath = Path.Combine(C.ShipmentSharedPath, idWiseTans[q.TanId].DocumentPath)
                        });
                }
                MyQueriesDTO dto = new MyQueriesDTO();
                dto.Queries = queries;
                return dto;
            }
        }
        public object Save(QueryDTO queryDto, string userId)
        {
            using (TransactionScope scope = new TransactionScope())
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var username = db.Users.Find(userId).UserName;
                if (queryDto.Id == 0)
                {
                    int? queryWorkflowUserId = null;
                    Tan tan = db.Tans
                        .Include(t => t.Curator)
                        .Include(t => t.Reviewer)
                        .Include(t => t.QC)
                        .Include(t => t.CurrentUserRole)
                        .Where(t => t.Id == queryDto.TanId)
                        .FirstOrDefault();
                    if (tan != null && !String.IsNullOrEmpty(tan.Curator.Id))
                    {
                        //Get query workflow starting with the current user
                        queryWorkflowUserId = db.QueryWorkflowUsers.Where(qwu => qwu.Role == QueryRole.L1 && qwu.UserId == userId).Select(qwu => qwu.Id).FirstOrDefault();
                    }
                    if (queryWorkflowUserId != null && queryWorkflowUserId > 0)
                    {
                        Query query = new Query
                        {
                            Comment = queryDto.Comment,
                            Page = queryDto.Page,
                            QueryType = queryDto.QueryType,
                            TanId = queryDto.TanId,
                            Title = queryDto.Title,
                            PostedById = userId,
                            Posted = DateTime.Now,
                            QueryWorkFlowUserId = queryWorkflowUserId.Value
                        };
                        db.Queries.Add(query);
                        db.SaveChanges();
                    }
                    else
                        return $"No Workflow Found In Query Module Starting With {username} . .";
                }
                else
                {
                    Query query = db.Queries.Find(queryDto.Id);
                    if (query != null)
                    {
                        query.Comment = queryDto.Comment;
                        query.Page = queryDto.Page;
                        query.QueryType = queryDto.QueryType;
                        query.TanId = queryDto.TanId;
                        query.Title = queryDto.Title;
                        query.UpdatedById = userId;
                        query.Updated = DateTime.Now;
                        if (query.QueryType == QueryType.Query && query.PostedById != userId)
                        {
                            Tan tan = db.Tans.Include(nameof(Tan.Batch)).Where(t => t.Id == query.TanId).FirstOrDefault();
                            if (tan != null)
                                tan.MarkedAsQuery = true;
                        }
                        db.SaveChanges();
                    }
                }
                scope.Complete();
                return "Query Saved Successfully";
            }
        }
        public object Submit(int queryId, string userId, string msg)
        {
            using (TransactionScope scope = new TransactionScope())
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Query query = db.Queries.Find(queryId);
                var user = db.Users.Find(userId);
                if (query != null)
                {

                    query.Responses.Add(new QueryResponse
                    {
                        QueryId = query.Id,
                        Response = msg,
                        Timestamp = DateTime.Now,
                        UserId = userId
                    });

                    QueryWorkflowUser nextUser = NextUser(db, (int)query.QueryWorkFlowUser.Role, query.QueryWorkFlowUser.WorkflowId);

                    //Submit to curator in case of tool manager.
                    if (nextUser == null && query.QueryWorkFlowUser.Role == QueryRole.L5)
                    {
                        nextUser = Curator(db, query.QueryWorkFlowUser.WorkflowId);
                    }

                    if (nextUser != null)
                    {
                        query.QueryWorkFlowUserId = nextUser.Id;
                        Tan tan = db.Tans.Include(nameof(Tan.Batch)).Where(t => t.Id == query.TanId).FirstOrDefault();
                        db.SaveChanges();
                        scope.Complete();
                        try
                        {
                            var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
                            List<string> connectionIds = LiveHub.UserConnectionIds[nextUser.User.UserName];
                            foreach (var connectionId in connectionIds)
                                context.Clients.Client(connectionId).notification($"{user.UserName} Sent a query to you . .");
                        }
                        catch (Exception ex)
                        {

                        }
                        return $"Query Submitted To {nextUser.User.UserName}";
                    }
                }
            }
            return "Query Submission Failed";
        }
        public bool IsQueryActive(int tanID, string userID)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var data = db.Queries.Include(nameof(Query.QueryWorkFlowUser)).Where(q => q.TanId == tanID && q.PostedById == userID && q.QueryWorkFlowUser.UserId != userID).Any();
                return data;
            }
        }
        public object Revert(int queryId, string userId, string msg)
        {
            using (TransactionScope scope = new TransactionScope())
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Query query = db.Queries.Find(queryId);
                var user = db.Users.Find(userId);
                if (query != null)
                {
                    query.Responses.Add(new QueryResponse
                    {
                        QueryId = query.Id,
                        Response = msg,
                        Timestamp = DateTime.Now,
                        UserId = userId
                    });

                    var previousUser = PreviousUser(db, (int)query.QueryWorkFlowUser.Role, query.QueryWorkFlowUser.WorkflowId);
                    if (previousUser != null)
                    {
                        query.QueryWorkFlowUserId = previousUser.Id;
                        Tan tan = db.Tans.Where(t => t.Id == query.TanId).FirstOrDefault();
                        db.SaveChanges();
                        scope.Complete();
                        try
                        {
                            var context = GlobalHost.ConnectionManager.GetHubContext<LiveHub>();
                            List<string> connectionIds = LiveHub.UserConnectionIds[previousUser.User.UserName];
                            foreach (var connectionId in connectionIds)
                                context.Clients.Client(connectionId).notification($"{user.UserName} Sent a query to you . .");
                        }
                        catch (Exception ex)
                        {

                        }
                        return $"Query Reverted To {previousUser.User.UserName}";
                    }
                }
            }
            return "Query Reverting Failed  . .";
        }

        public object Responses(int id)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var responses = db.QueryResponses.Include(nameof(QueryResponse.User)).Where(r => r.QueryId == id).OrderByDescending(r => r.Timestamp);
                if (responses != null)
                {
                    List<QueryResponseDTO> dtos = new List<QueryResponseDTO>();
                    foreach (var response in responses)
                        dtos.Add(new QueryResponseDTO
                        {
                            Id = response.Id,
                            Response = response.Response,
                            TimeStamp = response.Timestamp,
                            User = response.User.UserName
                        });
                    return dtos.ToList();
                }
            }
            return String.Empty;
        }
        public object Workflow(int id, string userId)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var queryWorkflowUser = db.Queries.Include(nameof(QueryWorkflowUser.User)).Where(q => q.Id == id).Select(q => q.QueryWorkFlowUser).FirstOrDefault();
                var user = db.Users.Find(queryWorkflowUser.UserId);
                QueryWorkflowDTO dto = new QueryWorkflowDTO();

                var nextUser = NextUser(db, (int)queryWorkflowUser.Role, queryWorkflowUser.WorkflowId);
                if (nextUser != null)
                {
                    dto.AllowSubmit = true;
                    dto.NextUser = nextUser.User.UserName;
                }
                if (nextUser == null && queryWorkflowUser.Role == QueryRole.L5)
                {
                    dto.AllowSubmit = true;
                    dto.NextUser = Curator(db, queryWorkflowUser.WorkflowId)?.User?.UserName;
                }

                var previousUser = PreviousUser(db, (int)queryWorkflowUser.Role, queryWorkflowUser.WorkflowId);
                if (previousUser != null)
                {
                    dto.AllowReject = true;
                    dto.PreviousUser = previousUser.User.UserName;
                }

                dto.CurrentUser = user.UserName;
                return dto;
            }
        }

        public QueryWorkflowUser NextUser(ApplicationDbContext db, int role, int workflowId)
        {
            return db.QueryWorkflowUsers.Include(nameof(QueryWorkflowUser.User)).Where(qwu => qwu.WorkflowId == workflowId && (int)qwu.Role > role).OrderBy(qwr => qwr.Role).FirstOrDefault();
        }
        public QueryWorkflowUser PreviousUser(ApplicationDbContext db, int role, int workflowId)
        {
            return db.QueryWorkflowUsers.Include(nameof(QueryWorkflowUser.User)).Where(qwu => qwu.WorkflowId == workflowId && (int)qwu.Role < role).OrderByDescending(qwr => qwr.Role).FirstOrDefault();
        }
        public QueryWorkflowUser Curator(ApplicationDbContext db, int workflowId)
        {
            return db.QueryWorkflowUsers.Include(nameof(QueryWorkflowUser.User)).Where(qwu => qwu.WorkflowId == workflowId && (int)qwu.Role == (int)QueryRole.L1).FirstOrDefault();
        }
        public object Users()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return db.Users.Select(u => u.UserName).OrderBy(u => u).ToList();
            }
        }
        public object Workflows()
        {
            Dictionary<int, QueryWorkflowUserDTO> workflowWiseUsers = new Dictionary<int, QueryWorkflowUserDTO>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var queryWorkflowUsers = db.QueryWorkflowUsers.Include(nameof(QueryWorkflowUser.User));
                foreach (var qwu in queryWorkflowUsers)
                {
                    var queryWorkflowUserDTO = workflowWiseUsers.ContainsKey(qwu.WorkflowId) ? workflowWiseUsers[qwu.WorkflowId] : new QueryWorkflowUserDTO();
                    if (qwu.Role == QueryRole.L1)
                        queryWorkflowUserDTO.L1User = qwu.User.UserName;
                    else if (qwu.Role == QueryRole.L2)
                        queryWorkflowUserDTO.L2User = qwu.User.UserName;
                    else if (qwu.Role == QueryRole.L3)
                        queryWorkflowUserDTO.L3User = qwu.User.UserName;
                    else if (qwu.Role == QueryRole.L4)
                        queryWorkflowUserDTO.L4User = qwu.User.UserName;
                    else if (qwu.Role == QueryRole.L5)
                        queryWorkflowUserDTO.L5User = qwu.User.UserName;
                    workflowWiseUsers[qwu.WorkflowId] = queryWorkflowUserDTO;
                }
            }
            return workflowWiseUsers.Values.ToList();
        }

        public object SaveWorkflows(List<QueryWorkflowUserDTO> dtos)
        {
            using (TransactionScope scope = new TransactionScope())
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var queryWorkflowUsers = db.QueryWorkflowUsers.Include(nameof(QueryWorkflowUser.User)).ToList();
                var userWiseWorkflowIds = queryWorkflowUsers
                    .Where(qwu => qwu.Role == QueryRole.L1)
                    .GroupBy(qwu => qwu.User.UserName)
                    .ToDictionary(d => d.Key, d => d.First().WorkflowId);
                var usernameWiseIds = db.Users.GroupBy(u => u.UserName).ToDictionary(d => d.Key, d => d.First().Id);
                foreach (var dto in dtos)
                {
                    //If workflow exists previously
                    if (userWiseWorkflowIds.ContainsKey(dto.L1User))
                    {
                        var workflowId = userWiseWorkflowIds[dto.L1User];
                        if (!String.IsNullOrEmpty(dto.L1User))
                        {
                            var L1user = queryWorkflowUsers.Where(qwu => qwu.WorkflowId == workflowId && qwu.Role == QueryRole.L1).FirstOrDefault();
                            if (L1user != null)
                                L1user.UserId = usernameWiseIds[dto.L1User];
                            else
                                db.QueryWorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L1, UserId = usernameWiseIds[dto.L1User], WorkflowId = workflowId });
                        }
                        if (!String.IsNullOrEmpty(dto.L2User))
                        {
                            var L2user = queryWorkflowUsers.Where(qwu => qwu.WorkflowId == workflowId && qwu.Role == QueryRole.L2).FirstOrDefault();
                            if (L2user != null)
                                L2user.UserId = usernameWiseIds[dto.L2User];
                            else
                                db.QueryWorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L2, UserId = usernameWiseIds[dto.L2User], WorkflowId = workflowId });
                        }
                        if (!String.IsNullOrEmpty(dto.L3User))
                        {
                            var L3user = queryWorkflowUsers.Where(qwu => qwu.WorkflowId == workflowId && qwu.Role == QueryRole.L3).FirstOrDefault();
                            if (L3user != null)
                                L3user.UserId = usernameWiseIds[dto.L3User];
                            else
                                db.QueryWorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L3, UserId = usernameWiseIds[dto.L3User], WorkflowId = workflowId });
                        }
                        if (!String.IsNullOrEmpty(dto.L4User))
                        {
                            var L4user = queryWorkflowUsers.Where(qwu => qwu.WorkflowId == workflowId && qwu.Role == QueryRole.L4).FirstOrDefault();
                            if (L4user != null)
                                L4user.UserId = usernameWiseIds[dto.L4User];
                            else
                                db.QueryWorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L4, UserId = usernameWiseIds[dto.L4User], WorkflowId = workflowId });
                        }
                        if (!String.IsNullOrEmpty(dto.L5User))
                        {
                            var L5user = queryWorkflowUsers.Where(qwu => qwu.WorkflowId == workflowId && qwu.Role == QueryRole.L5).FirstOrDefault();
                            if (L5user != null)
                                L5user.UserId = usernameWiseIds[dto.L5User];
                            else
                                db.QueryWorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L5, UserId = usernameWiseIds[dto.L5User], WorkflowId = workflowId });
                        }
                    }
                    //Create new workflow
                    else if (dto.HasValidData)
                    {
                        QueryWorkflow workflow = new QueryWorkflow();
                        workflow.Name = dto.L1User;
                        workflow.WorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L1, UserId = usernameWiseIds[dto.L1User], Workflow = workflow });
                        if (!String.IsNullOrEmpty(dto.L2User))
                            workflow.WorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L2, UserId = usernameWiseIds[dto.L2User], Workflow = workflow });
                        if (!String.IsNullOrEmpty(dto.L3User))
                            workflow.WorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L3, UserId = usernameWiseIds[dto.L3User], Workflow = workflow });
                        if (!String.IsNullOrEmpty(dto.L4User))
                            workflow.WorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L4, UserId = usernameWiseIds[dto.L4User], Workflow = workflow });
                        if (!String.IsNullOrEmpty(dto.L5User))
                            workflow.WorkflowUsers.Add(new QueryWorkflowUser { Role = QueryRole.L5, UserId = usernameWiseIds[dto.L5User], Workflow = workflow });
                        db.QueryWorkflows.Add(workflow);
                    }
                }
                db.SaveChanges();
                scope.Complete();
            }
            return "Workflows Saved Successfully";
        }

        public object QueriesReport(DateTime from, DateTime to)
        {
            List<QueryReportEntryDTO> dtos = new List<QueryReportEntryDTO>();
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var userWiseCreatedCount = db.Queries.Include(nameof(Query.PostedBy)).Where(q => DbFunctions.TruncateTime(q.Posted) >= DbFunctions.TruncateTime(from)
                 && DbFunctions.TruncateTime(q.Posted) <= DbFunctions.TruncateTime(to)
                )
                .GroupBy(q => q.PostedBy.UserName)
                .ToDictionary(d => d.Key, d => d.Count());

                var userWiseRespondedCount = db.QueryResponses.Include(nameof(QueryResponse.User)).Where(q => DbFunctions.TruncateTime(q.Timestamp) >= DbFunctions.TruncateTime(from)
                 && DbFunctions.TruncateTime(q.Timestamp) <= DbFunctions.TruncateTime(to)
                )
                .GroupBy(q => q.User.UserName)
                .ToDictionary(d => d.Key, d => d.Count());

                var allUserNames = new List<string>(userWiseCreatedCount.Keys);
                allUserNames.AddRange(userWiseRespondedCount.Keys);
                allUserNames = allUserNames.Distinct().ToList();

                foreach (var username in allUserNames)
                {
                    QueryReportEntryDTO dto = new QueryReportEntryDTO();
                    dto.User = username;
                    dto.Created = userWiseCreatedCount.ContainsKey(username) ? userWiseCreatedCount[username] : 0;
                    dto.Responded = userWiseRespondedCount.ContainsKey(username) ? userWiseRespondedCount[username] : 0;
                    dtos.Add(dto);
                }

            }
            return dtos;
        }
    }
}