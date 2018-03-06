using Client.Logging;
using Client.Util;
using Client.ViewModels;
using Client.XML;
using DTO;
using Excelra.Utils.Library;
using ProductTracking.Models.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Client.Models
{
    public enum StructureType
    {
        CGM,
        MOL
    }


    public class TanChemicalVM : ViewModelBase
    {
        private Guid id;
        private int num;
        private string reg, name, searchName, inchiKey, imagePath, molString,csh,stereoChemisrty;
        private StructureType structureType;
        private ChemicalType chemicalType;
        private ObservableCollection<ImagesVM> images;

        public Guid Id { get { return id; } set { SetProperty(ref id, value); } }
        public int NUM { get { return num; } set { SetProperty(ref num, value); } }
        public string RegNumber { get { return reg; } set { SetProperty(ref reg, value); } }
        public string Name { get { return name; } set { SetProperty(ref name, value); } }
        public string SearchName { get { return searchName; } set { SetProperty(ref searchName, value); } }
        public ChemicalType ChemicalType { get { return chemicalType; } set { SetProperty(ref chemicalType, value); } }
        public string InChiKey { get { return inchiKey; } set { SetProperty(ref inchiKey, value); } }
        public string ImagePath { get { return imagePath; } set { SetProperty(ref imagePath, value); } }

        public List<string> AllImagePaths;
        private string molFormula;

        public StructureType StructureType { get { return structureType; } set { SetProperty(ref structureType, value); } }
        public string CompoundNo { get; set; }
        public string GenericName { get; set; }
        public string MolString { get { return molString; } set { SetProperty(ref molString, value); } }
        public string MolFormula { get { return molFormula; } set { SetProperty(ref molFormula, value); } }
        public string CSH { get { return csh; } set { SetProperty(ref csh, value); } }
        public string StereoChemisrty { get { return stereoChemisrty; } set { SetProperty(ref stereoChemisrty, value); } }
        public List<TanChemicalMetaData> ChemicalmataData { get; set; }
        public BitmapImage Image
        {
            get
            {
                if (ChemicalType == ChemicalType.NUM)
                    return Images.FirstOrDefault()?.Image;
                return ImageUtil.GetImage(ChemicalType, ImagePath, RegNumber, MolString);
            }
        }
        public ObservableCollection<ImagesVM> Images
        {
            get
            {
                var images = new ObservableCollection<ImagesVM> ();
                foreach (var path in AllImagePaths)
                {
                    images.Add(new ImagesVM { ChemicalType = ChemicalType, ImagePath = path, MolString = MolString, RegNumber = RegNumber });
                }
                if (ChemicalType != ChemicalType.NUM)
                {
                    images.Add(new ImagesVM { ChemicalType = ChemicalType,ImagePath = ImagePath,MolString = MolString,RegNumber = RegNumber});
                }
                return images;
            }
            set { SetProperty(ref images, value); }
        }

        public BitmapImage BigImage
        {
            get
            {
                return ImageUtil.GetImage(ChemicalType, ImagePath, RegNumber, MolString, true);
            }
        }

        public TanChemicalVM()
        {
            AllImagePaths = new List<string>();
        }
        public TanChemicalVM(TanChemicalVM another)
        {
            this.NUM = another.NUM;
            this.RegNumber = another.RegNumber;
            this.Name = another.Name;
            //not all are copied here
        }
        public string PageInfo
        {
            get
            {
                if (ChemicalmataData!=null && ChemicalmataData.Count > 0)
                    return ChemicalmataData.Select(cm => $"On page {cm.PageNo}").FirstOrDefault();
                return string.Empty;
            }
        }

        public string GetDuplicateChemicalString(string inchiKey)
        {
            StringBuilder message = new StringBuilder();
            message.Append("This Chemical Matches With :");
            message.Append(Environment.NewLine);
            message.Append("Name : ");
            message.Append(this.Name);
            message.Append(Environment.NewLine);
            message.Append("NUM : ");
            message.Append(this.NUM);
            message.Append(Environment.NewLine);
            message.Append("REG : ");
            message.Append(this.RegNumber);
            message.Append(Environment.NewLine);
            message.Append("Series : ");
            message.Append(this.ChemicalType.DescriptionAttribute());
            message.Append(Environment.NewLine);
            message.Append("Source InChi Key : ");
            message.Append(inchiKey);
            message.Append(Environment.NewLine);
            message.Append("Matched InChi Key : ");
            message.Append(this.InChiKey);
            return message.ToString();
        }
    }

    public class ChemicalNameComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            try
            {
                var cx = x as TanChemicalVM;
                var cy = y as TanChemicalVM;
                int result = cx.ChemicalType.CompareTo(cy.ChemicalType);
                if (result == 0)
                    result = (cx.NUM == 0 || cy.NUM == 0) ? -cx.NUM.CompareTo(cy.NUM) : cx.NUM.CompareTo(cy.NUM);
                return result;
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return 0;
        }
    }
}
