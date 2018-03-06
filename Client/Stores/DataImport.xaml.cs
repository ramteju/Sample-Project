using Client.Common;
using Client.Logging;
using Client.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Stores
{
    /// <summary>
    /// Interaction logic for DataImport.xaml
    /// </summary>
    public partial class DataImport : Window
    {
        BackgroundWorker worker;
        public DataImport()
        {
            InitializeComponent();
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.DoWork += Worker_DoWork;
        }
        public void Import()
        {
            worker.RunWorkerAsync();
            Show();
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                int count = 0;
                int all = 2;
                if (Directory.Exists(C.NetworkImagesFolder))
                {
                    if (!Directory.Exists(C.MolImagesFolder))
                        Directory.CreateDirectory(C.MolImagesFolder);
                    worker.ReportProgress(0, "Loading Data . .");
                    var files = Directory.GetFiles(C.NetworkImagesFolder, "*", SearchOption.AllDirectories);
                    all += files.Length;
                    Parallel.ForEach(files, (file) =>
                    {
                        File.Copy(file, Path.Combine(C.MolImagesFolder, Path.GetFileName(file)), true);
                        Interlocked.Increment(ref count);
                        worker.ReportProgress(0, $"Importing {count} / {all} . .");
                    });
                }
                else
                    worker.ReportProgress(0, new Exception("Source images not avaliable"));
                if (Directory.Exists(C.NetworkFolderPathUserManuals))
                {
                    all = 2;
                    if (!Directory.Exists(C.UserManualsPath))
                        Directory.CreateDirectory(C.UserManualsPath);
                    worker.ReportProgress(0, "Loading UserManuals . .");
                    var files = Directory.GetFiles(C.NetworkFolderPathUserManuals, "*", SearchOption.AllDirectories);
                    all += files.Length;
                    Parallel.ForEach(files, (file) =>
                    {
                        File.Copy(file, Path.Combine(C.UserManualsPath, Path.GetFileName(file)), true);
                        Interlocked.Increment(ref count);
                        worker.ReportProgress(0, $"Importing {count} / {all} . .");
                    });
                }
                else
                    worker.ReportProgress(0, new Exception("Source User manuals not available"));

                worker.ReportProgress(0, $"Importing {count} / {all} . .");

                if (File.Exists(C.NetworkFilePath8500))
                    File.Copy(C.NetworkFilePath8500, C.FilePath8500, true);
                else
                    worker.ReportProgress(0, new Exception("Source 8500.xml not available"));

                count++;

                worker.ReportProgress(0, $"Importing {count} / {all} . .");
                if (File.Exists(C.NetworkFilePath9000))
                    File.Copy(C.NetworkFilePath9000, C.FilePath9000, true);
                else
                    worker.ReportProgress(0, new Exception("Source 9000.xml not available"));
            }
            catch (Exception ex)
            {
                Log.This(ex);
                worker.ReportProgress(0, ex);
            }
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Dispatcher.BeginInvoke((System.Action)delegate ()
            {
                Hide();
                AppInfoBox.ShowInfoMessage("Please Restart The Tool . .");
                Environment.Exit(0);
            });
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null)
                return;
            if (e.UserState is Exception)
                MessageBox.Show("Can't import data", e.UserState.ToString());
            else
                this.Dispatcher.BeginInvoke((System.Action)delegate ()
                {
                    Progress.Content = e.UserState.ToString();
                });
        }
    }
}
