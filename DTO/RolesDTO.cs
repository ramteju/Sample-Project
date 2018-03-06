using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RolesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        override public String ToString()
        {
            return Name;
        }
    }
}
