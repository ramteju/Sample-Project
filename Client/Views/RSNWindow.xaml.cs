using Client.Common;
using Client.Logging;
using Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for RSNDialog.xaml
    /// </summary>
    public partial class RSNDialog : Window
    {
        public bool DialogStatus;
        public RSNDialog()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if(App.Current.MainWindow !=null && (App.Current.MainWindow as MainWindow).DataContext as MainVM != null)
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).IsRsnWindowOpened = false;
            Hide();
        }

        public void SetData(Tuple<List<RsnVM>, ReactionVM, StageVM> tuple)
        {
            try
            {
                if (this.DataContext != null)
                {
                    var rsnWindowVM = (this.DataContext as RSNWindowVM);
                    rsnWindowVM.ClearEditForm.Execute(this);
                    rsnWindowVM.ReactionVM = tuple.Item2;
                    rsnWindowVM.StageVM = tuple.Item3;
                    rsnWindowVM.RsnLevelEnable = (tuple.Item3 != null && tuple.Item3.DisplayOrder > 1) ? false : true;
                    rsnWindowVM.SetRsns(tuple.Item1);
                    rsnWindowVM.UpdateRSNView();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DialogStatus = false;
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).IsRsnWindowOpened = false;
                Hide();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private void SubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rsnWindowVM = this.DataContext as RSNWindowVM;
                if (rsnWindowVM != null && rsnWindowVM.Rsns != null && rsnWindowVM.Rsns.Count > 0 && rsnWindowVM.Rsns.Select(rsn => (!string.IsNullOrEmpty(rsn.CvtText) ? rsn.CvtText.Length : 0) + (!string.IsNullOrEmpty(rsn.FreeText) ? rsn.FreeText.Length : 0)).Sum() > 500)
                {
                    DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("RSN Length Exceeds 500 characters. You want to Still submit the Data", "Reactions", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        this.DialogStatus = true;
                        //DialogResult = true;
                        return;
                    }
                    else
                        return;
                }
                else
                {
                    this.DialogStatus = true;
                    //DialogResult = true;
                }
                var mainVM = ((MainWindow)(App.Current.MainWindow)).DataContext as MainVM;
                List<RsnVM> rsns = new List<RsnVM>();
                foreach (var rsnVM in mainVM.TanVM.Rsns)
                    rsns.Add(new RsnVM(rsnVM));
                if (this.DialogStatus)
                {
                    if (rsnWindowVM != null)
                    {
                        var otherRsns = rsns.Where(rsn => rsn.Reaction.Id != mainVM.TanVM.SelectedReaction.Id).ToList();
                        if (rsnWindowVM.Rsns != null)
                            foreach (var rsn in rsnWindowVM.Rsns)
                                otherRsns.Add(new RsnVM(rsn));
                        mainVM.TanVM.SetRsns(new System.Collections.ObjectModel.ObservableCollection<RsnVM>(otherRsns));
                        rsnWindowVM.ResetWindow();
                        mainVM.TanVM.UpdateReactionPreview();
                        mainVM.TanVM.PerformAutoSave("Rsn Added/Updated");
                        mainVM.Validate(true);
                        if (U.RoleId == 1)
                        {
                            mainVM.TanVM.SelectedReaction.LastupdatedDate = DateTime.Now;
                            mainVM.TanVM.SelectedReaction.IsCurationCompleted = false;
                        }
                        else if (U.RoleId == 2)
                        {
                            mainVM.TanVM.SelectedReaction.ReviewLastupdatedDate = DateTime.Now;
                            mainVM.TanVM.SelectedReaction.IsReviewCompleted = false;
                        }
                        else if (U.RoleId == 3)
                            mainVM.TanVM.SelectedReaction.QCLastupdatedDate = DateTime.Now;
                    }
                }
                mainVM.IsRsnWindowOpened = false;
                Hide();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        public void show(TanVM tanVM)
        {
            var RSNWindowVM = this.DataContext as RSNWindowVM;
            if (RSNWindowVM != null && tanVM!=null && tanVM.SelectedReaction!=null)
            {

                RSNWindowVM.RSNTitle = $"{S.RSNTitle} - {tanVM.SelectedReaction.DisplayName}";
                ((App.Current.MainWindow as MainWindow).DataContext as MainVM).IsRsnWindowOpened = true;
                this.Height = 650;
                this.Width = 780;
                this.Show();
                this.Focus();
            }
        }

        private void TextBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var RSNWindowVM = this.DataContext as RSNWindowVM;
            if (RSNWindowVM != null)
            {
                if (RSNWindowVM.FreeTextMode == FreeTextMode.APPEND)
                {
                    System.Windows.Forms.MessageBox.Show("You cant add manual text in Append Mode. If you want to add text Please switch to Replace Mode.", "Reactions", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
                    txtFreetext.Text = RSNWindowVM.FreeText;
                }
            }
        }
    }
}
