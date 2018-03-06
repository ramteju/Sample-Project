using Client.Models;
using Client.Util;
using DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client.ViewModels
{
    public class NUMVM : ViewModelBase
    {
        public NUMVM()
        {
            Images = new ObservableCollection<ViewModels.ImagesVM>();
        }
        private String name, chemicalName, regNumber, aBSSterio, peptideSequence, nuclicAcidSequence, otherName,csh;
        private int num;
        private ChemicalType chemicalType;
        private TanChemicalVM chemical;
        private BitmapImage chemicalBitMapImage;
        private ObservableCollection<ImagesVM> images;
        public String Name { get { return name; } set { SetProperty(ref name, value); } }
        public String Formula { get { return chemicalName; } set { SetProperty(ref chemicalName, value); } }
        public String RegNumber { get { return regNumber; } set { SetProperty(ref regNumber, value); } }
        public int Num { get { return num; } set { SetProperty(ref num, value); } }
        public ChemicalType ChemicalType { get { return chemicalType; } set { SetProperty(ref chemicalType, value); } }
        public string ABSSterio { get { return aBSSterio; } set { SetProperty(ref aBSSterio, value); } }
        public string PeptideSequence { get { return peptideSequence; } set { SetProperty(ref peptideSequence, value); } }
        public string NuclicAcidSequence { get { return nuclicAcidSequence; } set { SetProperty(ref nuclicAcidSequence, value); } }
        public string OtherName { get { return otherName; } set { SetProperty(ref otherName, value); } }
        public string CSH { get { return csh; } set { SetProperty(ref csh, value); } }
        public TanChemicalVM Chemical
        {
            get { return chemical; }
            set
            {
                SetProperty(ref chemical, value);
            }
        }
        public BitmapImage ChemicalBitMapImage
        {
            get
            {
                return Chemical.Image;
            }
        }
        public ObservableCollection<ImagesVM> Images { get { return images; } set { SetProperty(ref images, value); } }
    }

    public class ImagesVM : ViewModelBase
    {
        private ChemicalType chemicalType;
        private string imagePath;
        private string regNumber;
        private string molString;
        public BitmapImage Image
        {
            get
            {
                return ImageUtil.GetImage(ChemicalType, ImagePath, RegNumber, MolString);
            }
        }
        public BitmapImage BigImage
        {
            get
            {
                return ImageUtil.GetImage(ChemicalType, ImagePath, RegNumber, MolString, true);
            }
        }

        public iTextSharp.text.Image ITextImage
        {
            get
            {
                return ImageUtil.GetItextImageFromPath(ChemicalType,ImagePath);
            }
        }

        public ChemicalType ChemicalType { get { return chemicalType; } set { SetProperty(ref chemicalType, value); } }
        public string ImagePath { get { return imagePath; } set { SetProperty(ref imagePath, value); } }
        public string RegNumber { get { return regNumber; } set { SetProperty(ref regNumber, value); } }
        public string MolString { get { return molString; } set { SetProperty(ref molString, value); } }

    }
}
