using Client.Common;
using Client.Logging;
using Client.ViewModels;
using Client.Views;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Export
{
    public static class NumsToPdf
    {
        #region Pdf Color settings

        static BaseColor bgcolNUMInfo = new BaseColor(204, 255, 204);
        static BaseColor bgDetails = new BaseColor(255, 239, 213);

        static private iTextSharp.text.Font fontTinyItalic = FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL);

        #endregion

        #region Public Properties
        static public string PdfFilePath { get; set; }
        public static List<NUMVM> Nums { get; set; }
        #endregion

        static StyleSheet styles;

        public static bool ExportTanNUMsToPDF()
        {
            bool blStatus = false;
            Document objPdf = null;

            try
            {
                if (Nums != null)
                {
                    objPdf = new Document();
                    objPdf.Open();

                    //Define font style
                    #region MyRegion
                    styles = new StyleSheet();
                    styles.LoadTagStyle("th", "size", "8px");
                    styles.LoadTagStyle("th", "face", "helvetica");
                    styles.LoadTagStyle("span", "size", "7px");
                    styles.LoadTagStyle("span", "face", "helvetica");
                    styles.LoadTagStyle("td", "size", "7px");
                    styles.LoadTagStyle("td", "face", "helvetica");
                    #endregion

                    ShowMessages("Creating Pdf. You can continue with your Work. Will Let you know once done.");

                    Task.Run(() =>
                    {
                        if (Nums.Any())
                        {


                            string strNUM = "";

                            objPdf = new Document();
                            PdfWriter writer = PdfWriter.GetInstance(objPdf, new FileStream(PdfFilePath, FileMode.Create));

                            writer.SetFullCompression();
                            writer.StrictImageSequence = true;
                            writer.SetLinearPageMode();

                            objPdf.Open();

                            Paragraph objPara = null;

                            iTextSharp.text.Font fntNUM = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 9f, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(163, 21, 21));
                            iTextSharp.text.Font lightblue = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 9f, iTextSharp.text.Font.NORMAL, new iTextSharp.text.BaseColor(43, 145, 175));
                            iTextSharp.text.Font courier = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.COURIER, 9f);
                            iTextSharp.text.Font georgia = FontFactory.GetFont("georgia", 10f);
                            georgia.Color = iTextSharp.text.BaseColor.GRAY;

                            float width = iTextSharp.text.PageSize.A4.Width - objPdf.LeftMargin - objPdf.RightMargin;
                            float height = iTextSharp.text.PageSize.A4.Height - objPdf.TopMargin - objPdf.BottomMargin;

                            Phrase objPhrse = null;
                            Chunk cnkNUM = null;
                            Chunk cnkNUM_Val = null;
                            Chunk cnkRegNo = null;
                            Chunk cnkRegNo_Val = null;
                            Chunk cnkForm = null;
                            Chunk cnkForm_Val = null;
                            Chunk cnkAbsStereo = null;
                            Chunk cnkAbsStereo_Val = null;
                            Chunk cnkName = null;
                            Chunk cnkName_Val = null;

                            Chunk cnkPSeq = null;
                            Chunk cnkPSeq_Val = null;

                            Chunk cnkNSeq = null;
                            Chunk cnkNSeq_Val = null;

                            Chunk cnkSyn = null;
                            Chunk cnkSyn_Val = null;

                            Paragraph emptypara = null;
                            //RegNoInfoBO objRegInfo = null;


                            foreach (var num in Nums)
                            {
                                strNUM = num.Num.ToString();
                                try
                                {
                                    if (num != null)
                                    {
                                        objPhrse = new Phrase();
                                        cnkNUM = new Chunk("NUM: ", georgia);
                                        cnkNUM_Val = new Chunk(num.Num.ToString(), FontFactory.GetFont("Arial", 10, 3, iTextSharp.text.BaseColor.RED));
                                        cnkRegNo = new Chunk("\r\nRegistry No: ", georgia);
                                        cnkRegNo_Val = new Chunk(num.RegNumber.ToString(), FontFactory.GetFont("Arial", 10, 3, iTextSharp.text.BaseColor.BLUE));
                                        cnkForm = new Chunk("\r\nFormula: ", georgia);
                                        cnkForm_Val = new Chunk(num.Formula, FontFactory.GetFont("Arial", 10, 3, iTextSharp.text.BaseColor.MAGENTA));

                                        cnkAbsStereo = new Chunk("\r\nAbsolute Stereo: ", georgia);
                                        cnkAbsStereo_Val = new Chunk(num.ABSSterio, FontFactory.GetFont("Arial", 10, 3, iTextSharp.text.BaseColor.ORANGE));

                                        cnkName = new Chunk("\r\nName: ", georgia);
                                        cnkName_Val = new Chunk(num.Name, FontFactory.GetFont("Arial", 9, 3, iTextSharp.text.BaseColor.BLUE));

                                        //New code for PSEQ, NSEQ on 26th Nov 2013
                                        cnkPSeq = new Chunk("\r\nPeptide Sequence: ", georgia);
                                        cnkPSeq_Val = new Chunk(num.PeptideSequence, FontFactory.GetFont("Arial", 9, 3, iTextSharp.text.BaseColor.GREEN));

                                        cnkNSeq = new Chunk("\r\nNeuclic Acid Sequence: ", georgia);
                                        cnkNSeq_Val = new Chunk(num.NuclicAcidSequence, FontFactory.GetFont("Arial", 9, 3, iTextSharp.text.BaseColor.GREEN));

                                        cnkSyn = new Chunk("\r\nOther Names: \r\n", georgia);
                                        cnkSyn_Val = new Chunk(num.OtherName.Replace("\r\n", ", "), FontFactory.GetFont("Arial", 9, 3, iTextSharp.text.BaseColor.BLUE));

                                        objPhrse.Add(cnkNUM);
                                        objPhrse.Add(cnkNUM_Val);
                                        objPhrse.Add(cnkRegNo);
                                        objPhrse.Add(cnkRegNo_Val);
                                        objPhrse.Add(cnkForm);
                                        objPhrse.Add(cnkForm_Val);
                                        if (cnkAbsStereo != null)
                                        {
                                            objPhrse.Add(cnkAbsStereo);
                                        }
                                        if (cnkAbsStereo_Val != null)
                                        {
                                            objPhrse.Add(cnkAbsStereo_Val);
                                        }
                                        objPhrse.Add(cnkName);
                                        objPhrse.Add(cnkName_Val);


                                        objPhrse.Add(cnkPSeq);
                                        objPhrse.Add(cnkPSeq_Val);

                                        objPhrse.Add(cnkNSeq);
                                        objPhrse.Add(cnkNSeq_Val);

                                        objPhrse.Add(cnkSyn);
                                        objPhrse.Add(cnkSyn_Val);

                                        objPara = new Paragraph();
                                        objPara.Add(objPhrse);
                                        objPdf.Add(objPara);
                                        if (num.Images != null)
                                        {
                                            foreach (var image in num.Images)
                                            {

                                                try
                                                {
                                                    iTextSharp.text.Image chemimg = image.ITextImage;
                                                    //Give space before image 
                                                    chemimg.SpacingBefore = 30f;
                                                    //Give some space after the image 
                                                    chemimg.SpacingAfter = 1f;
                                                    chemimg.Alignment = iTextSharp.text.Image.ALIGN_CENTER;
                                                    objPdf.Add(chemimg);
                                                }
                                                catch
                                                {
                                                    objPdf.Add(new Chunk("Structure Error", FontFactory.GetFont("Arial", 10, 3, iTextSharp.text.BaseColor.RED)));
                                                }
                                            }
                                        }
                                        emptypara = new Paragraph("-------------------------------------------------------------------------------------------------------------------------------------------------------\r\n\r\n", georgia);
                                        objPdf.Add(emptypara);
                                    }
                                }
                                catch
                                {

                                }
                            }

                            objPdf.Close();
                            blStatus = true;
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                ShowMessages("Nums Exported to Pdf Successfully.");
                            });
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return blStatus;
        }
        public static void ShowMessages(string msg)
        {
            AppInfoBox.ShowInfoMessage(msg);
        }
    }
}
