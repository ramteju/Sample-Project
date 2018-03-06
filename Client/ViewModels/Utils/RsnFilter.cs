using Client.Common;
using Client.Logging;
using Entities;

namespace Client.ViewModels.Utils
{
    public static class RsnFilter
    {
        public static bool Filter(object obj)
        {

            try
            {
                if (obj != null && obj is RsnVM)
                {
                    RsnVM rsnVM = obj as RsnVM;
                    var mainVM = ((App.Current.MainWindow as MainWindow).DataContext as MainVM);
                    if (mainVM != null && mainVM.TanVM != null)
                    {
                        var selectedRxn = mainVM.TanVM.SelectedReaction;
                        if (selectedRxn != null && selectedRxn.SelectedStage != null && rsnVM.Reaction.Id == selectedRxn.Id)
                        {
                            if (selectedRxn.SelectedStage.DisplayOrder == 1)
                                return (rsnVM.Stage == null || rsnVM.Stage.DisplayOrder == 1);
                            else if (selectedRxn.SelectedStage.DisplayOrder > 1)
                                return rsnVM.Stage != null && rsnVM.Stage.DisplayOrder == selectedRxn.SelectedStage.DisplayOrder;
                        }
                    }
                }
                return false;
            }
            catch (System.Exception ex)
            {
                Log.This(ex);
                return false;
            }
        }
    }
}
