using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class GenerateXMLDTO
    {
        public bool IsSuccess { get; set; }
        public String OutXmlPath { get; set; }
        public String StackTrace { get; set; }
    }
}
