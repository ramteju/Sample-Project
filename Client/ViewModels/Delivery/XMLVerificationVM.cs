using Client.Command;
using Client.Common;
using Client.Logging;
using Client.ViewModels.Delivery;
using Client.Views;
using Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Client.ViewModels
{
    public class XMLVerificationVM : ViewModelBase
    {

        static private OpenFileDialog openFileDialog = new OpenFileDialog();
        private static string XSD_PATH = AppDomain.CurrentDomain.BaseDirectory + "CORE_Input_Schema_List.xsd"; //"CAS_React_Schema.xsd";

        private string xmlPath;
        private int s8000Chemicals;
        private bool isBusy;
        private int totalTans, totalRXNs;
        private int? maxRSDLength, maxRSNLength;
        private ObservableCollection<ValidateRSDVM> rSDResult;
        private ObservableCollection<ValidateXSDVM> xSDResult;
        private ObservableCollection<UnicodeErrorVM> commentsUnicodeErrors, rSNUnicodeErrors, s8000UnicodeErrors;

        public string XmlPath
        {
            get { return xmlPath; }
            set
            {
                IsBusy = true;
                SetProperty(ref xmlPath, value);
                IsBusy = false;
            }
        }
        public int S8000Chemicals { get { return s8000Chemicals; } set { SetProperty(ref s8000Chemicals, value); } }
        public bool IsBusy { get { return isBusy; } set { SetProperty(ref isBusy, value); } }
        public int TotalTans { get { return totalTans; } set { SetProperty(ref totalTans, value); } }
        public int TotalRXNs { get { return totalRXNs; } set { SetProperty(ref totalRXNs, value); } }
        public int? MaxRSDLength { get { return maxRSDLength; } set { SetProperty(ref maxRSDLength, value); } }
        public int? MaxRSNLength { get { return maxRSNLength; } set { SetProperty(ref maxRSNLength, value); } }
        public ObservableCollection<ValidateRSDVM> RSDResult { get { return rSDResult; } set { SetProperty(ref rSDResult, value); } }
        public ObservableCollection<ValidateXSDVM> XSDResult { get { return xSDResult; } set { SetProperty(ref xSDResult, value); } }
        public ObservableCollection<UnicodeErrorVM> CommentsUnicodeErrors { get { return commentsUnicodeErrors; } set { SetProperty(ref commentsUnicodeErrors, value); } }
        public ObservableCollection<UnicodeErrorVM> RSNUnicodeErrors { get { return rSNUnicodeErrors; } set { SetProperty(ref rSNUnicodeErrors, value); } }
        public ObservableCollection<UnicodeErrorVM> S8000UnicodeErrors { get { return s8000UnicodeErrors; } set { SetProperty(ref s8000UnicodeErrors, value); } }
        public DelegateCommand ValidateXML { get; private set; }
        public DelegateCommand BrowseXML { get; private set; }
        public DelegateCommand ClearForm { get; private set; }

        internal void ClearState()
        {

            try
            {
                ;
                XmlPath = String.Empty;
                TotalTans = 0;
                TotalRXNs = 0;
                MaxRSDLength = 0;
                S8000Chemicals = 0;
                CommentsUnicodeErrors = null;
                RSNUnicodeErrors = null;
                S8000UnicodeErrors = null;
                XSDResult = null;
                RSDResult = null;
                IsBusy = false;
                ;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        public XMLVerificationVM()
        {
            IsBusy = false;
            ValidateXML = new DelegateCommand(DoValidateXML);
            BrowseXML = new DelegateCommand(DoBrowseXML);
            ClearForm = new DelegateCommand(DoClearForm);

            openFileDialog.DefaultExt = ".xml";
            openFileDialog.Filter = "XML Documents (.xml)|*.xml";
        }

        private void DoClearForm(object obj)
        {

            try
            {
                ClearState();
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void DoBrowseXML(object obj)
        {

            try
            {
                ClearState();
                if (openFileDialog.ShowDialog() == true)
                {
                    XmlPath = openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void DoValidateXML(object obj)
        {

            try
            {
                if (!File.Exists(XSD_PATH))
                {
                    MessageBox.Show("XSD File Not Found !");
                    return;
                }
                if (File.Exists(XmlPath))
                {
                    try
                    {
                        VerifyXML(XmlPath);
                    }
                    catch (Exception ex)
                    {
                        AppErrorBox.ShowErrorMessage("Error While Verifying XML . .", ex.ToString());
                    }
                    try
                    {
                        using (var reader = new XmlTextReader(XmlPath))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(xsd.RXNFILE));
                            xsd.RXNFILE rxnFile = (xsd.RXNFILE)serializer.Deserialize(reader);
                            CollectSummary(rxnFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppErrorBox.ShowErrorMessage("Error While Collecting Information From XML . .", ex.ToString());
                    }
                }
                else
                    MessageBox.Show("XML File Path Is Not Valid . .");
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void VerifyXML(string fileName)
        {

            try
            {
                XSDResult = new ObservableCollection<ValidateXSDVM>();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.ValidationType = ValidationType.Schema;
                settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings;
                settings.Schemas.Add(null, XmlReader.Create(XSD_PATH));
                settings.ValidationEventHandler += new ValidationEventHandler(XmlErrorHandler);
                XmlReader reader = XmlReader.Create(fileName, settings);
                while (reader.Read()) ;
            }
            catch (Exception ex)
            {
                AppErrorBox.ShowErrorMessage("Error While Verifying XML . .", ex.ToString());
                Log.This(ex);

            }
        }

        private void XmlErrorHandler(object sender, ValidationEventArgs e)
        {

            try
            {
                XSDResult.Add(new ValidateXSDVM
                {
                    Level = e.Severity.ToString(),
                    Text = e.Message,
                    LineNumber = e.Exception.LineNumber,
                    LinePosition = e.Exception.LinePosition
                });
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }

        private void CollectSummary(xsd.RXNFILE rxnFile)
        {
            try
            {
                int rxnsCount = 0;
                S8000Chemicals = 0;
                List<ValidateRSDVM> rsdData = new List<ValidateRSDVM>();

                CommentsUnicodeErrors = new ObservableCollection<UnicodeErrorVM>();
                RSNUnicodeErrors = new ObservableCollection<UnicodeErrorVM>();
                S8000UnicodeErrors = new ObservableCollection<UnicodeErrorVM>();

                if (rxnFile.DOCUMENT != null)
                {
                    TotalTans = rxnFile.DOCUMENT.Count();

                    foreach (var tan in rxnFile.DOCUMENT)
                    {
                        if (tan.RXNGRP != null)
                        {
                            if (tan.RXNGRP.RXN != null)
                            {
                                rxnsCount += tan.RXNGRP.RXN.Count();

                                foreach (var rxn in tan.RXNGRP.RXN)
                                {
                                    #region 8000 Chemical Name Unicode Check
                                    if (rxn.SUBDESC != null && rxn.SUBDESC.SUBDEFN != null)
                                    {
                                        S8000Chemicals += rxn.SUBDESC.SUBDEFN.Length;
                                        foreach (var s8000Definition in rxn.SUBDESC.SUBDEFN)
                                        {
                                            if (CharUtils.NotAValidUniCodeString(s8000Definition.SUBNAME))
                                                S8000UnicodeErrors.Add(new UnicodeErrorVM
                                                {
                                                    TanNumber = tan.TAN,
                                                    Num = s8000Definition.NRNNUM,
                                                    Seq = rxn.RXNID.RXNSEQ,
                                                    SubstanceName = s8000Definition.SUBNAME
                                                });
                                        }
                                    }
                                    #endregion

                                    if (rxn.RXNPROCESS.RSN != null)
                                    {
                                        foreach (var rsn in rxn.RXNPROCESS.RSN)
                                        {
                                            if (CharUtils.NotAValidUniCodeString(rsn.Value))
                                                RSNUnicodeErrors.Add(new UnicodeErrorVM
                                                {
                                                    TanNumber = tan.TAN,
                                                    RXN = rxn.NO,
                                                    Seq = rxn.RXNID.RXNSEQ,
                                                    Comments = rsn.Value
                                                });

                                        }
                                    }

                                    #region RSD Data
                                    rsdData.Add(new ValidateRSDVM
                                    {
                                        ProductNo = rxn.RXNID.RXNNUM,
                                        RSD = rxn.RSD,
                                        RsdLength = rxn.RSD.Length,
                                        RxnNo = rxn.NO,
                                        Semicolumns = rxn.RSD.Split(';').Length - 1,
                                        Sequence = rxn.RXNID.RXNSEQ,
                                        Stages = rxn.RXNPROCESS.STAGE.Count(),
                                        TanNumber = tan.TAN
                                    });
                                    #endregion
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(tan.COMMENTS) && CharUtils.NotAValidUniCodeString(tan.COMMENTS))
                            CommentsUnicodeErrors.Add(new UnicodeErrorVM { TanNumber = tan.TAN, Comments = tan.COMMENTS });
                    }
                }
                var sortedRSDs = rsdData.OrderByDescending(r => r.RsdLength);
                RSDResult = new ObservableCollection<ValidateRSDVM>(sortedRSDs);
                MaxRSDLength = sortedRSDs.FirstOrDefault()?.RsdLength;
                TotalRXNs = rxnsCount;
            }
            catch (Exception ex)
            {
                Log.This(ex);

            }
        }
    }
}
