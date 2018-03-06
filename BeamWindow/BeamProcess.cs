using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WhiteAutoamtionDll;
using TestStack.White;
using BeamWindow.BlueBeamService;

namespace BeamWindow
{
    public class BeamProcess
    {
        private static TestStack.White.Application _application;
        static string ExeSourceFile = ConfigurationSettings.AppSettings["Bluebeampath"].ToString();
        static string path = System.Configuration.ConfigurationSettings.AppSettings["folderpath"].ToString();
        static string PearlModelCommandPrompt = "Running the OCR module...";

        public enum Status
        {
            Progress,
            Completed,
            Nopdf,
            Error
        }

        public static void RunModel()
        {
            BlueBeamService.BlueDataClient blueobj = new BlueDataClient();
            BlueBeamService.Tan tan = blueobj.GetAllFilePaths();
            try
            {
                if (tan != null)
                {

                    if (tan.OCRStatus == Status.Progress.ToString())
                    {

                    }
                    else
                    {
                        if (tan.DocumentPath != null)
                        {
                            tan.OCRStatus = Status.Progress.ToString();
                            blueobj.UpdateFileModel(tan);
                            RunOCRonBluebeamtool(tan.DocumentPath);
                            tan.OCRStatus = Status.Completed.ToString();
                            blueobj.UpdateFileModel(tan);
                            blueobj.Close();
                        }
                        else
                        {
                            tan.OCRStatus = Status.Nopdf.ToString();
                            blueobj.UpdateFileModel(tan);
                            blueobj.Close();
                        }
                    }
                }
               
            }
            catch (Exception)
            {
                tan.OCRStatus = Status.Error.ToString();
                blueobj.UpdateFileModel(tan);
                blueobj.Close();
            }
           
        }
        public static void RunOCRonBluebeamtool(string folderpath)
        {
            try
            {
                ProcessHandlers.killapp("Revu");
                var psi = new ProcessStartInfo(ExeSourceFile);
                psi.UseShellExecute = true;
                _application = TestStack.White.Application.AttachOrLaunch(psi);
                Process p = Process.GetProcessesByName("Revu").FirstOrDefault();
                while (p.MainWindowHandle == IntPtr.Zero)
                {
                    p.Refresh();
                }
                p.WaitForInputIdle();
                if (p != null)
                {
                    IntPtr h = p.MainWindowHandle;
                    ProcessHandlers.SetForegroundWindow(h);
                    p.WaitForInputIdle();
                    IntPtr hPRAZChem = ProcessHandlers.FindWindow(null, "Bluebeam Revu x64");
                    if (!hPRAZChem.Equals(IntPtr.Zero))
                    {
                        ProcessHandlers.SetForegroundWindow(hPRAZChem);
                    }
                    p.WaitForInputIdle();

                    Thread.Sleep(3000);


                    sendkeyevent(1, "%");
                  //  sendkeyevent(1, "{RIGHT}");
                    sendkeyevent(1, "{DOWN}");
                    sendkeyevent(7, "{RIGHT}");
                    sendkeyevent(1, "{ENTER}");
                    sendkeyevent(1, "{DOWN}");
                    sendkeyevent(1, "{ENTER}");

                    IntPtr hPRAZChem1 = ProcessHandlers.FindWindow(null, "Add Files for Batch Processing");
                    p.WaitForInputIdle();
                    if (!hPRAZChem1.Equals(IntPtr.Zero))
                    {
                        ProcessHandlers.SetForegroundWindow(hPRAZChem1);
                    }
                    p.WaitForInputIdle();

                    SendKeys.SendWait(folderpath);
                    Thread.Sleep(10000);
                    if (folderpath.Contains(".pdf"))
                    {
                        sendkeyevent(4, "{ENTER}");
                    }
                    else
                    {

                        sendkeyevent(1, "{ENTER}");
                        Thread.Sleep(3000);
                        sendkeyevent(8, "{TAB}");
                        sendkeyevent(1, "^(a)");
                        // SendKeys.SendWait("{A}");
                        sendkeyevent(4, "{ENTER}");
                    }
                    checkprogress();
                    checkprogress();
                    p.WaitForInputIdle();
                    ProcessHandlers.killapp("Revu");
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        private static void checkprogress()
        {
            try
            {
                IntPtr hFocusPearlCommandPrompt = ProcessHandlers.FindWindow(null, PearlModelCommandPrompt);
                while (!(hFocusPearlCommandPrompt.Equals(IntPtr.Zero)))
                {
                    Thread.Sleep(25000);
                    hFocusPearlCommandPrompt = ProcessHandlers.FindWindow(null, PearlModelCommandPrompt);
                    Thread.Sleep(25000);
                }
                Thread.Sleep(25000);
                IntPtr hFocusPearlCommandPromptCheck = ProcessHandlers.FindWindow(null, PearlModelCommandPrompt);
                while (!(hFocusPearlCommandPromptCheck.Equals(IntPtr.Zero)))
                {
                    Thread.Sleep(25000);
                    hFocusPearlCommandPromptCheck = ProcessHandlers.FindWindow(null, PearlModelCommandPrompt);
                    Thread.Sleep(25000);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        private static void sendkeyevent(int v1, string v2)
        {

            for (int i = 0; i < v1; i++)
            {
                System.Windows.Forms.SendKeys.SendWait(v2);
                Thread.Sleep(3000);
            }

        }
    }
}
