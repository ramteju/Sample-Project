using Client.Common;
using Client.Logging;
using Client.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for Conditions.xaml
    /// </summary>
    public partial class Conditions : Window
    {
        ConditionVM conditionVM;

        public Conditions()
        {
            InitializeComponent();
            conditionVM = new ConditionVM();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            try
            {

                e.Cancel = true;
                this.Hide();
                if (App.Current != null && App.Current.MainWindow != null)
                    (App.Current.MainWindow).Focus();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key < Key.D0 || e.Key > Key.D9)
                {

                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Tab)
                            {
                                if (e.Key != Key.OemPeriod)
                                {
                                    e.Handled = true;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void TextBoxPH_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key < Key.D0 || e.Key > Key.D9)
                {

                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Tab)
                            {
                                if (e.Key != Key.OemPeriod)
                                {
                                    if (e.Key != Key.OemComma)
                                    {
                                        e.Handled = true;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private bool IsShiftKey { get; set; }

        private bool checkShiftConditions(KeyEventArgs e)
        {
            if (e.Key == Key.D1 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)   /// Shift + ! key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D2 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + @ key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D3 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + # key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D4 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + $ key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D5 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + % key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D6 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + ^ key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D7 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + & key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D8 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + * key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D9 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + ( key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.D0 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + ) key event Stoped...
            {
                IsShiftKey = true;
            }
            else if (e.Key == Key.OemMinus && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)  /// Shift + ) key event Stoped...
            {
                IsShiftKey = true;
            }
            //else if (e.Key == Key.OemPeriod && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)   /// Shift + > key event Stoped...
            //{
            //    IsShiftKey = true;
            //}
           
            else
            {
                IsShiftKey = false;
            }
            return IsShiftKey;
        }

        private void TextBoxTempRange_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                checkSpecialChar(sender);
                bool Output = checkShiftConditions(e);
                if (Output)
                {
                    e.Handled = true;
                }

                if (e.Key == Key.OemPeriod && ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
                {
                    e.Handled = true;
                }

                else if (e.Key < Key.D0 || e.Key > Key.D9)
                {

                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        //if (e.Key == Key.D7 && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        //{
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Subtract)
                            {
                                if (e.Key != Key.OemMinus)
                                {
                                    if (e.Key != Key.Tab)
                                    {
                                        if (e.Key != Key.OemPeriod)
                                        {
                                            e.Handled = true;
                                        }
                                    }
                                }
                            }
                            //}
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

               
        private void TextBoxRange_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.Key < Key.D0 || e.Key > Key.D9)
                {

                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Tab)
                            {
                                if (e.Key != Key.OemPeriod)
                                {
                                    e.Handled = true;
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }


        private void TextBoxPHRange_KeyDown(object sender, KeyEventArgs e)
        {
            checkSpecialChar(sender);
            bool Output = checkShiftConditions(e);
            if (Output)
            {
                e.Handled = true;
            }
            try
            {
                if (e.Key < Key.D0 || e.Key > Key.D9)
                {

                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Tab)
                            {
                                if (e.Key != Key.OemPeriod)
                                {
                                    if (e.Key != Key.OemComma)
                                    {
                                        e.Handled = true;
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void TextBoxPresure_KeyDown(object sender, KeyEventArgs e)
        {

            try
            {
                checkSpecialChar(sender);
                bool Output = checkShiftConditions(e);
                if (Output)
                {
                    e.Handled = true;
                }

                if (e.Key < Key.D0 || e.Key > Key.D9)
                {

                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Tab)
                            {
                                e.Handled = true;

                            }
                        }
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void checkSpecialChar(object sender)
        {
            TextBox s = (sender as TextBox);
            var chr = new HashSet<char> { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_' };
            bool test = s.Text.Any(c => chr.Contains(c));

            if (test)
            {
                s.Text = "";
                s.Focus();
            }
        }

        private void txtTemp_KeyPress(object sender, KeyEventArgs e)
        {
            try
            {
                checkSpecialChar(sender);

                bool Output = checkShiftConditions(e);
                if (Output)
                {
                    e.Handled = true;
                }
                if (e.Key == Key.OemComma && ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
                {
                
                    // e.Handled = false;
                }
                    //e.Handled = true;
                

                //BKSP-8,--45,.-46, <-60,>-62
                else if (e.Key < Key.D0 || e.Key > Key.D9)
                {
                    
                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Tab)
                            {
                                if (e.Key != Key.OemPeriod)
                                {
                                    if (e.Key != Key.OemMinus)
                                    {
                                        //if (e.Key == Key.OemComma)
                                        //{



                                        if (e.Key != Key.Subtract)
                                        {
                                            e.Handled = true;
                                        }


                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }


        }



        private void txtTime_KeyPress(object sender, KeyEventArgs e)
        {

            try
            {
                checkSpecialChar(sender);
                bool Output = checkShiftConditions(e);
                if (Output)
                {
                    e.Handled = true;
                }
                if (e.Key == Key.OemComma && ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift))
                {

                    // e.Handled = false;
                }


                //BKSP-8,--45,.-46, <-60,>-62
               else if (e.Key < Key.D0 || e.Key > Key.D9)
                {

                    if (e.Key < Key.NumPad0 || e.Key > Key.NumPad9)
                    {
                        if (e.Key != Key.Decimal)
                        {
                            if (e.Key != Key.Tab)
                            {
                                if (e.Key != Key.OemPeriod)
                                {

                                   

                                        e.Handled = true;

                                 

                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }


        }

        public void show()
        {
            this.Height = 580;
            this.Width = 870;
            this.Show();
        }

    }
}
