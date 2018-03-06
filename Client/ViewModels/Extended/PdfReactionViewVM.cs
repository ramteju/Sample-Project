using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels.Extended
{
    public class PdfReactionViewVM : ViewModelBase
    {
        public PdfReactionViewVM()
        {
            LeftBoxCollection = new ObservableCollection<ListToShow>();
            RightBoxCollection = new ObservableCollection<ListToShow>();
        }
        private string leftHeaderBoxText;
        private string rightHeaderBoxText;
        private ObservableCollection<ListToShow> leftBoxCollection;
        private ObservableCollection<ListToShow> rightBoxCollection;
        private Visibility leftBoxVisibility;
        private Visibility rightBoxVisibility;
        public string RightHeaderBoxText { get { return rightHeaderBoxText; } set { SetProperty(ref rightHeaderBoxText, value); } }
        public string LeftHeaderBoxText { get { return leftHeaderBoxText; } set { SetProperty(ref leftHeaderBoxText, value); } }
        public ObservableCollection<ListToShow> LeftBoxCollection { get { return leftBoxCollection; } set { SetProperty(ref leftBoxCollection, value); } }
        public ObservableCollection<ListToShow> RightBoxCollection { get { return rightBoxCollection; } set { SetProperty(ref rightBoxCollection, value); } }
        public Visibility LeftBoxVisibility { get { return leftBoxVisibility; } set { SetProperty(ref leftBoxVisibility, value); } }
        public Visibility RightBoxVisibility { get { return rightBoxVisibility; } set { SetProperty(ref rightBoxVisibility, value); } }
    }

    public class ListToShow : OrderableVM
    {
        private bool isValid;
        private string textToShow;
        private int displayOrder;

        public bool IsValid { get { return isValid; } set { SetProperty(ref isValid, value); } }
        public string TextToShow { get { return textToShow; } set { SetProperty(ref textToShow, value); } }
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
