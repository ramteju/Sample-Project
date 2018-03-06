using Client.Common;
using Client.Logging;
using Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Client.Util
{
    public static class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {

            try
            {
                ;
                return (string)obj.GetValue(BindableSourceProperty);
            }
            catch (Exception ex)
            {
                Log.This(ex);

                return "";
            }
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {

            try
            {
                ;
                obj.SetValue(BindableSourceProperty, value);
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                WebBrowser browser = o as WebBrowser;
                if (browser != null)
                {
                    string uri = e.NewValue as string;
                    browser.Source = !String.IsNullOrEmpty(uri) ? new Uri(uri) : null;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

    }
}
