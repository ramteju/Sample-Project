using Microsoft.Practices.Unity;
using ProductTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace ProductTracking.Services.Core
{
    public class ClaimService
    {
        [Dependency("UserService")]
        public UserService userService { get; set; }

        private ApplicationDbContext db = new ApplicationDbContext();

        public string UserId(ClaimsIdentity claimsIdentity)
        {
            return (from c in claimsIdentity.Claims where c.Type == ClaimTypes.WindowsAccountName select c).FirstOrDefault()?.Value;
        }
    }
}