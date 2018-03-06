using System;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Client.Logging;
using Client.Views;
using System.Diagnostics;
using Client.Stores;
using Client.Common;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            string procName = Process.GetCurrentProcess().ProcessName;
            Process[] processes = Process.GetProcessesByName(procName);
            if (processes.Length > 1)
            {
                MessageBox.Show("Only one instance of Reactions-NG is allowed !","Reactions - NG",MessageBoxButton.OK,MessageBoxImage.Information);
                Environment.Exit(0);
            }
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionHandler);
            Application.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;


            if (!Directory.Exists(C.MolImagesFolder) || !File.Exists(C.FilePath8500) || !File.Exists(C.FilePath9000) || !Directory.Exists(C.UserManualsPath))
            {
                MessageBox.Show("We will download some required information, After that please restart the application . .", "Reactions - NG", MessageBoxButton.OK, MessageBoxImage.Information);
                new DataImport().Import();
            }
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleFatal(e.Exception);
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleFatal(e.Exception);
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            HandleFatal(e.Exception);
        }

        private void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception == null)
                exception = new Exception(e.ToString());
            HandleFatal(exception);
        }

        private void HandleFatal(Exception ex)
        {
            Log.This(ex);
        }

        [STAThread]
        private void App_Startup(object sender, StartupEventArgs e)
        {
            //App.Current.MainWindow = MainWindow;
            //LoginWindow mainUI = new LoginWindow();
            //LoginVM mainVM = new LoginVM();
            //mainUI.DataContext = mainVM;
            //mainUI.Show();
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
