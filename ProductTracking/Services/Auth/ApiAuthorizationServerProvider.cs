using Microsoft.Owin.Security.OAuth;
using ProductTracking.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Web.Security;
using ProductTracking.Services.Core;
using Entities;
using ProductTracking.Logging;
using ProductTracking.AppClient;

namespace ProductTracking.Services.Auth
{
    public class ApiAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        UserService userService = new UserService();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            try
            {
                context.Validated();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });
                using (var db = new ApplicationDbContext())
                {
                    ApplicationUser user = null;
                    if(!string.IsNullOrEmpty(context.UserName) && context.UserName.Contains("----"))
                    {
                        var data = context.UserName.Split(new string[] { "----" }, StringSplitOptions.RemoveEmptyEntries);
                        AppAuthenticationRequest request = new AppClient.AppAuthenticationRequest();
                        request.Application = "NGP-R";
                        request.UserName = data[0];
                        request.SystemLoginUser = data[1];
                        request.SystemID = data[2];
                        request.IP = data[3];
                        request.ApplicationVersion = data[4];
                        request.PWD = context.Password;
                        AppASClient client = new AppASClient();
                        var LDAPResult = client.AuthenticateUser(request);
                        if (!LDAPResult.Acknowledgement.IsError)
                        {
                            user = userService.SyncLdapUser(data[0], context.Password, UserManager);
                            if (user == null)
                            {
                                context.SetError("invalid", "The user name or password is incorrect.");
                                return;
                            }
                        }
                        else
                        {
                            context.SetError("invalid", LDAPResult.Acknowledgement.Description);
                            return;
                        }
                        var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                        identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                        identity.AddClaim(new Claim(ClaimTypes.WindowsAccountName, user.Id));
                        var usr = (from u in db.Users where u.UserName.Equals(user.UserName) select u).FirstOrDefault();
                        if (usr.AnalystId == null)
                        {
                            usr.AnalystId = 0;
                            usr.BenchMark = 0;
                        }
                        var userLogin = new Models.Core.UserLogin { ApplicationUser = usr, Ip = context.OwinContext.Request.RemoteIpAddress };
                        db.UserLogins.Add(userLogin);
                        db.Entry(userLogin).State = System.Data.Entity.EntityState.Added;
                        db.SaveChanges();
                        context.Validated(identity);
                        context.Response.Headers.Add("UserId", new string[] { user.Id });
                    }
                    else
                    {
                        context.SetError("invalid", "Some fields Missed in Request");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}