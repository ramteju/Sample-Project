using Client.Logging;
using Client.Styles;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.ViewModels
{
    public class RxnCompleteToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            try
            {
                if (value != null && value is bool && ((bool)value))
                    return StyleConstants.CompleteRxnBrush;
                return StyleConstants.IncompleteRxnBrush;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
