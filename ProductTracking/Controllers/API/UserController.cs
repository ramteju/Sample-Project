using Microsoft.Practices.Unity;
using ProductTracking.Services.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ProductTracking.Controllers.API
{
    [Authorize]
    public class UserController : ApiController
    {
        // GET: User
        [Dependency("UserService")]
        public UserService userService { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public object GetAllCurators()
        {
            return userService.GetAllCurators();
        }
    }
}