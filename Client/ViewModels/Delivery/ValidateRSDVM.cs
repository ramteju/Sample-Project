using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class ValidateRSDVM : ViewModelBase
    {
        private string tanNumber, rsd;
        private int rxnNo, semicolumns, stages, rsdLength, productNo, sequence;
        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public string RSD { get { return rsd; } set { SetProperty(ref rsd, value); } }
        public int RxnNo { get { return rxnNo; } set { SetProperty(ref rxnNo, value); } }
        public int ProductNo { get { return productNo; } set { SetProperty(ref productNo, value); } }
        public int Sequence { get { return sequence; } set { SetProperty(ref sequence, value); } }
        public int Semicolumns { get { return semicolumns; } set { SetProperty(ref semicolumns, value); } }
        public int Stages { get { return stages; } set { SetProperty(ref stages, value); } }
        public int RsdLength { get { return rsdLength; } set { SetProperty(ref rsdLength, value); } }
    }
}
