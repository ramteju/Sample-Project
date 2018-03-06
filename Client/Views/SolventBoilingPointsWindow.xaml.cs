using Client.Common;
using Client.Logging;
using Client.ViewModel;
using Client.ViewModels.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for SolventBoilingPointsWindow.xaml
    /// </summary>
    public partial class SolventBoilingPointsWindow : Window
    {
        static private SolventBoilingPointsWindow thisWindow;
        static private SolventBoilingPointsVM thisWindowVM;
        static SolventBoilingPointsWindow()
        {
            
        }
        public SolventBoilingPointsWindow()
        {
            InitializeComponent();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
        public static void ShowSolventBoilingPoints()
        {
            try
            {
                if (thisWindow==null)
                {
                    thisWindow = new Views.SolventBoilingPointsWindow();
                    thisWindowVM = new SolventBoilingPointsVM();
                    thisWindow.DataContext = thisWindowVM;
                    if (S.SolventBoilingPoints.Any())
                    {
                        foreach (var solvent in S.SolventBoilingPoints)
                        {
                            thisWindowVM.BoilingPoints.Add(new BoilingPointsVM
                            {
                                RegNo = solvent.RegNo,
                                DegreesBoilingPoint = solvent.DegreesBoilingPoint,
                                KelvinBoilingPoint = solvent.KelvinBoilingPoint,
                                FahrenheitBoilingPoint = solvent.fahrenheitBoilingPoint,
                                RankineBoilingPoint = solvent.RankineBoilingPoint,
                                Name = solvent.Name
                            });
                        }
                        thisWindowVM.BoilingPoints.UpdateDisplayOrder();
                    }
                }
                thisWindow.Show();
                thisWindow.Focus();
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }
}
