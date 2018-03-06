using Entities;
using ProductTracking.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web;

namespace ProductTracking.Services.Auth
{
    public class LdapService
    {
        private static readonly string LDAP_DOMAIN = ConfigurationManager.AppSettings["LDAP_DOMAIN"];
        private static readonly string LDAP_SEARCH_PATH = ConfigurationManager.AppSettings["LDAP_SEARCH_PATH"];

        public static bool IsUserValid(String userName, String password)
        {
            try
            {
                bool isValid = false;
                try
                {
                    using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, LDAP_DOMAIN))
                        isValid = pc.ValidateCredentials(userName, password);
                }
                catch (Exception ex)
                {
                    isValid = false;
                    Log.Error(ex);
                }
                return isValid;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

    }
}