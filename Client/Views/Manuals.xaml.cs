using System;
using System.Windows;
using System.Windows.Controls;
using AxFoxitPDFSDKProLib;
using System.IO;
using Client.Common;
using Client.Logging;
using Client.ViewModels;
using System.Configuration;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for Manuals.xaml
    /// </summary>
    public partial class Manuals : Window
    {
        static public bool DialogStatus;
        public static int tanId { get; set; }
        public static string tanNumber { get; set; }
        static private Manuals thisInstance;
        public Manuals()
        {
            InitializeComponent();
            
        }
        static public void ShowUserManuls()
        {
            try
            {
                if (thisInstance == null)
                    thisInstance = new Manuals();
                if (!thisInstance.IsVisible)
                    thisInstance.Show();
                thisInstance.Focus();
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
                e.Cancel = true;
                Hide();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void ReactionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string DocumentPath;
                if (ReactionsList.SelectedValue != null)
                {
                    DocumentPath = Path.Combine(C.UserManualsPath,(ReactionsList.SelectedValue as UserManualVM).Value);
                    if (File.Exists(DocumentPath))
                    {
                        AxFoxitPDFSDK foxit = UserPdfHost.Child as AxFoxitPDFSDK;
                        if (foxit != null)
                        {
                            foxit.UnLockActiveX(C.LICENCE_ID, C.UNLOCK_CODE);
                            foxit.ShowTitleBar(false);
                            foxit.ShowToolBar(false);
                            foxit.ShowNavigationPanels(true);
                            foxit.SetShowSavePrompt(false, 0);
                        }

                        if (foxit != null)
                        {
                            if (!foxit.OpenFile(DocumentPath, String.Empty))
                                AppErrorBox.ShowErrorMessage("Can't open pdf file", "Can't open pdf file");
                        }
                    }
                    else
                        AppInfoBox.ShowInfoMessage("PDF File not Found. Please Try after some time.");
                }
                
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
