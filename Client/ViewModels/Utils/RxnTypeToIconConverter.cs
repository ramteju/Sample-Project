using Client.Logging;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Client.ViewModels
{
    public class RxnTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            try
            {
                if (value != null && value is Guid && ((Guid)value) != Guid.Empty)
                    return "/images/a-icon.png";
                return "/images/m-icon.png";
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
