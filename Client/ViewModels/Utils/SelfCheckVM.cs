using Client.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excelra.Utils.Library;

namespace Client.ViewModels.Utils
{
    public class SelfCheckVM : ViewModelBase
    {
        private string serverURL, user, system, s8500Count, s9000Count, sharePath, regexCount, cvtCount, freeTextCount;
        private string solventsBoilingPointCount;

        public string ServerURL { get { return serverURL; } set { SetProperty(ref serverURL, value); } }
        public string User { get { return user; } set { SetProperty(ref user, value); } }
        public string System { get { return system; } set { SetProperty(ref system, value); } }
        public string S8500Count { get { return s8500Count; } set { SetProperty(ref s8500Count, value); } }
        public string S9000Count { get { return s9000Count; } set { SetProperty(ref s9000Count, value); } }
        public string SharePath { get { return sharePath; } set { SetProperty(ref sharePath, value); } }
        public string RegexCount { get { return regexCount; } set { SetProperty(ref regexCount, value); } }
        public string CvtCount { get { return cvtCount; } set { SetProperty(ref cvtCount, value); } }
        public string FreeTextCount { get { return freeTextCount; } set { SetProperty(ref freeTextCount, value); } }//SolventsBoilingPointCount
        public string SolventsBoilingPointCount { get { return solventsBoilingPointCount; } set { SetProperty(ref solventsBoilingPointCount, value); } }//

        public string S8000Tool
        {
            get
            {
                try
                {
                    var acceNames = String.Join(",", SystemUtils.GetInstalledSoftwares().Where(s => s.Contains(S.ACCE_DRAW)));
                    return String.IsNullOrEmpty(acceNames) ? S.NOT_FOUND : acceNames;
                }
                catch (Exception ex)
                {
                    return "Error";
                }
            }
        }

        public SelfCheckVM()
        {
            S.DataLoadingComplete += S_DataLoadingComplete;
        }

        public void LoadData()
        {
            System = Environment.MachineName;
            User = Environment.UserName;
            ServerURL = C.BASE_URL;
            if (S.CommentDictionary != null && S.CommentDictionary.CVT != null && S.CommentDictionary.FreeText != null)
            {
                CvtCount = S.CommentDictionary.CVT.Count.ToString();
                FreeTextCount = S.CommentDictionary.FreeText.Count.ToString();
            }
            else
            {
                CvtCount = S.NOT_LOADED_YET;
                FreeTextCount = S.NOT_LOADED_YET;
            }
            if (S.AllSeriesNames != null)
            {
                S8500Count = S.AllSeriesNames.Where(chem => chem.ChemicalType == DTO.ChemicalType.S8500).Count().ToString();
                S9000Count = S.AllSeriesNames.Where(chem => chem.ChemicalType == DTO.ChemicalType.S9000).Count().ToString();
            }
            else
            {
                S8500Count = S.NOT_LOADED_YET;
                S9000Count = S.NOT_LOADED_YET;
            }
            if (S.RegularExpressions != null)
                RegexCount = S.RegularExpressions.Count.ToString();
            else
                RegexCount = S.NOT_LOADED_YET;
            if (S.SolventBoilingPoints != null)
                SolventsBoilingPointCount = S.SolventBoilingPoints.Count.ToString();
            else
                SolventsBoilingPointCount = S.NOT_LOADED_YET;
            SharePath = C.MolImagesFolder;
        }

        private void S_DataLoadingComplete(object sender, bool e)
        {
            LoadData();
        }
    }
}
