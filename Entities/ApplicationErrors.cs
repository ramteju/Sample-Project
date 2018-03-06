using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ActivityTracing
    {
        public ActivityTracing()
        {
            FromTime = DateTime.Now;
        }

        public int Id { get; set; }
        public string MethodName { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string ErrorMessage { get; set; }
        public string User { get; set; }
        public int tanId { get; set; }
        public String Info { get; set; }
        public bool FromTool { get; set; }

    }
}
