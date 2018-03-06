using Client.ViewModels.Extended;
using System.Collections.Generic;
using System.Windows;
using Client.ViewModel;
using System.Linq;
using System;
using System.Diagnostics;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for PdfReactionsView.xaml
    /// </summary>
    public partial class PdfReactionsView : Window
    {
        static private PdfReactionsView thisInstance;
        public PdfReactionsView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        static public void ShowWindow(List<ListToShow> allReactions, List<ListToShow> pdfReactions, string HeaderText = "", string LeftBoxText = "", string RightBoxText = "")
        {
            var StartTime = DateTime.Now;
            Debug.WriteLine($"ShowPdfReactionViewWindow ShowWindow started at: {StartTime}");
            if (thisInstance == null)
                thisInstance = new PdfReactionsView();
            var pdfReactionViewVM = thisInstance.DataContext as PdfReactionViewVM;
            if (allReactions != null)
            {
                pdfReactionViewVM.LeftBoxCollection = new System.Collections.ObjectModel.ObservableCollection<ListToShow>(allReactions.Where(r=>!r.IsValid));
                pdfReactionViewVM.LeftBoxVisibility = allReactions != null ? Visibility.Visible : Visibility.Collapsed;
                pdfReactionViewVM.LeftBoxCollection.UpdateDisplayOrder();
            }
            else
            {
                pdfReactionViewVM.LeftBoxCollection = null;
                pdfReactionViewVM.LeftBoxVisibility = Visibility.Collapsed;
            }
            if (pdfReactions != null)
            {
                pdfReactionViewVM.RightBoxCollection = new System.Collections.ObjectModel.ObservableCollection<ListToShow>(pdfReactions.Where(r => !r.IsValid));
                pdfReactionViewVM.RightBoxVisibility = Visibility.Visible;
                pdfReactionViewVM.RightBoxCollection.UpdateDisplayOrder();
            }
            else
            {
                pdfReactionViewVM.RightBoxCollection = null;
                pdfReactionViewVM.RightBoxVisibility = Visibility.Collapsed;
            }
            pdfReactionViewVM.LeftHeaderBoxText = LeftBoxText;
            pdfReactionViewVM.RightHeaderBoxText = RightBoxText;

            Debug.WriteLine($"ShowPdfReactionViewWindow ShowWindow Done in: {(DateTime.Now - StartTime).TotalSeconds} seconds");
            if (!thisInstance.IsVisible)
                thisInstance.ShowDialog();
        }
    }
}
