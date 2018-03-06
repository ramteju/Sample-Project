using Client.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Deployment.Application;
using System.Reflection;

namespace Client.Common
{
    //Short notation for user store
    public static class U
    {
        public static int RoleId { get; set; }
        public static string UserName { get; set; }
        public static string UserId { get; set; }
        public static string UserRole { get; set; }
        public static bool CanSubmit { get; set; }
        public static bool CanApprove { get; set; }
        public static bool CanReject { get; set; }
        public static int LastSelectedTab { get; set; }
        public static int TargetRole { get; set; }
    }
}
