using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class S8000NameLocationVM : ViewModelBase
    {
        private string tanNumber,  substanceName, substanceLocation, substanceOtherName, substanceAuthorName, tanCategory;
        private int tanSeries;

        public string TanNumber { get { return tanNumber; } set { SetProperty(ref tanNumber, value); } }
        public string TanCategory { get { return tanCategory; } set { SetProperty(ref tanCategory, value); } }
        public int TanSeries { get { return tanSeries; } set { SetProperty(ref tanSeries, value); } }
        public string SubstanceName { get { return substanceName; } set { SetProperty(ref substanceName, value); } }
        public string SubstanceLocation { get { return substanceLocation; } set { SetProperty(ref substanceLocation, value); } }
        public string SubstanceOtherName { get { return substanceOtherName; } set { SetProperty(ref substanceOtherName, value); } }
        public string SubstanceAuthorName { get { return substanceAuthorName; } set { SetProperty(ref substanceAuthorName, value); } }
    }
}
