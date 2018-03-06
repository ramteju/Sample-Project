using Client.Common;
using System;

namespace Client.ViewModels
{
    public class CvtVM : ViewModelBase
    {
        private int id;
        private String text, associatedFreeText; private bool isIgnorableInDelivery;
        public bool IsIgnorableInDelivery { get { return isIgnorableInDelivery; } set { SetProperty(ref isIgnorableInDelivery, value); } }
        public int Id { get { return id; } set { SetProperty(ref id, value); } }
        public String Text
        {
            get { return text; }
            set
            {
                SetProperty(ref text, value);
                //if (S.IgnorableCVTs.Contains(value))
                //    ForeColor = "Blue";
                //else
                //    ForeColor = "Black";
            }
        }
        public String AssociatedFreeText { get { return associatedFreeText; } set { SetProperty(ref associatedFreeText, value); } }
        public string ForeColor { get; set; }
    }
}
