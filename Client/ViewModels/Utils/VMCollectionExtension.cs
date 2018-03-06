using Client.Logging;
using System;
using System.Collections.ObjectModel;

namespace Client.ViewModel
{
    static public class VMCollectionExtension
    {
        public static void UpdateDisplayOrder<T>(this ObservableCollection<T> orderableCollection) where T : OrderableVM
        {
            
            try
            {
                int count = 1;
                foreach (var item in orderableCollection)
                    item.DisplayOrder = (count++);
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
