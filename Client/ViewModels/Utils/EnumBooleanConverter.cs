using Client.Util;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Client.ViewModels
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

    public class EnumBooleanReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }


    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,object parameter, System.Globalization.CultureInfo culture)
        {
            return (Int32)value == 0 ? parameter = null : (Int32)value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class ReverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                value = "0";
            return value.ToString().Equals("0") ? parameter = null : value.ToString();
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            string stringValue = value != null ? value.ToString() : "0";

            return stringValue.Equals("0") ? Binding.DoNothing : stringValue;
        }
    }

    public class StyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush CompleteRxnBrush = null;
            if ((int)value % 2 == 0)
                CompleteRxnBrush = new SolidColorBrush(System.Drawing.Color.LightSalmon.ToMediaColor());
            else
                CompleteRxnBrush = new SolidColorBrush(System.Drawing.Color.LightYellow.ToMediaColor());
            return CompleteRxnBrush;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
