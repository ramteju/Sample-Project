using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class UnicodeErrorVM : ViewModelBase
    {
        private string tanNumber, comments, level, substanceName;
        private int rxn, s8000,  seq;
        private string num;

        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public string Comments { get { return comments; } set { SetProperty(ref comments, value); } }
        public string Level { get { return level; } set { SetProperty(ref level, value); } }
        public string SubstanceName { get { return substanceName; } set { SetProperty(ref substanceName, value); } }
        public int RXN { get { return rxn; } set { SetProperty(ref rxn, value); } }
        public int Seq { get { return seq; } set { SetProperty(ref seq, value); } }
        public int S8000 { get { return s8000; } set { SetProperty(ref s8000, value); } }
        public string Num { get { return num; } set { SetProperty(ref num, value); } }
    }
}
