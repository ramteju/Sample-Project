using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class ErrorReportDto
    {
        public int Id { get; set; }
        public int TanID { get; set; }
        public string TanNumber { get; set; }
        public string FirstUserName { get; set; }
        public string SecondUserName { get; set; }
        public string FirstRoleUserId { get; set; }
        public int FirstRoleId { get; set; }
        public string SecondRoleUserId { get; set; }
        public int SecondRoleId { get; set; }
        /// <summary>
        /// Serialse to Tan class
        /// </summary>
        public string FirstRoleTanData { get; set; }
        /// <summary>
        /// Serialse to Tan class
        /// </summary>
        public string SecondRoleTanData { get; set; }
    }
}
