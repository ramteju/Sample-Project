using Client.ViewModels.Core;
using FoxitPDFSDKProLib;
using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for PdfSearch.xaml
    /// </summary>
    public partial class PdfSearch : Window
    {
        static private PdfSearch thisInstance;
        static public AxFoxitPDFSDKProLib.AxFoxitPDFSDK foxit;
        List<IFindResult> searchResults;
        public PdfSearch()
        {
            InitializeComponent();
            searchResults = new List<IFindResult>();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = 20;
            this.Left = 20;
            SearchList.SelectionChanged += SearchList_SelectionChanged;
        }

        private void SearchList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = SearchList.SelectedItem;
            if (item != null)
            {
                var searchResultVM = item as PdfSearchResultVM;
                if (searchResultVM != null && foxit != null)
                    foxit.GoToSearchResult(searchResultVM.FindResult);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public static void ShowWindow()
        {
            InitIfNot();
            if (!thisInstance.IsVisible)
                thisInstance.Show();
            thisInstance.BringIntoView();
        }

        private static void InitIfNot()
        {
            if (thisInstance == null)
                thisInstance = new PdfSearch();
        }

        private void Search()
        {
            PdfSearchVM vm = DataContext as PdfSearchVM;
            vm.Clear();
            searchResults.Clear();

            if (foxit != null)
            {
                foxit.SetSearchHighlightFillColor((uint)ColorTranslator.ToOle(System.Drawing.Color.Tomato), 200);
                if (!String.IsNullOrEmpty(Query.Text))
                {
                    FindResult result = foxit.FindFileFirst(foxit.FilePath, Query.Text, MatchCase.IsChecked.Value, WholeWord.IsChecked.Value);
                    if (result != null)
                    {
                        vm.Results.Add(new PdfSearchResultVM { PageNum = result.GetFindPageNum() + 1, ResultText = result.GetFindString(), FindResult = result });
                        FindResult nextResult;
                        while ((nextResult = foxit.FindFileNext()) != null)
                            vm.Results.Add(new PdfSearchResultVM { PageNum = nextResult.GetFindPageNum() + 1, ResultText = nextResult.GetFindString(), FindResult = nextResult });
                        vm.TotalCount = $"{vm.Results.Count} Found.";
                    }
                }
            }
            else
                AppInfoBox.ShowInfoMessage("Pdf Document Not Available");
        }

        private void Query_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        public static void SearchText(string text)
        {
            InitIfNot();
            thisInstance.Query.Text = text;
            if (!thisInstance.IsVisible)
                thisInstance.Show();
            thisInstance.Search();
        }
    }
}
