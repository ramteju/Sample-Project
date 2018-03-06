using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client.ViewModels.Utils
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility text1Vis = (Visibility)values;

            if (text1Vis == Visibility.Collapsed)
                return Visibility.Collapsed;

            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
