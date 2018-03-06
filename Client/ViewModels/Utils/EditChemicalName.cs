using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels
{
    public class EditChemicalName
    {
        public TanChemicalVM ChemicalNameVM { get; set; }
        public IEnumerable<TanChemicalVM> ChoosableChemicalVMs { get; set; }
        
    }
}
