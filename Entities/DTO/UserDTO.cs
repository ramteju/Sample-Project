using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class UserDTO
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public Role Role { get; set; }

    }

    public class UserReportsDTO
    {
        public int ReactionsCount { get; set; }
        public string Date { get; set; }
        public bool SingleUser { get; set; }
    }
}
