using Client.Models;
using DTO;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.ViewModels
{
    public class ReactionParticipantViewVM : ViewModelBase
    {
        private string name, shortName, formula;
        private int num;
        private TanChemicalVM chemicalName;
        private string reg;
        private ParticipantType participantType;
        private String nextImagePath;
        private SolidColorBrush borderBrush;
        private string bgColor;
        private string tooltipText;
        private StageVM stageVM;
        private ObservableCollection<ImagesVM> images;
        public BitmapImage Image
        {
            get
            {
                return ChemicalName?.Image;
            }
        }
        public BitmapImage BigImage
        {
            get
            {
                return ChemicalName?.BigImage;
            }
        }

        public String Name { get { return name; } set { SetProperty(ref name, value); } }
        public String Formula { get { return formula; } set { SetProperty(ref formula, value); } }
        public int Num { get { return num; } set { SetProperty(ref num, value); } }
        public TanChemicalVM ChemicalName { get { return chemicalName; } set { SetProperty(ref chemicalName, value); } }
        public string ShortName { get { return shortName; } set { SetProperty(ref shortName, value); } }
        public StageVM StageVM { get { return stageVM; } set { SetProperty(ref stageVM, value); } }
        public string Reg { get { return reg; } set { SetProperty(ref reg, value); } }
        public ParticipantType ParticipantType { get { return participantType; } set { SetProperty(ref participantType, value); } }
        public String BgColor { get { return bgColor; } set { SetProperty(ref bgColor, value); } }
        public String NextImagePath { get { return nextImagePath; } set { SetProperty(ref nextImagePath, value); } }
        public SolidColorBrush BorderBrush { get { return borderBrush; } set { SetProperty(ref borderBrush, value); } }
        public string TooltipText { get { return tooltipText; } set { SetProperty(ref tooltipText, value); } }
        public ObservableCollection<ImagesVM> Images
        {
            get
            {
                return ChemicalName?.Images;
            }
        }
    }
}
