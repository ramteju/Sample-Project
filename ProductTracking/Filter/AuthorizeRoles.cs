using Entities;
using Excelra.Utils.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProductTracking.Filter
{
    public class AuthorizeRoles : AuthorizeAttribute
    {
        private Role[] roles;
        public AuthorizeRoles(params Role[] roles)
        {
            this.roles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            foreach (Role role in roles)
            {
                if (httpContext.User.IsInRole(role.DescriptionAttribute()))
                    return true;
            }
            HttpContext.Current.GetOwinContext().Authentication.SignOut();
            return false;
        }
    }
}