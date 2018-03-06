using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestStack.White;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.Utility;

namespace Client.Tests
{
    public class AppWindows
    {
        private static Application app;
        public static void Init(Application application)
        {
            AppWindows.app = application;
        }
        public static MainWindow Main
        {
            get
            {
                return new MainWindow(GetWindow("Reactions - "));
            }
        }
        public static LoginWindow Login
        {
            get
            {
                return new LoginWindow(GetWindow("Login -"));
            }
        }
        public static TaskWindow TaskWindow
        {
            get
            {
                return new TaskWindow(GetWindow("Task Sheet"));
            }
        }
        public static PdfReaderWindow PdfReader
        {
            get
            {
                return new PdfReaderWindow(GetWindow("Pdf Reader"));
            }
        }

        private static Window GetWindow(string title)
        {
            Window window = null;
            while (window == null)
            {
                try
                {
                    window = app.GetWindows().First(w => w.Title.StartsWith(title));
                    if (window != null)
                        return window;
                    Thread.Sleep(150);
                }
                catch (Exception e)
                {

                }
            }
            return window;
        }
    }
}
