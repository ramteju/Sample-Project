using System.Collections;

namespace Client.ViewModels
{
    public class RsnsComparater : IComparer
    {
        public int Compare(object x, object y)
        {
            int result;
            var px = x as RsnVM;
            var py = y as RsnVM;

            result = px.Reaction.Id.CompareTo(py.Reaction.Id);
            return result;
        }
    }
}
