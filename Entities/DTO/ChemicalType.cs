using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public enum ChemicalType
    {
        NUM = 1,
        [Description("9000 Series")]
        S9000 = 2,
        [Description("8500 Series")]
        S8500 = 3,
        [Description("8000 Series")]
        S8000 = 4
    }
}
