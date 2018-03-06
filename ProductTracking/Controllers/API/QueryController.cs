using Entities.DTO;
using Microsoft.Practices.Unity;
using ProductTracking.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace ProductTracking.Controllers.API
{
    [Authorize]
    public class QueryController : ApiController
    {
        [Dependency("QueryService")]
        public QueryService queryService { get; set; }
        [Dependency("ClaimService")]
        public ClaimService claimServices { get; set; }

        [HttpGet]
        public object UserQueries(bool allUsers)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return queryService.UserQueries(id, allUsers);
        }
        public object Save(QueryDTO queryDto)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return queryService.Save(queryDto, id);
        }
        public object Submit(int queryId, string msg)
        {
            var userId = claimServices.UserId((ClaimsIdentity)User.Identity);
            return queryService.Submit(queryId, userId, msg);
        }
        public object Revert(int queryId, string msg)
        {
            var userId = claimServices.UserId((ClaimsIdentity)User.Identity);
            return queryService.Revert(queryId, userId, msg);
        }//GetQueryStatus

        public bool IsQueryActive(int tanID)
        {
            var userId = claimServices.UserId((ClaimsIdentity)User.Identity);
            return queryService.IsQueryActive(tanID, userId);
        }//GetQueryStatus
        public object Responses(int id)
        {
            var userId = claimServices.UserId((ClaimsIdentity)User.Identity);
            return queryService.Responses(id);
        }
        public object Workflow(int id)
        {
            var userId = claimServices.UserId((ClaimsIdentity)User.Identity);
            return queryService.Workflow(id, userId);
        }
        public object Users()
        {
            return queryService.Users();
        }
        public object Workflows()
        {
            return queryService.Workflows();
        }
        public object SaveWorkflows(List<QueryWorkflowUserDTO> dtos)
        {
            return queryService.SaveWorkflows(dtos);
        }
        public object QueriesReport(QueryReportRequestDTO dto)
        {
            return queryService.QueriesReport(dto.From, dto.To);
        }
    }
}
