using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModels.Delivery
{
    public class ValidateXSDVM : ViewModelBase
    {
        private string level, text;
        private int lineNumber, linePosition;
        public string Level { get { return level; } set { SetProperty(ref level, value); } }
        public string Text { get { return text; } set { SetProperty(ref text, value); } }
        public int LineNumber { get { return lineNumber; } set { SetProperty(ref lineNumber, value); } }
        public int LinePosition { get { return linePosition; } set { SetProperty(ref linePosition, value); } }
    }
}
