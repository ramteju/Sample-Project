using Client.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.White.UIItems.WindowItems;

namespace Client.Tests
{
    public class LoginWindow : BaseWindow
    {
        internal LoginWindow(Window window) : base(window)
        {

        }

        public void SetPassword(string password)
        {
            GetTexField("txtPassword").Text = password;
        }
        public void SetRole(ProductRole role)
        {
            var selectbax = GetSelectBox("cmbRoles");
            selectbax.SetValue(role);
        }
        public void DoLogin()
        {
            GetButton("button").Click();
        }
    }
}
