using Entities;
using Microsoft.AspNet.Identity;
using ProductTracking.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductTracking.Models
{
    public class AppPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            try
            {
                return hashedPassword == providedPassword ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}