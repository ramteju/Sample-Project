using AxFoxitPDFSDKProLib;
using Client.Common;
using Client.Logging;
using Client.ViewModel;
using Client.ViewModels.Pdf;
using FoxitPDFSDKProLib;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Client.Views.Pdf
{
    /// <summary>
    /// Interaction logic for PdfAnnotations.xaml
    /// </summary>
    public partial class PdfAnnotations : Window
    {
        private static PdfAnnotations thisInstance;
        static public string FilePath;

        static public event EventHandler<PdfAnnotationResultVM> AnnotationSelected = delegate { };
        public PdfAnnotations()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            this.Top = 20;
            this.Left = 20;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
        public static void ShowWindow()
        {
            thisInstance = new PdfAnnotations();
            thisInstance.CollectAnnotations();
            if (!thisInstance.IsVisible)
                thisInstance.Show();
        }

        private void CollectAnnotations()
        {
            PdfAnnotationsVM vm = thisInstance.DataContext as PdfAnnotationsVM;
            vm.Clear();
            AxFoxitPDFSDK Foxit = (PdfHost.Child as AxFoxitPDFSDK);
            (PdfHost.Child as AxFoxitPDFSDK).UnLockActiveX(C.LICENCE_ID, C.UNLOCK_CODE);
            if (!String.IsNullOrEmpty(FilePath) && File.Exists(FilePath) && Foxit != null)
            {
                try
                {
                    if (Foxit.OpenFile(FilePath, null))
                    {
                        vm.FileName = System.IO.Path.GetFileName(FilePath);
                        Foxit.ForceRefresh();
                        for (int page = 0; page < Foxit.PageCount; page++)
                        {
                            PDFPageAnnots annotations = Foxit.GetPageAnnots(page);
                            int count = annotations.GetAnnotsCount();
                            for (int index = 0; index < count; index++)
                            {
                                PDFAnnot annot = annotations.GetAnnot(index);
                                vm.Results.Add(new PdfAnnotationResultVM { Text = annot.GetContents(), PageNum = page + 1, Author = annot.Author, Type = annot.GetSubType() });
                            }
                        }
                        vm.Results.UpdateDisplayOrder();
                        vm.TotalCount = vm.Results.Count.ToString();
                        Foxit.CloseFile();
                    }
                    else
                        AppErrorBox.ShowErrorMessage("Pdf can't be opened to collect annotations", $"{FilePath} Can't be opened");
                }
                catch (Exception ex)
                {
                    AppErrorBox.ShowErrorMessage("Can't collect annotations . . ", ex.ToString());
                    Log.This(ex);
                }
            }
            else
                AppInfoBox.ShowInfoMessage("Pdf file not avaialble");
        }

        private void AnnotationsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var vm = DataContext as PdfAnnotationsVM;
            if (vm != null && vm.SelectedAnnotation != null)
            {
                AnnotationSelected.Invoke(this, vm.SelectedAnnotation);
            }
        }
    }
}
