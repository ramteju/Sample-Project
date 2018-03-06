using Client.ViewModels;
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
    /// Interaction logic for ReactionPreviewLargeView.xaml
    /// </summary>
    public partial class ReactionPreviewLargeView : Window
    {
        public static ReactionPreviewLargeView ReactionPreview;
        public ReactionPreviewLargeView()
        {
            InitializeComponent();
            Closing += ReactionPreviewLargeView_Closing;
            PreviewKeyDown += ReactionPreviewLargeView_PreviewKeyDown;
        }

        private void ReactionPreviewLargeView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Hide();
        }

        private void ReactionPreviewLargeView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public static void ShowWindow(ReactionViewVM ReactionViewVM)
        {
            if (ReactionPreview == null)
                ReactionPreview = new ReactionPreviewLargeView();
            if (ReactionViewVM != null)
            {
                ReactionPreview.DataContext = ReactionViewVM;
                ReactionPreview.Show();
                ReactionPreview.Activate();
            }
        }
    }
}
