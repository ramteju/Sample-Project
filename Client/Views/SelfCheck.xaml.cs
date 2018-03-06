﻿using Client.ViewModels.Utils;
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
    /// Interaction logic for SelfCheck.xaml
    /// </summary>
    public partial class SelfCheck : Window
    {
        static private SelfCheck thisInstance;
        public SelfCheck()
        {
            InitializeComponent();
        }

        public static void ShowWindow()
        {
            if (thisInstance == null)
                thisInstance = new SelfCheck();
            if (!thisInstance.IsVisible)
            {
                thisInstance.Show();
                (thisInstance.DataContext as SelfCheckVM).LoadData();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
