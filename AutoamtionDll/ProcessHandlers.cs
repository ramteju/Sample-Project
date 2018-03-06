using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WhiteAutoamtionDll
{
    public static class ProcessHandlers
    {
        #region //Mouce events
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            int xco = xpos;
            int yco = ypos;

            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        //window events
        [DllImport("USER32.DLL")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);



        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        #endregion

        #region processregion

        public static void killapp(string processname)
        {
            try
            {
                Process[] process = Process.GetProcessesByName(processname);
                if (process.Length > 0)
                {
                    foreach (Process proc in process)
                    {
                        KillProcessAndChildren(proc.Id);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private static void KillProcessAndChildren(int pid)
        {

            ManagementObjectSearcher searcher = new ManagementObjectSearcher
             ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                Process proc = Process.GetProcessById(pid);
                //proc.Kill();
                using (Process p = Process.GetProcessById(pid))
                {
                    if (p == null || p.HasExited)
                    { }
                    else
                    {
                        p.Kill();
                        p.WaitForExit();
                    }


                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

    }
}
