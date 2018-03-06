using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class UserInfoDTO
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<string> RoleNames { get; set; }
        public bool HasSubOrdinates { get; set; }
        public string SuperiorName { get; set; }
        public bool canSubmit { get; set; }
        public bool canReject { get; set; }
        public bool canApprove { get; set; }
    }

    public class RoleInfo
    {
        public bool canWork { get; set; }
        public bool canSubmit { get; set; }
        public bool canReject { get; set; }
        public bool canAccept { get; set; }
        public bool ShowShipments { get; set; }

    }
}
