using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class ErrorPercentageDto
    {

        public DateTime Date { get; set; }
        public int RoleID { get; set; }
        /// <summary>
        /// Serialse to Tan class
        /// </summary>
        public List<string> FirstRoleTanData { get; set; }
        /// <summary>
        /// Serialse to Tan class
        /// </summary>
        public List<string> SecondRoleTanData { get; set; }

    }
}
