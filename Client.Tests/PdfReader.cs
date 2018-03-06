using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.White.UIItems.WindowItems;

namespace Client.Tests
{
    public class PdfReaderWindow : BaseWindow
    {
        internal PdfReaderWindow(Window window) : base(window)
        {

        }

        public void CompletPdf()
        {
            GetButton("CompleteBtn").Click();
        }
    }
}
