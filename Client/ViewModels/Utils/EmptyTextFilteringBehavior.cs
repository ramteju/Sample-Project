using Client.Common;
using Client.Logging;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace Client.ViewModels
{
    public class EmptyTextFilteringBehavior : FilteringBehavior
    {
        public override IEnumerable<object> FindMatchingItems(string searchText, System.Collections.IList items, IEnumerable<object> escapedItems, string textSearchPath, TextSearchMode textSearchMode)
        {
            try
            {
                var lastText = searchText.Split(',').LastOrDefault();
                var matches = base.FindMatchingItems(lastText, items, escapedItems, textSearchPath, textSearchMode);
                if (string.IsNullOrWhiteSpace(lastText))
                    return items.OfType<object>().Where(x => !escapedItems.Contains(x));
                return matches;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }
    }
}
