using DTO;
using Entities;
using Entities.DTO;
using Entities.DTO.Core;
using Microsoft.Practices.Unity;
using ProductTracking.Models.Core;
using ProductTracking.Services.Core;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Web.Http;
namespace ProductTracking.Controllers.API
{
    [Authorize]
    public class TanController : ApiController
    {

        static string ip = System.Web.HttpContext.Current != null ? System.Web.HttpContext.Current.Request.UserHostAddress : String.Empty;
        [Dependency("ClaimService")]
        public ClaimService claimServices { get; set; }
        [Dependency("Tanservice")]
        public TanService TanService { get; set; }
        [Dependency("Userservice")]
        public UserService Userservice { get; set; }


        [HttpGet]
        public List<TaskDTO> Mytans([FromUri]int currentRole, [FromUri]bool pullTasks)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.MyTans(id, currentRole, pullTasks);
        }

        [HttpPost]
        public object Assigntans(List<int> tanIds, [FromUri]string toUserId, [FromUri]int currentRoleId, [FromUri]string comment, [FromUri]Role role)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.AssignTans(tanIds, toUserId, currentRoleId, id, comment, role);
        }

        [HttpGet]
        public object Pulltask([FromUri]int currentRole, [FromUri]bool pullTasks)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.PullTask(id, currentRole, pullTasks);
        }

        [HttpGet]
        public object PullTaskWithGrouping([FromUri]int currentRole, [FromUri]bool pullTasks)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.PullTask(id, currentRole, pullTasks);
        }

        [HttpGet]
        public object GetTan([FromUri]int tanId, [FromUri]int currentRole)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.GetTan(tanId, id, currentRole);
        }

        [HttpGet]
        public object GetRoleBasedTanData([FromUri]int tanId, [FromUri]int currentRole)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.GetRoleBasedTanData(tanId, id, currentRole);
        }

        [HttpGet]
        public object UserReactionsReports([FromUri]int currentRoleId)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.UserReactionsReports(id, currentRoleId);
        }

        [HttpPost]
        public string SaveTan(OfflineDTO offlineDTO, [FromUri]int currentUserRole, [FromUri]string UserName)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.SaveTan(id, offlineDTO, currentUserRole, ip, UserName);
        }
        [AllowAnonymous]
        [HttpPost]
        public object SaveException(ActivityTracing ErrorInfo)
        {
            return TanService.SaveException(ErrorInfo);
        }

        [AllowAnonymous]
        [HttpGet]
        public List<RolesDTO> GetUserRoles([FromUri]string UserName)
        {
            return TanService.UserRoles(UserName);
        }

        [AllowAnonymous]
        [HttpGet]
        public List<RegulerExpression> GetRegulerExpressions()
        {
            return TanService.GetRegulerExpressions();
        }


        public UserInfoDTO GetUserInfo(int currentRole)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.GetUserInfo(id, currentRole);
        }

        [HttpPost]
        public object AproveTan(int tanId, int currentRole, [Optional] bool QCRequired)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.ApproveTan(tanId, currentRole, id, QCRequired);
        }

        [HttpPost]
        public object RejectTan(int tanId, int currentRole)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.RejectTan(tanId, currentRole, id);

        }
        [HttpPost]
        public object SubmitTan(int tanId, int currentRole)
        {
            var id = claimServices.UserId((ClaimsIdentity)User.Identity);
            return TanService.SubmitTan(tanId, currentRole, id);
        }
        public object Versions(string tanNumber)
        {
            return TanService.GetVersions(tanNumber);
        }

        [HttpPost]
        public object GetErrorReportData([FromBody]ErrorReportDto ErrorReport)
        {
            return TanService.GetErrorReport(ErrorReport);
        }

        [HttpPost]
        public object AddErrorReport([FromBody]ErrorReport ErrorReport)
        {
            return Userservice.AddErrorReport(ErrorReport);
        }
    }
}