using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Client.ViewModel;

namespace Client.ViewModels.Extended
{
    public class SolventBoilingPointsVM : ViewModelBase
    {
        public SolventBoilingPointsVM()
        {
            BoilingPoints = new ObservableCollection<BoilingPointsVM>();
        }
        private ObservableCollection<BoilingPointsVM> boilingPoints;
        public ObservableCollection<BoilingPointsVM> BoilingPoints { get { return boilingPoints; } set { SetProperty(ref boilingPoints, value); } }
    }
    public class BoilingPointsVM : OrderableVM
    {
        private string regNo;
        private string name;
        private float degreesBoilingPoint;
        private float kelvinBoilingPoint;
        private float rankineBoilingPoint;
        private float fahrenheitBoilingPoint;
        private int displayOrder;

        public string RegNo { get { return regNo; } set { SetProperty(ref regNo, value); } }
        public string Name { get { return name; } set { SetProperty(ref name, value); } }
        public float DegreesBoilingPoint { get { return degreesBoilingPoint; } set { SetProperty(ref degreesBoilingPoint, value); } }
        public float KelvinBoilingPoint { get { return kelvinBoilingPoint; } set { SetProperty(ref kelvinBoilingPoint, value); } }
        public float RankineBoilingPoint { get { return rankineBoilingPoint; } set { SetProperty(ref rankineBoilingPoint, value); } }
        public float FahrenheitBoilingPoint { get { return fahrenheitBoilingPoint; } set { SetProperty(ref fahrenheitBoilingPoint, value); } }

        public override int DisplayOrder
        {
            get { return displayOrder; }
            set
            {
                SetProperty(ref displayOrder, value);
            }
        }
    }
}
