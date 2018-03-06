using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestStack.White;
using Xunit;

namespace Client.Tests
{
    public class Tests
    {

        [ReadableFact]
        public void VerifyWrongPassword()
        {
            var application = TestApp.GetInstance();
            AppWindows.Init(application);
            var loginWindow = AppWindows.Login;
            loginWindow.SetPassword("WrongPassword");
            loginWindow.DoLogin();
            var errorMsg = loginWindow.GetMessageBoxText(String.Empty);
            Assert.Equal("Login FAILED !", errorMsg);
            application.Close();
            application.Dispose();
        }

        [ReadableFact]
        public void ValidateUserLogin()
        {
            var application = TestApp.GetInstance();
            AppWindows.Init(application);

            var loginWindow = AppWindows.Login;
            loginWindow.SetPassword("R@M$4554");
            loginWindow.DoLogin();

            var mainWindow = AppWindows.Main;
            Assert.StartsWith("Reactions - ", mainWindow.window.Title);

            application.Close();
            application.Dispose();
        }

        [ReadableFact]
        public void OpenTanFromTaskSheetAsACurator()
        {
            var tanNumber = "34584855V";
            var application = TestApp.GetInstance();
            AppWindows.Init(application);

            var loginWindow = AppWindows.Login;
            loginWindow.SetPassword("R@M$4554");
            ProductRole role = new ProductRole { RoleId = 1,RoleName = "Curator"};
            loginWindow.SetRole(role);
            loginWindow.DoLogin();

            var mainWindow = AppWindows.Main;
            try
            {
                mainWindow.OpenTaskSheet();
            }
            catch (Exception)
            {

            }
            var taskWindow = AppWindows.TaskWindow;
            taskWindow.DoubleClickOnRow("UserTasksGrid", tanNumber);
            try
            {
                mainWindow.CreateReactionAfter();
            }
            catch (Exception)
            {

            }
            Thread.Sleep(8000);
            //AppWindows.PdfReader.CompletPdf();

            application.Close();
            application.Dispose();
        }
    }
}
