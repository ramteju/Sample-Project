using Client.Common;
using Client.Logging;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class TanCategoryVM : ViewModelBase
    {
        private string itemDescription;
        private int itemValue;

        public string Description { get { return itemDescription; } set { SetProperty(ref itemDescription, value); } }
        public int Value { get { return itemValue; } set { SetProperty(ref itemValue, value); } }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return this.itemValue == ((TanCategoryVM)obj).itemValue;
        }
        public override int GetHashCode()
        {
            return itemValue;
        }

        public override String ToString()
        {
            return Description;
        }

    }
}
