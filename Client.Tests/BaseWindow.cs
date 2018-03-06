using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestStack.White.UIItems;
using TestStack.White.UIItems.Finders;
using TestStack.White.UIItems.ListBoxItems;
using TestStack.White.UIItems.TableItems;
using TestStack.White.UIItems.WindowItems;

namespace Client.Tests
{
    public class BaseWindow
    {
        internal Window window;
        internal BaseWindow(Window window)
        {
            this.window = window;
        }
        public Label GetLabel(string name)
        {
            return window.Get<Label>(name);
        }
        public string GetMessageBoxText(string messageBoxTitle)
        {
            return GetMessageBox(messageBoxTitle).Get<Label>(SearchCriteria.Indexed(0)).Text;
        }
        public TextBox GetTexField(string name)
        {
            return window.Get<TextBox>(name);
        }
        public Button GetButton(string name)
        {
            return window.Get<Button>(name);
        }
        public Window GetMessageBox(string name)
        {
            return window.MessageBox(name);
        }
        public ListView GetListView(string name)
        {
            return window.Get<ListView>(name);
        }
        public ComboBox GetSelectBox(string name)
        {
            return window.Get<ComboBox>(name);
        }
        public void DoubleClickOnRow(string gridName, string cellText)
        {
            var listView = GetListView(gridName);
            if (listView != null)
            {
                //StringBuilder sb = new StringBuilder();
                //foreach (var row in listView.Rows)
                //{
                //    sb.AppendLine(row.ToString());
                //}
                //if (File.Exists(@"E:\Sample.txt"))
                //    File.Delete(@"E:\Sample.txt");
                //File.WriteAllText(@"E:\Sample.txt",sb.ToString());
                int i = 0;
                foreach (ListViewRow row in listView.Rows)
                {
                    if (i == 0)
                    {
                        foreach (var cell in row.Cells)
                        {
                            if (cell.Text == cellText)
                            {
                                row.DoubleClick();
                                return;
                            }
                        }
                        i = 1;
                    }
                    else
                        i = 0;
                }
            }
        }
    }
}
