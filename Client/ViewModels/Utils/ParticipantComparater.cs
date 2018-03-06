using System.Collections;
using System.Diagnostics;

namespace Client.ViewModels
{
    public class ParticipantComparater : IComparer
    {
        public int Compare(object x, object y)
        {
            int result;
            var px = x as ReactionParticipantVM;
            var py = y as ReactionParticipantVM;

            result = px.ReactionVM.DisplayOrder.CompareTo(py.ReactionVM.DisplayOrder);
            if (result == 0 && px.StageVM != null && py.StageVM != null)
                result = px.StageVM.DisplayOrder.CompareTo(py.StageVM.DisplayOrder);
            if (result == 0)
                result = px.ParticipantType.CompareTo(py.ParticipantType);
            if (result == 0)
                result = px.DisplayOrder.CompareTo(py.DisplayOrder);
            if (result == -1)
            {

            }
            return result;
        }
    }
}
