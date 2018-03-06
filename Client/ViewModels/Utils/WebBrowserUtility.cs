using Client.Common;
using Client.Logging;
using Entities;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Client.ViewModels.Utils
{
    public static class WebBrowserUtility
    {
        public static readonly DependencyProperty BindableSourceProperty =
            DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebBrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            
            try
            {
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
                    try
                    {
                        string uri = e.NewValue as string;
                        browser.Source = !String.IsNullOrEmpty(uri) ? new Uri(uri) : null;
                    }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
                    catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
                    {
                        MessageBox.Show("The given xml path is not valid . .","Reactions",MessageBoxButton.OK,MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

    }
}
