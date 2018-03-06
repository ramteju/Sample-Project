using AxFoxitPDFSDKProLib;
using Client.Common;
using Client.Logging;
using System;
using System.IO;
using System.Windows;

namespace Client.Views.Pdf
{
    /// <summary>
    /// Interaction logic for ExportedPDF.xaml
    /// </summary>
    public partial class ExportedPDF : Window
    {
        public static ExportedPDF thisWindow;
        public ExportedPDF()
        {
            InitializeComponent();
        }

        public static void OpenDocument(string DocumentPath)
        {
            try
            {
                if (thisWindow == null)
                    thisWindow = new ExportedPDF();
                if (File.Exists(DocumentPath))
                {
                    AxFoxitPDFSDK foxit = thisWindow.PdfHost.Child as AxFoxitPDFSDK;
                    AxFoxitPDFSDK foxitSplitted = thisWindow.PdfHostSplitted.Child as AxFoxitPDFSDK;
                    OpenPdf(foxit, DocumentPath);
                    OpenPdf(foxitSplitted, DocumentPath);
                    thisWindow.Show();
                }
                else
                    AppInfoBox.ShowInfoMessage("PDF File not Found. Please Try after some time.");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private static void OpenPdf(AxFoxitPDFSDK foxit, string DocumentPath)
        {
            if (foxit != null)
            {
                foxit.UnLockActiveX(C.LICENCE_ID, C.UNLOCK_CODE);
                foxit.ShowTitleBar(false);
                foxit.ShowToolBar(false);
                foxit.ShowNavigationPanels(true);
                foxit.SetShowSavePrompt(false, 0);
                foxit.CurrentTool = C.TEXT_TOOL;
            }

            if (foxit != null)
            {
                try
                {
                    if (!foxit.OpenFile(DocumentPath, null))
                        AppErrorBox.ShowErrorMessage("Can't open pdf file", "Can't open pdf file");
                    foxit.CurrentTool = C.TEXT_TOOL;
                    foxit.RemoveEvaluationMark();
                }
                catch (Exception ex)
                {
                    Log.This(ex);
                    AppErrorBox.ShowErrorMessage("Unable to Open PDF", ex.ToString());
                }
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                AxFoxitPDFSDK foxit = (PdfHost.Child as AxFoxitPDFSDK);
                AxFoxitPDFSDK splittedfoxit = (PdfHostSplitted.Child as AxFoxitPDFSDK);
                if (foxit != null)
                    foxit.CloseFile();
                if (splittedfoxit != null)
                    splittedfoxit.CloseFile();
                Hide();
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void SearchPdf_Click(object sender, RoutedEventArgs e)
        {
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null)
            {
                PdfSearch.foxit = foxit;
                PdfSearch.ShowWindow();
            }
        }

        private void ShowAnnotations_Click(object sender, RoutedEventArgs e)
        {
            var foxit = PdfHostSplitted.Child as AxFoxitPDFSDK;
            if (foxit != null)
            {
                PdfSearch.foxit = foxit;
                PdfSearch.ShowWindow();
            }
        }
    }
}
