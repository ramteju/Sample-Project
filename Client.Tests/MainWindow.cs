using TestStack.White;
using TestStack.White.UIItems.WindowItems;

namespace Client.Tests
{
    public class MainWindow : BaseWindow
    {
        internal MainWindow(Window window) : base(window)
        {

        }

        public void OpenTaskSheet()
        {
            GetButton("SheetBtn").Click();
        }
        public void CreateReactionAfter()
        {
            GetButton("AfterInsert").Click();
        }
    }
}
