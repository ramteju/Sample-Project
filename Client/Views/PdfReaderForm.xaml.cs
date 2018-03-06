using AxFoxitPDFSDKProLib;
using Client.Common;
using Client.Logging;
using Client.Views.Pdf;
using Entities.DTO;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for PdfReaderForm.xaml
    /// </summary>
    public partial class PdfReaderForm : Window
    {
        static public bool DialogStatus;
        public static int tanId { get; set; }
        public static string tanNumber { get; set; }

        public PdfReaderForm()
        {
            InitializeComponent();
            DialogStatus = false;
        }

        public bool? OpenDocument(string PdfPath, bool showControls = true)
        {
            try
            {
                DialogStatus = false;
                if (File.Exists(PdfPath))
                {
                    string DocumentPath = Path.Combine(T.TanFolderpath,"LocalPdfs", Path.GetFileName(PdfPath));
                    if (!Directory.Exists(Path.GetDirectoryName(DocumentPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(DocumentPath));
                    if (!File.Exists(DocumentPath))
                        File.Copy(PdfPath, DocumentPath);

                    AxFoxitPDFSDK foxit = PdfHost.Child as AxFoxitPDFSDK;
                    AxFoxitPDFSDK foxitSplitted = PdfHostSplitted.Child as AxFoxitPDFSDK;
                    OpenPdf(foxit,DocumentPath);
                    OpenPdf(foxitSplitted, DocumentPath);
                    if (showControls)
                        CompleteBtn.Visibility = Visibility.Visible;
                    else
                    {
                        CompleteBtn.Visibility = Visibility.Hidden;
                        Show();
                    }
                    if (showControls)
                    {
                        if (File.Exists(T.MasterTanDataFilePath))
                        {
                            var TanInfoDto = JsonConvert.DeserializeObject<TanInfoDTO>(File.ReadAllText(T.MasterTanDataFilePath));
                            TanInfoDto.Tan.DocumentReadStartTime = DateTime.Now;
                            string masterTanJson = JsonConvert.SerializeObject(TanInfoDto);
                            System.IO.File.WriteAllText(T.MasterTanDataFilePath, masterTanJson);
                        }
                        return ShowDialog();
                    }
                }
                else
                    AppInfoBox.ShowInfoMessage("PDF File not Found. Please Try after some time.");
                return false;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return null;
        }

        private void OpenPdf(AxFoxitPDFSDK foxit,string DocumentPath)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Do you have any doubts/query?", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    //this.Hide();
                    QueryWindow.SetTan(tanNumber, tanId);
                    QueryWindow.ShowWindow();
                }
                else
                {
                    AddReviewedUserId();
                    DialogResult = DialogStatus = true;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        public void AddReviewedUserId()
        {
            try
            {
                AxFoxitPDFSDK foxit = (PdfHost.Child as AxFoxitPDFSDK);
                if (foxit != null)
                {
                    var filepath = foxit.FilePath;
                    foxit.CloseFile();
                    if (File.Exists(T.MasterTanDataFilePath))
                    {
                        var TanInfoDto = JsonConvert.DeserializeObject<TanInfoDTO>(File.ReadAllText(T.MasterTanDataFilePath));
                        TanInfoDto.Tan.DocumentReadCompletedTime = DateTime.Now;
                        TanInfoDto.Tan.DocumentReviwedUser = U.UserId;
                        string masterTanJson = JsonConvert.SerializeObject(TanInfoDto);
                        System.IO.File.WriteAllText(T.MasterTanDataFilePath, masterTanJson);
                    }
                    if (File.Exists(filepath))
                        File.Delete(filepath);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                AxFoxitPDFSDK foxit = (PdfHost.Child as AxFoxitPDFSDK);
                if (foxit != null)
                    foxit.CloseFile();
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
            var foxit = PdfHost.Child as AxFoxitPDFSDK;
            if (foxit != null && !string.IsNullOrEmpty(foxit.FilePath))
            {
                PdfAnnotations.FilePath = foxit.FilePath;
                PdfAnnotations.ShowWindow();
            }
        }

        private void SearchSplittedPdf_Click(object sender, RoutedEventArgs e)
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
