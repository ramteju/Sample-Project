using Client.Logging;
using Client.ViewModels;
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for ParticipantsDisplay.xaml
    /// </summary>
    public partial class ParticipantsDisplay : UserControl
    {
        public ParticipantsDisplay()
        {
            InitializeComponent();
        }

        private void Participant_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount == 2)
                {
                    try
                    {
                        BindingExpression bindingExpression = (sender as TextBlock).GetBindingExpression(TextBlock.TextProperty);
                        var participant = ((bindingExpression.Target as TextBlock).DataContext as ReactionParticipantVM);
                        (App.Current.MainWindow as MainWindow).SelectParticipant(participant);
                    }
                    catch (Exception ex)
                    {
                        Log.This(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
