using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using IndxReactNarr;
using System.Collections;
using MarkupConverter;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using Client.ViewModels;
using Client.Logging;
using Entities.DTO;
using Client.ViewModels.Core;
using ProductTracking.Models.Core;
using Client.Util;

namespace IndxReactNarr.PdfExport
{
    public class RxnsExportToPdf
    {
        #region Constructor

        private IMarkupConverter markupConverter;
        public RxnsExportToPdf()
        {
            markupConverter = new MarkupConverter.MarkupConverter();
        }

        #endregion

        #region Public Properties

        public int TAN_ID { get; set; }
        public string TAN { get; set; }
        public string CAN { get; set; }
        public string DOI { get; set; }

        public string BatchName { get; set; }
        public string OutputFolderPath { get; set; }
        public DataTable CgmFileData { get; set; }
        public DataTable ProductsData { get; set; }
        public DataTable RxnRSN { get; set; }
        public DataTable RxnConditions { get; set; }
        public DataTable ParticipantsData { get; set; }
        public DataTable RxnParticipants { get; set; }
        public DataTable StagesData { get; set; }
        public DataTable CommentsData { get; set; }

        public DataTable Ser8000Data { get; set; }
        public DataTable Ser8500Data { get; set; }

        float rowHeight = 60f;
        StyleSheet styles;

        public string OutputFileName { get; set; }

        #endregion

        #region Pdf Color settings

        BaseColor bgcolTANInfo = new BaseColor(204, 255, 204);

        BaseColor bgcolRxnNo = new BaseColor(255, 239, 213);
        BaseColor bgcolNumSeq = new BaseColor(209, 238, 238);//(255, 215, 0);
        BaseColor bgcolPageInfo = new BaseColor(238, 245, 238);
        BaseColor bgcolProduct = new BaseColor(255, 204, 153);//(255, 215, 0);
        BaseColor bgcolReactant = new BaseColor(255, 239, 213);
        BaseColor bgcolRctNumStage = new BaseColor(255, 239, 213);
        BaseColor bgcolRxnPartpnt = new BaseColor(245, 255, 250);

        private iTextSharp.text.Font fontTinyItalic = FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL);

        #endregion

        #region Public variables for RSN terms

        string strRSN_CVT_Rxn = "";
        string strRSN_FT_Rxn = "";
        string strRSN_FT_Stage = "";

        #endregion

        #region Public variables

        PdfPCell pcInfo = null;
        PdfPCell pcCmntsHdr = null;
        PdfPCell pcComments = null;
        PdfPCell pcTAN = null;
        PdfPCell pcCAN = null;
        PdfPCell pcBatch = null;
        PdfPCell pcNum = null;
        PdfPCell prdYieldCell = null;

        #endregion

        MDL.Draw.Renditor.Renditor ChemRenditor = new MDL.Draw.Renditor.Renditor();

        public bool ExportTANReactionsToPDF(TanVM tanVM)
        {
            bool blStatus = false;
            try
            {
                if (!string.IsNullOrEmpty(TAN) && !string.IsNullOrEmpty(BatchName) && !string.IsNullOrEmpty(OutputFolderPath))
                {
                    string strOutFileName = OutputFileName;//OutputFolderPath + "\\" + TAN + "_Reactions.pdf";

                    //Products, Participants, Reactants, RSN, Coditions and Stages on TAN
                    GetProductsPartpntsCondsOnTAN(TAN_ID);

                    //Get Reaction on TAN
                    DataTable dtReactions = ReactDB.GetReactionsOnTANID(TAN_ID);
                    if (dtReactions != null)
                    {
                        //if ()
                        //{
                        using (iTextSharp.text.Document doc = new iTextSharp.text.Document())
                        {
                            iTextSharp.text.pdf.PdfWriter.GetInstance(doc, new FileStream(strOutFileName, FileMode.Create));
                            doc.Open();

                            //Declare Reaction Participants tables
                            #region MyRegion
                            DataTable dtProdTbl = null;
                            DataTable dtReactantTbl = null;
                            DataTable dtPartpntTbl = null;
                            DataTable dtCondsTbl = null;
                            DataTable dtRsnTbl = null;
                            DataTable dtStagesTbl = null;
                            MDL.Draw.Renditor.Renditor objRenditor = null;
                            #endregion

                            iTextSharp.text.Image chemimg = null;
                            iTextSharp.text.Font georgia = FontFactory.GetFont("georgia", 10f);

                            //Define variables
                            #region MyRegion

                            int intRxnID = 0;
                            int intProdCnt = 0;
                            int intReactCnt = 0;

                            string strRxnHdr = "";
                            PdfPTable PdfTable = null;
                            PdfPCell rxnCell = null;
                            PdfPCell pdfCell = null;
                            PdfPTable ptNested = null;
                            #endregion

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

                            //Define Participants elements
                            List<IElement> lstPartpnt = null;
                            RichTextBox objRTB = null;
                            PdfPCell pcNumStage = null;

                            //Add Pdf Header to document
                            PdfPTable ptHeader = GetPdfHeaderTable(tanVM);
                            doc.Add(ptHeader);

                            PdfPCell prdHdrCell = null;
                            PdfPCell pcPartpnt = null;

                            int j = 0;
                            foreach (var reaction in tanVM.Reactions)
                            {
                                j++;
                                intReactCnt = tanVM.ReactionParticipants.OfReactionOfType(reaction.Id, DTO.ParticipantType.Reactant).Count();
                                intProdCnt = tanVM.ReactionParticipants.OfReactionOfType(reaction.Id, DTO.ParticipantType.Product).Count();
                                if (intReactCnt > 0 && intProdCnt > 0)
                                {
                                    PdfTable = new PdfPTable(intProdCnt + intReactCnt);
                                    PdfTable.SpacingAfter = 4f;
                                    PdfTable.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right                                
                                    PdfTable.TotalWidth = 800f;// doc.PageSize.Width;
                                    PdfTable.WidthPercentage = 100;

                                    //Add Reaction Header to Pdf Table
                                    strRxnHdr = reaction.Name;
                                    rxnCell = new PdfPCell(new Phrase(strRxnHdr, fontTinyItalic));
                                    rxnCell.Colspan = intProdCnt + intReactCnt;
                                    rxnCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;// 0; //0=Left, 1=Centre, 2=Right
                                    rxnCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                                    rxnCell.BackgroundColor = bgcolRxnNo;
                                    PdfTable.AddCell(rxnCell);
                                }
                            }


                            for (int i = 0; i < tanVM.Reactions.Count; i++)
                            {
                                
                                //Reset RSN public variables
                                strRSN_CVT_Rxn = "";
                                strRSN_FT_Rxn = "";
                                strRSN_FT_Stage = "";

                                //intRxnID = Convert.ToInt32(dtReactions.Rows[i]["RXN_ID"].ToString().Trim());

                                //Get Products & Reactants on Reaction ID
                                //dtProdTbl = GetProductDataOnReactionID(intRxnID);
                                //dtReactantTbl = GetReactantsDataOnReactionID(intRxnID);

                                //Get Products & Reactants for Reaction formation on Reaction ID
                                //dtProdTbl = GetProductsForProdFormation(dtProdTbl, intRxnID, "PRODUCT");
                                //dtReactantTbl = GetProductsForProdFormation(dtReactantTbl, intRxnID, "REACTANT"); //GetReactantsForProdFormation(dtReactantTbl);

                                intReactCnt = dtReactantTbl != null ? dtReactantTbl.Rows.Count : 0;
                                intProdCnt = dtProdTbl != null ? dtProdTbl.Rows.Count : 0;

                                if (intReactCnt > 0 && intProdCnt > 0)
                                {
                                    //Get Reaction Participants
                                    dtPartpntTbl = GetParticipantsDataOnReactionID(intRxnID);
                                    dtCondsTbl = GetConditionsDataOnReactionID(intRxnID);
                                    dtRsnTbl = GetRSNDataOnReactionID(intRxnID);
                                    dtStagesTbl = GetStagesOnReactionID(intRxnID);

                                    //Get Reaction Participants for Product formation
                                    RxnParticipants = GetParticipantsForProdFormation(dtPartpntTbl, dtCondsTbl, dtRsnTbl, dtStagesTbl);

                                    //Create instance of the pdf table and set the number of column in that table
                                    PdfTable = new PdfPTable(intProdCnt + intReactCnt);
                                    PdfTable.SpacingAfter = 4f;
                                    PdfTable.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right                                
                                    PdfTable.TotalWidth = 800f;// doc.PageSize.Width;
                                    PdfTable.WidthPercentage = 100;

                                    //Add Reaction Header to Pdf Table
                                    strRxnHdr = "Reaction - " + (i + 1).ToString() + "      Num - " + dtReactions.Rows[i]["RXN_NUM"].ToString() + " Seq " + dtReactions.Rows[i]["RXN_SEQ"].ToString();
                                    rxnCell = new PdfPCell(new Phrase(strRxnHdr, fontTinyItalic));
                                    rxnCell.Colspan = intProdCnt + intReactCnt;
                                    rxnCell.HorizontalAlignment = PdfPCell.ALIGN_LEFT;// 0; //0=Left, 1=Centre, 2=Right
                                    rxnCell.VerticalAlignment = PdfPCell.ALIGN_TOP;
                                    rxnCell.BackgroundColor = bgcolRxnNo;
                                    PdfTable.AddCell(rxnCell);

                                    //Reactants Table
                                    #region Bind Reactants to Pdf Document

                                    for (int rIndx = 0; rIndx < dtReactantTbl.Rows.Count; rIndx++)
                                    {
                                        if (dtReactantTbl.Rows[rIndx]["STRUCTURE"] != null)
                                        {
                                            iTextSharp.text.Image objStruct = dtReactantTbl.Rows[rIndx]["STRUCTURE"] as iTextSharp.text.Image;
                                            //int regno = Convert.ToInt32(dtReactantTbl.Rows[rIndx]["REG_NO"].ToString());

                                            chemimg = objStruct;// iTextSharp.text.Image.GetInstance(dtReactantTbl.Rows[rIndx]["STRUCTURE"] as Image, System.Drawing.Imaging.ImageFormat.Jpeg);//GetChemistryImageOnHexCode(strHexArr[hIndx], regno.ToString());

                                            if (chemimg != null)
                                            {
                                                chemimg.ScaleToFit(50f, 50f);
                                                chemimg.ScaleAbsolute((float)50f, (float)50f);
                                                chemimg.Alignment = iTextSharp.text.Image.TEXTWRAP | iTextSharp.text.Image.ALIGN_CENTER;

                                                pdfCell = new PdfPCell(chemimg, true);
                                            }
                                            else
                                            {

                                                pdfCell = new PdfPCell(new Phrase("Structure generation error", fontTinyItalic));
                                            }
                                            pdfCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                            pdfCell.FixedHeight = rowHeight;
                                            pdfCell.HorizontalAlignment = 1;

                                            ptNested = new PdfPTable(1);
                                            prdHdrCell = new PdfPCell(new Phrase("Reactant - " + (rIndx + 1).ToString(), fontTinyItalic));
                                            prdHdrCell.BackgroundColor = bgcolReactant;

                                            //Add Header, StageInfo and Image Cells to Table
                                            ptNested.AddCell(prdHdrCell);

                                            pcNumStage = new PdfPCell(GetNUM_StageTable(dtReactantTbl.Rows[rIndx]));
                                            pcNumStage.Border = PdfPCell.NO_BORDER;
                                            pcNumStage.BackgroundColor = bgcolRctNumStage;
                                            ptNested.AddCell(pcNumStage);

                                            ptNested.AddCell(pdfCell);
                                            PdfPCell nesthousing = new PdfPCell(ptNested);
                                            //nesthousing.Border = PdfPCell.BOX;
                                            nesthousing.Padding = 0f;
                                            nesthousing.VerticalAlignment = PdfPCell.ALIGN_TOP;
                                            PdfTable.AddCell(nesthousing);
                                        }
                                    }
                                    #endregion

                                    //Product Table
                                    #region Bind Products Data to Pdf Document

                                    for (int prodIndx = 0; prodIndx < dtProdTbl.Rows.Count; prodIndx++)
                                    {
                                        if (dtProdTbl.Rows[prodIndx]["STRUCTURE"] != null)
                                        {
                                            iTextSharp.text.Image objStruct = dtProdTbl.Rows[prodIndx]["STRUCTURE"] as iTextSharp.text.Image;
                                            //int regno = Convert.ToInt32(dtProdTbl.Rows[prodIndx]["REG_NO"].ToString());

                                            chemimg = objStruct;// iTextSharp.text.Image.GetInstance(dtReactantTbl.Rows[rIndx]["STRUCTURE"] as Image, System.Drawing.Imaging.ImageFormat.Jpeg);//GetChemistryImageOnHexCode(strHexArr[hIndx], regno.ToString());
                                            if (chemimg != null)
                                            {
                                                chemimg.ScaleToFit(50f, 50f);
                                                chemimg.ScaleAbsolute((float)50f, (float)50f);
                                                chemimg.Alignment = iTextSharp.text.Image.TEXTWRAP | iTextSharp.text.Image.ALIGN_CENTER;

                                                pdfCell = new PdfPCell(chemimg, true);
                                            }
                                            else
                                            {
                                                pdfCell = new PdfPCell(new Phrase("Structure generation error", fontTinyItalic));
                                            }

                                            if (chemimg != null)
                                            {
                                                chemimg.ScaleToFit(50f, 50f);
                                                chemimg.ScaleAbsolute((float)50f, (float)50f);
                                                chemimg.Alignment = iTextSharp.text.Image.TEXTWRAP | iTextSharp.text.Image.ALIGN_CENTER;

                                                pdfCell = new PdfPCell(chemimg, true);
                                            }
                                            else
                                            {
                                                pdfCell = new PdfPCell(new Phrase("Structure generation error", fontTinyItalic));
                                            }
                                            pdfCell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
                                            pdfCell.FixedHeight = rowHeight;
                                            pdfCell.HorizontalAlignment = 1;

                                            ptNested = new PdfPTable(1);
                                            prdHdrCell = new PdfPCell(new Phrase("Product - " + (prodIndx + 1).ToString(), fontTinyItalic));
                                            prdHdrCell.BackgroundColor = bgcolProduct;

                                            //Add Header, StageInfo and Image Cells to Table
                                            ptNested.AddCell(prdHdrCell);

                                            pcNumStage = new PdfPCell(GetProductNUMTable(dtProdTbl.Rows[prodIndx]));
                                            pcNumStage.Border = PdfPCell.NO_BORDER;
                                            pcNumStage.BackgroundColor = bgcolRctNumStage;
                                            ptNested.AddCell(pcNumStage);

                                            ptNested.AddCell(pdfCell);
                                            PdfPCell nesthousing = new PdfPCell(ptNested);
                                            //nesthousing.Border = PdfPCell.BOX;

                                            nesthousing.Padding = 0f;
                                            nesthousing.VerticalAlignment = PdfPCell.ALIGN_TOP;
                                            PdfTable.AddCell(nesthousing);
                                        }
                                    }
                                    #endregion

                                    //Get Participatns text from RichTextBox object
                                    objRTB = GetPartpntsDataBindToRichTextBox();

                                    //iTextSharp.text.html.simpleparser.HTMLWorker hw = new iTextSharp.text.html.simpleparser.HTMLWorker(doc);
                                    //hw.Parse(new StringReader(markupConverter.ConvertRtfToHtml(objRTB.Rtf)));                                                                

                                    //Get Participants elements from RichTextBox Rtf and convert to HTML
                                    lstPartpnt = HTMLWorker.ParseToList(new StringReader(markupConverter.ConvertRtfToHtml(objRTB.Rtf)), styles);

                                    //Add Participants string in a row cell to Pdf Table
                                    pcPartpnt = new PdfPCell();
                                    foreach (IElement iEle in lstPartpnt)
                                    {
                                        pcPartpnt.AddElement(iEle);
                                    }
                                    pcPartpnt.Colspan = intProdCnt + intReactCnt;
                                    //pcPartpnt.BackgroundColor =  bgcolRxnPartpnt;
                                    pcPartpnt.VerticalAlignment = Element.ALIGN_TOP;
                                    pcPartpnt.HorizontalAlignment = 0;// Element.ALIGN_LEFT; //0=Left, 1=Centre, 2=Right                                   
                                    PdfTable.AddCell(pcPartpnt);

                                    //Add Pdf Table to Pdf Document
                                    doc.Add(PdfTable);
                                }
                            }

                            //Close Pdf Document
                            doc.Close();
                            blStatus = true;
                        }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return blStatus;
        }

        //Get PDF Header table
        private PdfPTable GetPdfHeaderTable(TanVM tanVM)
        {
            PdfPTable ptHdr = null;
            try
            {
                if (!string.IsNullOrEmpty(TAN))
                {
                    ptHdr = new PdfPTable(3);
                    ptHdr.WidthPercentage = 100;
                    ptHdr.SpacingAfter = 8f;

                    pcInfo = new PdfPCell(new Phrase("GVKBio Sciences Pvt. Ltd Internal document", fontTinyItalic));
                    pcInfo.Colspan = 3;
                    pcInfo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    pcInfo.BackgroundColor = bgcolTANInfo;// new BaseColor(255, 160, 122);
                    ptHdr.AddCell(pcInfo);

                    #region Code commented
                    ////Add GVKBio Logo
                    //System.Drawing.Image oImage = System.Drawing.Image.FromFile(Application.StartupPath.ToString() + "\\gvkbio-logo.png");
                    ////Bitmap imge = new Bitmap();
                    //iTextSharp.text.Image imgLogo = iTextSharp.text.Image.GetInstance(oImage, System.Drawing.Imaging.ImageFormat.Jpeg);
                    //imgLogo.ScaleToFit(107f, 28f);
                    //imgLogo.ScaleAbsolute((float)107f, (float)28f);
                    ////imgLogo.Alignment = iTextSharp.text.Image.TEXTWRAP | iTextSharp.text.Image.ALIGN_CENTER;
                    //PdfPCell pcLogo = new PdfPCell(imgLogo);
                    //pcLogo.Colspan = 1;                   
                    //pcLogo.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                    //pcLogo.BackgroundColor = bgcolTANInfo;// new BaseColor(255, 160, 122);
                    //ptHdr.AddCell(pcLogo); 
                    #endregion

                    pcTAN = new PdfPCell(new Phrase("TAN - " + tanVM.TanNumber, fontTinyItalic));
                    pcCAN = new PdfPCell(new Phrase("CAN - " + tanVM.CanNumber, fontTinyItalic));
                    //PdfPCell pcDOI = new PdfPCell(new Phrase("DOI - " + DOI, fontTinyItalic));
                    pcBatch = new PdfPCell(new Phrase("Batch - " + tanVM.BatchNumber, fontTinyItalic));

                    pcTAN.BackgroundColor = bgcolRxnNo;
                    pcCAN.BackgroundColor = bgcolRxnNo;
                    //pcDOI.BackgroundColor = bgcolPageInfo;
                    pcBatch.BackgroundColor = bgcolRxnNo;

                    #region MyRegion
                    //pcTAN.Border = PdfPCell.NO_BORDER;
                    //pcCAN.Border = PdfPCell.NO_BORDER;
                    //pcDOI.Border = PdfPCell.NO_BORDER;
                    //pcBatch.Border = PdfPCell.NO_BORDER; 
                    #endregion

                    ptHdr.AddCell(pcTAN);
                    ptHdr.AddCell(pcCAN);
                    //ptHdr.AddCell(pcDOI);
                    ptHdr.AddCell(pcBatch);

                    //Get Comments string from TAN Comments
                    string tanComments = GetFormattedComments(tanVM.TanComments.TanComments.ToList());

                    //Add Comments to Pdf table                    
                    if (!string.IsNullOrEmpty(tanComments))
                    {
                        DataTable dtTANComments = GetCommentsTableFromTANComments(tanComments);
                        string strComments = GetCommentsForPdfFromTable(dtTANComments);
                        if (!string.IsNullOrEmpty(tanComments))
                        {
                            //Comments Header
                            pcCmntsHdr = new PdfPCell(new Phrase("Comments", fontTinyItalic));
                            pcCmntsHdr.Colspan = 3;
                            pcCmntsHdr.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                            pcCmntsHdr.BackgroundColor = bgcolReactant;// new BaseColor(255, 160, 122);
                            ptHdr.AddCell(pcCmntsHdr);

                            //Bind comments to RichTextBox for coloring
                            RichTextBox rtbComments = BindCommentsDataToRichTextBox(tanComments);

                            //Get Participants elements from RichTextBox Rtf and convert to HTML
                            List<IElement> lstComments = HTMLWorker.ParseToList(new StringReader(markupConverter.ConvertRtfToHtml(rtbComments.Rtf)), styles);

                            //Add Participants string in a row cell to Pdf Table
                            pcComments = new PdfPCell();
                            foreach (IElement iEle in lstComments)
                            {
                                pcComments.AddElement(iEle);
                            }

                            //TAN Comments
                            //pcComments = new PdfPCell(new Phrase(strComments, fontTinyItalic));
                            pcComments.Colspan = 3;
                            pcComments.HorizontalAlignment = PdfPCell.ALIGN_LEFT;
                            pcComments.VerticalAlignment = PdfPCell.ALIGN_TOP;
                            //pcComments.BackgroundColor = bgcolTANInfo;// new BaseColor(255, 160, 122);
                            ptHdr.AddCell(pcComments);
                        }
                    }
                    return ptHdr;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return ptHdr;
        }

        //Get Reaction Header table
        private PdfPTable GetReactionHeaderTable(int _rxnSNo, DataRow _rxnRowData)
        {
            PdfPTable ptRxnHdr = null;
            try
            {
                if (_rxnRowData != null)
                {
                    //Create Nested Table for Reaction header - Reaction.No, Num-Seq.No and Page info                                                                    
                    ptRxnHdr = new PdfPTable(2);

                    PdfPCell pcRxnNo = null;
                    PdfPCell pcNumSeq = null;
                    //PdfPCell pcPageInfo = null;

                    ptRxnHdr.WidthPercentage = 100;
                    pcRxnNo = new PdfPCell(new Phrase("Reaction - " + (_rxnSNo + 1).ToString(), fontTinyItalic));
                    pcNumSeq = new PdfPCell(new Phrase("Num - " + _rxnRowData["RXN_NUM"].ToString() + " Seq " + _rxnRowData["RXN_SEQ"].ToString(), fontTinyItalic));
                    //pcPageInfo = new PdfPCell(new Phrase("Page Info - " + _rxnRowData["page_label"].ToString(), fontTinyItalic));
                    pcRxnNo.BackgroundColor = bgcolRxnNo;
                    pcNumSeq.BackgroundColor = bgcolNumSeq;
                    //pcPageInfo.BackgroundColor = bgcolPageInfo;
                    pcRxnNo.Border = PdfPCell.NO_BORDER;
                    pcNumSeq.Border = PdfPCell.NO_BORDER;
                    //pcPageInfo.Border = PdfPCell.NO_BORDER;
                    ptRxnHdr.AddCell(pcRxnNo);
                    ptRxnHdr.AddCell(pcNumSeq);
                    //ptRxnHdr.AddCell(pcPageInfo);
                    return ptRxnHdr;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return ptRxnHdr;
        }

        //Get Reactant NUM & Stage Info table
        private PdfPTable GetNUM_StageTable(DataRow _rxnRowData)
        {
            PdfPTable ptNum_Stage = null;
            try
            {
                if (_rxnRowData != null)
                {
                    //Stage and NUM info table
                    ptNum_Stage = new PdfPTable(2);
                    ptNum_Stage.WidthPercentage = 100;
                    PdfPCell pcNum = new PdfPCell(new Phrase("NUM - " + _rxnRowData["NUM"].ToString(), fontTinyItalic));
                    PdfPCell pcStage = new PdfPCell(new Phrase(_rxnRowData["STAGE"].ToString(), fontTinyItalic));
                    pcNum.BackgroundColor = bgcolRctNumStage;
                    pcStage.BackgroundColor = bgcolRctNumStage;
                    pcNum.Border = PdfPCell.NO_BORDER;
                    pcStage.Border = PdfPCell.NO_BORDER;
                    ptNum_Stage.AddCell(pcNum);
                    ptNum_Stage.AddCell(pcStage);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return ptNum_Stage;
        }

        //Get Product NUM Info table        
        private PdfPTable GetProductNUMTable(DataRow _prodRowData)
        {
            PdfPTable ptProdNum = null;
            try
            {
                if (_prodRowData != null)
                {
                    ptProdNum = new PdfPTable(2);
                    ptProdNum.WidthPercentage = 100;
                    pcNum = new PdfPCell(new Phrase("NUM - " + _prodRowData["NUM"].ToString(), fontTinyItalic));
                    prdYieldCell = new PdfPCell(new Phrase("Yield - " + _prodRowData["YIELD"].ToString(), fontTinyItalic));
                    pcNum.BackgroundColor = bgcolProduct;// bgcolRctNumStage;
                    prdYieldCell.BackgroundColor = bgcolProduct;// bgcolRctNumStage;
                    pcNum.Border = PdfPCell.NO_BORDER;
                    prdYieldCell.Border = PdfPCell.NO_BORDER;
                    ptProdNum.AddCell(pcNum);
                    ptProdNum.AddCell(prdYieldCell);
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return ptProdNum;
        }

        #region Helper methods       

        #region Comments Methods

        private string GetFormattedComments(List<Client.ViewModels.Core.Comments> TanComments)
        {
            string strComments = "";
            try
            {
                if (TanComments != null && TanComments.Any())
                {
                    //Get CAS consulted for comments
                    strComments = strComments.Trim() + Environment.NewLine + GetCommentsOnCommentsType(CommentType.CAS, TanComments);

                    //Get Author Error comments
                    strComments = strComments.Trim() + Environment.NewLine + GetCommentsOnCommentsType(CommentType.AUTHOR, TanComments);

                    //Get Indexing Error comments
                    strComments = strComments.Trim() + Environment.NewLine + GetCommentsOnCommentsType(CommentType.INDEXING, TanComments);

                    //Get Other comments
                    strComments = strComments.Trim() + Environment.NewLine + GetCommentsOnCommentsType(CommentType.TEMPERATURE, TanComments);

                    //Get Other comments
                    strComments = strComments.Trim() + Environment.NewLine + GetCommentsOnCommentsType(CommentType.OTHER, TanComments);

                    //Get Default comments
                    strComments = strComments.Trim() + "\r\n" + GetCommentsOnCommentsType(CommentType.DEFAULT, TanComments);

                }

                //if (string.IsNullOrEmpty(strComments.Trim()))
                //{
                //    strComments = "~~";
                //}
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strComments.Trim();
        }

        private string GetCommentsOnCommentsType(CommentType commentsType, List<Client.ViewModels.Core.Comments> TanComments)
        {
            string strComments = "";
            try
            {
                if (TanComments != null && TanComments.Any())
                {
                    string strDelimiter = "";
                    switch (commentsType)
                    {
                        case CommentType.CAS:
                            strDelimiter = "CAS: ";
                            break;

                        case CommentType.INDEXING:
                            strDelimiter = "INDEXING: ";
                            break;

                        case CommentType.AUTHOR:
                            strDelimiter = "AUTHOR: ";
                            break;

                        case CommentType.OTHER:
                            strDelimiter = "OTHER: ";
                            break;
                        case CommentType.TEMPERATURE:
                            strDelimiter = "TEMPERATURE: ";
                            break;
                        case CommentType.DEFAULT:
                            strDelimiter = "DEFAULT: ";
                            break;
                    }

                    var rows = from r in TanComments
                               where r.CommentType == commentsType
                               select new
                               {
                                   Comments = r.TotalComment
                               };

                    if (rows != null)
                    {
                        foreach (var r in rows)
                        {
                            strComments = string.IsNullOrEmpty(strComments) ? (strDelimiter + r.Comments.Trim()) : (strComments + Environment.NewLine + strDelimiter + r.Comments.Trim());
                            //End with .
                            strComments = !strComments.Trim().EndsWith(".") ? strComments.Trim() + "." : r.Comments.Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strComments.Trim();
        }

        #endregion

        private object GetStructureFromSeries8000Table(int _ser8000val)
        {
            object objStruct = null;
            try
            {
                DataTable dtSer8000 = Generic.GlobalVariables.TAN_Series8000Data;
                if (dtSer8000 != null)
                {
                    if (dtSer8000.Rows.Count > 0)
                    {
                        DataRow[] dtRArr = dtSer8000.Select("SERIES_8000 = " + _ser8000val);
                        if (dtRArr != null)
                        {
                            if (dtRArr.Length > 0)
                            {
                                if (dtRArr[0]["SUBST_MOLECULE"] != null)
                                {
                                    ChemRenditor.MolfileString = "";
                                    ChemRenditor.MolfileString = dtRArr[0]["SUBST_MOLECULE"].ToString();
                                    objStruct = ChemRenditor.Image;
                                }
                                return objStruct;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return objStruct;
        }

        private object GetStructureFromMolFileString(string molfile)
        {
            object objStruct = null;
            try
            {
                if (!string.IsNullOrEmpty(molfile))
                {
                    ChemRenditor.MolfileString = "";
                    ChemRenditor.MolfileString = molfile;
                    objStruct = ChemRenditor.Image;
                }
                return objStruct;
            }
            catch (Exception ex)
            {
                Generic.Log.This(ex);
            }
            return objStruct;
        }

        private DataTable GetProductsForProdFormation(DataTable _prod_react, int _rxnID, string _partpnttype)
        {
            DataTable dtProducts = null;
            try
            {
                if (_prod_react != null && _prod_react.Rows.Count > 0)
                {
                    dtProducts = new DataTable();
                    dtProducts.Columns.Add("NUM", typeof(string));
                    dtProducts.Columns.Add("REG_NO", typeof(string));
                    dtProducts.Columns.Add("STRUCTURE", typeof(object));
                    dtProducts.Columns.Add("YIELD", typeof(string));
                    dtProducts.Columns.Add("STAGE", typeof(string));//For Reactants

                    DataView dvTemp = _prod_react.DefaultView;
                    if (_partpnttype.ToUpper() == "PRODUCT")
                    {
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID + "";
                    }
                    else if (_partpnttype.ToUpper() == "REACTANT")
                    {
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID + " and PP_TYPE = 'REACTANT'";
                        dvTemp.Sort = "RXN_STAGE_ID asc";
                    }
                    DataTable dtTemp = dvTemp.ToTable();

                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {

                        int intP_Num = 0;
                        DataRow dtRow = null;
                        string seriesType = "";
                        int intRegNo = 0;
                        int serNUMID = 0;
                        string strYield = "";
                        string strStage = "";

                        for (int i = 0; i < dtTemp.Rows.Count; i++)
                        {
                            int.TryParse(dtTemp.Rows[i]["SER_TAN_NUM_ID"].ToString(), out serNUMID);
                            int.TryParse(dtTemp.Rows[i]["SERIES_NUM"].ToString(), out intP_Num);
                            strYield = dtTemp.Rows[i]["YIELD"].ToString();
                            int.TryParse(dtTemp.Rows[i]["REG_NO"].ToString(), out intRegNo);
                            seriesType = dtTemp.Rows[i]["SER_TYPE"].ToString();
                            if (_partpnttype.ToUpper() == "REACTANT")
                            {
                                strStage = GetStageNameOnReactionIDStageID(_rxnID, Convert.ToInt32(dtTemp.Rows[i]["RXN_STAGE_ID"]));
                            }

                            if (seriesType == "NUM")
                            {
                                DataTable dtHexCodes = ReactDB.GetStructuresOnRegNo(serNUMID, intRegNo, "NUM");
                                if (dtHexCodes != null)
                                {
                                    foreach (DataRow row in dtHexCodes.Rows)
                                    {
                                        if (row["MOL_HEX_CODE"] != null && !string.IsNullOrEmpty(row["MOL_HEX_CODE"].ToString()))
                                        {
                                            string[] splitter = { "<CSIM>" };
                                            string[] strVals = row["MOL_HEX_CODE"].ToString().Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                            if (strVals != null && strVals.Length > 0)
                                            {
                                                for (int ridx = 0; ridx < strVals.Length; ridx++)
                                                {
                                                    dtRow = dtProducts.NewRow();
                                                    dtRow["NUM"] = intP_Num;
                                                    dtRow["REG_NO"] = intRegNo.ToString();
                                                    dtRow["YIELD"] = strYield;

                                                    dtRow["STRUCTURE"] = HexCodeToStructureImage.GetITextImageOnHexCode(strVals[ridx].Trim(), intRegNo.ToString());

                                                    dtRow["STAGE"] = strStage;
                                                    dtProducts.Rows.Add(dtRow);
                                                }
                                            }
                                        }
                                        else if (row["MOL_IMAGE"] != null)
                                        {
                                            dtRow = dtProducts.NewRow();
                                            dtRow["NUM"] = intP_Num;
                                            dtRow["REG_NO"] = intRegNo.ToString();
                                            dtRow["YIELD"] = strYield;

                                            try
                                            {
                                                dtRow["STRUCTURE"] = iTextSharp.text.Image.GetInstance(row["MOL_IMAGE"] as byte[], true);
                                            }
                                            catch//Some NUMs have empty structures
                                            {
                                                dtRow["STRUCTURE"] = null;
                                            }

                                            dtRow["STAGE"] = strStage;
                                            dtProducts.Rows.Add(dtRow);
                                        }
                                    }
                                }
                            }
                            else if (seriesType == "9000")
                            {
                                DataTable dtStructures = ReactDB.GetStructuresOnRegNo(serNUMID, intRegNo, "9000");

                                dtRow = dtProducts.NewRow();
                                dtRow["NUM"] = intP_Num;
                                dtRow["REG_NO"] = intRegNo.ToString();
                                dtRow["YIELD"] = strYield;
                                //dtRow["STRUCTURE"] = GetStructureFromMolFileString(dtStructures.Rows[0]["MOL_FILE"].ToString());

                                object img = GetStructureFromMolFileString(dtStructures.Rows[0]["MOL_FILE"].ToString());
                                if (img != null)
                                {
                                    dtRow["STRUCTURE"] = iTextSharp.text.Image.GetInstance(img as System.Drawing.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else
                                {
                                    dtRow["STRUCTURE"] = null;
                                }

                                dtRow["STAGE"] = strStage;
                                dtProducts.Rows.Add(dtRow);

                            }
                            else if (seriesType == "8500")
                            {
                                DataTable dtStructures = ReactDB.GetStructuresOnRegNo(serNUMID, intRegNo, "8500");

                                dtRow = dtProducts.NewRow();
                                dtRow["NUM"] = intP_Num;
                                dtRow["REG_NO"] = intRegNo.ToString();
                                dtRow["YIELD"] = strYield;
                                dtRow["STAGE"] = strStage;
                                object img = GetStructureFromMolFileString(dtStructures.Rows[0]["MOL_FILE"].ToString());
                                if (img != null)
                                {
                                    dtRow["STRUCTURE"] = iTextSharp.text.Image.GetInstance(img as System.Drawing.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else
                                {
                                    dtRow["STRUCTURE"] = null;
                                }
                                dtProducts.Rows.Add(dtRow);
                            }
                            else if (seriesType == "8000")
                            {
                                dtRow = dtProducts.NewRow();
                                dtRow["NUM"] = intP_Num;
                                dtRow["REG_NO"] = intRegNo;// dtTemp.Rows[i]["nrnreg"].ToString();
                                dtRow["YIELD"] = strYield;
                                dtRow["STAGE"] = strStage;

                                object img = GetStructureFromSeries8000Table(intP_Num);
                                if (img != null)
                                {
                                    dtRow["STRUCTURE"] = iTextSharp.text.Image.GetInstance(img as System.Drawing.Image, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                else
                                {
                                    dtRow["STRUCTURE"] = null;
                                }

                                dtProducts.Rows.Add(dtRow);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic.Log.This(ex);
            }
            return dtProducts;
        }

        private DataTable GetReactantsForProdFormation(DataTable _reactanttbl)
        {
            DataTable dtReacts = null;
            try
            {
                if (_reactanttbl != null)
                {
                    if (_reactanttbl.Rows.Count > 0)
                    {
                        dtReacts = new DataTable();
                        dtReacts.Columns.Add("STAGE", typeof(string));
                        dtReacts.Columns.Add("NUM", typeof(string));
                        dtReacts.Columns.Add("REG_NO", typeof(string));
                        dtReacts.Columns.Add("STRUCTURE", typeof(object));

                        DataTable dtReact = null;

                        int intP_Num = 0;
                        int intP_9000 = 0;
                        int intP_8500 = 0;
                        int intP_8000 = 0;

                        object objStruct = null;
                        DataRow dtRow = null;

                        dtReact = _reactanttbl;
                        string seriesType = "";
                        int intRegNo = 0;

                        if (dtReact.Rows.Count > 0)
                        {
                            for (int rIndx = 0; rIndx < dtReact.Rows.Count; rIndx++)
                            {

                                int.TryParse(dtReact.Rows[rIndx]["SERIES_NUM"].ToString(), out intP_Num);
                                int.TryParse(dtReact.Rows[rIndx]["REG_NO"].ToString(), out intRegNo);
                                seriesType = dtReact.Rows[rIndx]["SER_TYPE"].ToString();

                                objStruct = null;
                                if (seriesType.ToUpper() == "NUM")
                                {
                                    dtRow = dtReacts.NewRow();
                                    dtRow["Stage"] = GetStageNameOnReactionIDStageID(Convert.ToInt32(dtReact.Rows[rIndx]["RXN_ID"]), Convert.ToInt32(dtReact.Rows[rIndx]["RXN_STAGE_ID"]));
                                    dtRow["NUM"] = intP_Num;
                                    dtRow["NrnReg"] = dtReact.Rows[rIndx]["REG_NO"].ToString();
                                    objStruct = null;// CASRxnDataAccess.GetStructuresOnRegNo(Convert.ToInt32(dtReact.Rows[rIndx]["nrnreg"]));
                                    if (objStruct == null)
                                    {
                                        // objStruct = GetStructure from CGM
                                    }
                                    dtRow["Structure"] = objStruct;
                                    dtReacts.Rows.Add(dtRow);
                                }
                                else if (intP_9000 > 0)
                                {
                                    dtRow = dtReacts.NewRow();
                                    dtRow["Stage"] = GetStageNameOnReactionIDStageID(Convert.ToInt32(dtReact.Rows[rIndx]["RXN_ID"]), Convert.ToInt32(dtReact.Rows[rIndx]["RXN_STAGE_ID"]));
                                    dtRow["NUM"] = intP_9000;
                                    dtRow["NrnReg"] = dtReact.Rows[rIndx]["REG_NO"].ToString();
                                    dtRow["Structure"] = null;// CASRxnDataAccess.GetStructuresOnRegNo(Convert.ToInt32(dtReact.Rows[rIndx]["nrnreg"]));
                                    dtReacts.Rows.Add(dtRow);
                                }
                                else if (intP_8500 > 0)
                                {
                                    dtRow = dtReacts.NewRow();
                                    dtRow["Stage"] = GetStageNameOnReactionIDStageID(Convert.ToInt32(dtReact.Rows[rIndx]["RXN_ID"]), Convert.ToInt32(dtReact.Rows[rIndx]["RXN_STAGE_ID"]));
                                    dtRow["NUM"] = intP_8500;
                                    dtRow["NrnReg"] = dtReact.Rows[rIndx]["REG_NO"].ToString();
                                    dtRow["Structure"] = null;//  CASRxnDataAccess.GetStructuresOnRegNo(Convert.ToInt32(dtReact.Rows[rIndx]["nrnreg"]));
                                    dtReacts.Rows.Add(dtRow);
                                }
                                else if (intP_8000 > 0)
                                {
                                    dtRow = dtReacts.NewRow();
                                    dtRow["Stage"] = GetStageNameOnReactionIDStageID(Convert.ToInt32(dtReact.Rows[rIndx]["RXN_ID"]), Convert.ToInt32(dtReact.Rows[rIndx]["RXN_STAGE_ID"]));
                                    dtRow["NUM"] = intP_8000;
                                    dtRow["NrnReg"] = dtReact.Rows[rIndx]["REG_NO"].ToString();
                                    dtRow["Structure"] = GetStructureFrom8000Table(intP_8000);
                                    dtReacts.Rows.Add(dtRow);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic.Log.This(ex);
            }
            return dtReacts;
        }

        private void GetProductsPartpntsCondsOnTAN(int tanID)
        {
            try
            {
                if (tanID > 0)
                {
                    //Product details
                    //Reatants,Agents,Solvents,Catalyst details
                    DataTable prodPartpnts = ReactDB.GetProduct_ParticipantsDataOnTAN(tanID);
                    if (prodPartpnts != null)
                    {
                        DataView dvTemp = prodPartpnts.DefaultView;
                        dvTemp.RowFilter = "PP_TYPE = 'PRODUCT'";
                        ProductsData = dvTemp.ToTable();
                    }

                    ParticipantsData = prodPartpnts;

                    //RSN Details
                    RxnRSN = ReactDB.GetRSNDetailsOnTAN(tanID);

                    //Reaction Condition Details
                    RxnConditions = ReactDB.GetConditionDataOnTAN(tanID);

                    //All Stages on TAN
                    StagesData = ReactDB.GetStagesOnTAN(tanID);

                    //TAN Comments
                    CommentsData = ReactDB.GetTANCommentsOnTANID(tanID);

                    //Get saved 8000 and NrnReg details on TAN and save in Global datatable
                    //Ser8000Data = CASRxnDataAccess.GetSeries8000DetailsOnTAN(tan);
                    Ser8000Data = ReactDB.GetSeries8000DetailsOnTAN(tanID);

                    //Get saved 8500 and NrnReg details on TAN and save in Global datatable
                    IndxReactNarrBll.ManageSeries8500 mngSer8500 = new IndxReactNarrBll.ManageSeries8500();
                    mngSer8500.TAN_ID = TAN_ID;
                    mngSer8500.OrgRefID = 0;
                    DataTable dtTABSer5000 = null;
                    ReactDB.UpdateSeries8500DetailsOnTAN(mngSer8500, out dtTABSer5000);
                    Ser8500Data = dtTABSer5000;
                }
            }
            catch (Exception ex)
            {
                Generic.Log.This(ex);
            }
        }

        private DataTable GetProductDataOnReactionID(int _rxnID)
        {
            DataTable dtProd = null;
            try
            {
                if (ProductsData != null)
                {
                    if (ProductsData.Rows.Count > 0)
                    {
                        dtProd = ProductsData.Copy();
                        DataView dvTemp = dtProd.DefaultView;
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID;
                        dtProd = dvTemp.ToTable();
                        return dtProd;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtProd;
        }

        private DataTable GetReactantsDataOnReactionID(int _rxnID)
        {
            DataTable dtReactant = null;
            try
            {
                if (ParticipantsData != null)
                {
                    if (ParticipantsData.Rows.Count > 0)
                    {
                        dtReactant = ParticipantsData.Copy();
                        DataView dvTemp = dtReactant.DefaultView;
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID + " and PP_TYPE = 'REACTANT'";
                        //dvTemp.Sort = "rxn_stage_id asc";
                        dtReactant = dvTemp.ToTable();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtReactant;
        }

        private DataTable GetParticipantsDataOnReactionID(int _rxnID)
        {
            DataTable dtPartpnts = null;
            try
            {
                if (ParticipantsData != null)
                {
                    if (ParticipantsData.Rows.Count > 0)
                    {
                        dtPartpnts = ParticipantsData.Copy();
                        DataView dvTemp = dtPartpnts.DefaultView;
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID + " and PP_TYPE <> 'REACTANT'";
                        //dvTemp.Sort = "rxn_stage_id asc";
                        dtPartpnts = dvTemp.ToTable();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtPartpnts;
        }

        private DataTable GetConditionsDataOnReactionID(int _rxnID)
        {
            DataTable dtConds = null;
            try
            {
                if (RxnConditions != null)
                {
                    if (RxnConditions.Rows.Count > 0)
                    {
                        dtConds = RxnConditions.Copy();
                        DataView dvTemp = dtConds.DefaultView;
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID;
                        //dvTemp.Sort = "rxn_stage_id asc";
                        dtConds = dvTemp.ToTable();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtConds;
        }

        private DataTable GetRSNDataOnReactionID(int _rxnID)
        {
            DataTable dtRsn = null;
            try
            {
                if (RxnRSN != null)
                {
                    if (RxnRSN.Rows.Count > 0)
                    {
                        dtRsn = RxnRSN.Copy();
                        DataView dvTemp = dtRsn.DefaultView;
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID;
                        //dvTemp.Sort = "rxn_stage_id asc";
                        dtRsn = dvTemp.ToTable();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtRsn;
        }

        private DataTable GetStagesOnReactionID(int _rxnID)
        {
            DataTable dtStages = null;
            try
            {
                if (StagesData != null)
                {
                    if (StagesData.Rows.Count > 0)
                    {
                        dtStages = StagesData.Copy();
                        DataView dvTemp = dtStages.DefaultView;
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID;
                        //dvTemp.Sort = "stageid asc";
                        dtStages = dvTemp.ToTable();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtStages;
        }

        private object GetStructureFrom8000Table(int _ser8000val)
        {
            object objStruct = null;
            try
            {
                DataTable dtSer8000 = Ser8000Data;
                if (dtSer8000 != null)
                {
                    if (dtSer8000.Rows.Count > 0)
                    {
                        DataRow[] dtRArr = dtSer8000.Select("SERIES_8000 = " + _ser8000val);
                        if (dtRArr != null)
                        {
                            if (dtRArr.Length > 0)
                            {
                                objStruct = dtRArr[0]["SUBST_MOLECULE"];
                                return objStruct;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic.Log.This(ex);
            }
            return objStruct;
        }

        private object GetStructureFrom8500Table(int _ser8500val)
        {
            object objStruct = null;
            try
            {
                DataTable dtSer8500 = Ser8500Data;
                if (dtSer8500 != null)
                {
                    if (dtSer8500.Rows.Count > 0)
                    {
                        DataRow[] dtRArr = dtSer8500.Select("SERIES_8500 = " + _ser8500val);
                        if (dtRArr != null)
                        {
                            if (dtRArr.Length > 0)
                            {
                                objStruct = dtRArr[0]["MOL_FILE"];
                                return objStruct;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic.Log.This(ex);
            }
            return objStruct;
        }

        private string GetStageNameOnReactionIDStageID(int _rxnID, int _rxn_stage_id)
        {
            string strStageName = "";
            try
            {
                if (StagesData != null)
                {
                    if (StagesData.Rows.Count > 0)
                    {
                        DataTable dtStages = StagesData.Copy();
                        DataView dvTemp = dtStages.DefaultView;
                        dvTemp.RowFilter = "RXN_ID = " + _rxnID + " and RXN_STAGE_ID = " + _rxn_stage_id;
                        dtStages = dvTemp.ToTable();
                        if (dtStages != null)
                        {
                            if (dtStages.Rows.Count > 0)
                            {
                                strStageName = "Stage" + dtStages.Rows[0]["DISPLAY_ORDER"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strStageName;
        }

        private string GetProductNUMAndYieldFromTable(DataTable _prodtbl)
        {
            string strProd = "";
            try
            {
                if (_prodtbl != null)
                {
                    int intP_Num = 0;
                    int intP_9000 = 0;
                    int intP_8500 = 0;
                    int intP_8000 = 0;

                    if (_prodtbl.Rows.Count > 0)
                    {
                        for (int i = 0; i < _prodtbl.Rows.Count; i++)
                        {
                            int.TryParse(_prodtbl.Rows[i]["p_num"].ToString(), out intP_Num);
                            int.TryParse(_prodtbl.Rows[i]["p_9000"].ToString(), out intP_9000);
                            int.TryParse(_prodtbl.Rows[i]["p_8500"].ToString(), out intP_8500);
                            int.TryParse(_prodtbl.Rows[i]["p_8000"].ToString(), out intP_8000);

                            if (intP_Num > 0)
                            {
                                if (strProd.Trim() == "")
                                {
                                    strProd = intP_Num.ToString();
                                }
                                else
                                {
                                    strProd = strProd + ", " + intP_Num.ToString();
                                }

                                if (_prodtbl.Rows[i]["yield"].ToString().Trim() != "")
                                {
                                    strProd = strProd + " (" + _prodtbl.Rows[i]["yield"].ToString().Trim() + "%" + ")";
                                }
                            }
                            else if (intP_9000 > 0)
                            {
                                if (strProd.Trim() == "")
                                {
                                    strProd = intP_9000.ToString();
                                }
                                else
                                {
                                    strProd = strProd + ", " + intP_9000.ToString();
                                }

                                if (_prodtbl.Rows[i]["yield"].ToString().Trim() != "")
                                {
                                    strProd = strProd + " (" + _prodtbl.Rows[i]["yield"].ToString().Trim() + "%" + ")";
                                }
                            }
                            else if (intP_8500 > 0)
                            {
                                if (strProd.Trim() == "")
                                {
                                    strProd = intP_8500.ToString();
                                }
                                else
                                {
                                    strProd = strProd + ", " + intP_8500.ToString();
                                }

                                if (_prodtbl.Rows[i]["yield"].ToString().Trim() != "")
                                {
                                    strProd = strProd + " (" + _prodtbl.Rows[i]["yield"].ToString().Trim() + "%" + ")";
                                }
                            }
                            else if (intP_8000 > 0)
                            {
                                if (strProd.Trim() == "")
                                {
                                    strProd = intP_8000.ToString();
                                }
                                else
                                {
                                    strProd = strProd + ", " + intP_8000.ToString();
                                }

                                if (_prodtbl.Rows[i]["yield"].ToString().Trim() != "")
                                {
                                    strProd = strProd + " (" + _prodtbl.Rows[i]["yield"].ToString().Trim() + "%" + ")";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strProd;
        }

        private DataTable GetPartpantDataFromTableOnStageID(DataTable _partpnttbl, int _stageid, string _partpnttype)
        {
            DataTable dtpatpnt = null;
            try
            {
                if (_partpnttbl != null)
                {
                    if (_partpnttbl.Rows.Count > 0 && _stageid > 0)
                    {
                        DataView dvTemp = _partpnttbl.DefaultView;
                        if (_partpnttype.Trim() != "")
                        {
                            dvTemp.RowFilter = "RXN_STAGE_ID = " + _stageid + " and PP_TYPE = '" + _partpnttype + "'";
                        }
                        else
                        {
                            dvTemp.RowFilter = "RXN_STAGE_ID = " + _stageid;
                        }
                        dtpatpnt = dvTemp.ToTable();
                        return dtpatpnt;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtpatpnt;
        }

        private string GetParticipantStringFromTable(DataTable partpnttbl, string partpnt)
        {
            string strPartpnt = "";
            string strValue = "";
            try
            {
                string strTemp = "";
                if (partpnt == "AGENT")
                {
                    strTemp = "NO AGENT";
                }
                else if (partpnt == "SOLVENT")
                {
                    strTemp = "NO SOLVENT";
                }
                else if (partpnt == "CATALYST")
                {
                    strTemp = "NO CATALYST";
                }

                if (partpnttbl != null)
                {
                    if (partpnttbl.Rows.Count > 0)
                    {
                        for (int i = 0; i < partpnttbl.Rows.Count; i++)
                        {
                            strValue = "";

                            if (partpnttbl.Rows[i]["SERIES_NUM"].ToString().Trim() != "" && partpnttbl.Rows[i]["SERIES_NUM"].ToString().Trim() != "0")
                            {
                                strValue = "(" + partpnttbl.Rows[i]["SERIES_NUM"].ToString().Trim() + ") ";
                            }

                            if (partpnttbl.Rows[i]["PP_NAME"].ToString().Trim() != "")
                            {
                                strValue = strValue.Trim() + partpnttbl.Rows[i]["PP_NAME"].ToString().Trim();
                            }

                            if (!string.IsNullOrEmpty(strValue.Trim()))
                            {
                                strPartpnt = string.IsNullOrEmpty(strPartpnt.Trim()) ? partpnt + "= " + strValue : strPartpnt + ", " + strValue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strPartpnt;
        }

        private string GetCondtionsStringFromTable(DataTable condstbl, out string time_out, out string pressure_out, out string ph_out)
        {
            string strTemp = "";
            string strTime = "";
            string strPressure = "";
            string strPH = "";

            try
            {
                if (condstbl != null)
                {
                    if (condstbl.Rows.Count > 0)
                    {
                        for (int i = 0; i < condstbl.Rows.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(condstbl.Rows[i]["TEMPERATURE"].ToString().Trim()))
                            {
                                strTemp = string.IsNullOrEmpty(strTemp.Trim()) ? "TP: " + condstbl.Rows[i]["TEMPERATURE"].ToString() : strTemp + "," + condstbl.Rows[i]["TEMPERATURE"].ToString();
                            }

                            if (!string.IsNullOrEmpty(condstbl.Rows[i]["RC_TIME"].ToString().Trim()))
                            {
                                strTime = string.IsNullOrEmpty(strTime.Trim()) ? "TM: " + condstbl.Rows[i]["RC_TIME"].ToString() : strTime + "," + condstbl.Rows[i]["RC_TIME"].ToString();
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(condstbl.Rows[i]["TEMPERATURE"].ToString().Trim()))
                                {
                                    strTime = string.IsNullOrEmpty(strTime.Trim()) ? "TM: " : strTime + ",";
                                }
                            }

                            if (!string.IsNullOrEmpty(condstbl.Rows[i]["PRESSURE"].ToString().Trim()))
                            {
                                strPressure = string.IsNullOrEmpty(strPressure.Trim()) ? "PR: " + condstbl.Rows[i]["PRESSURE"].ToString() : strPressure + "," + condstbl.Rows[i]["PRESSURE"].ToString();
                            }

                            if (!string.IsNullOrEmpty(condstbl.Rows[i]["PH"].ToString().Trim()))
                            {
                                strPH = string.IsNullOrEmpty(strPH.Trim()) ? "PH: " + condstbl.Rows[i]["ph"].ToString() : strPH + "," + condstbl.Rows[i]["PH"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            time_out = strTime;
            pressure_out = strPressure;
            ph_out = strPH;
            return strTemp;
        }

        private string GetRSNDetailsFromTable(DataTable rsntbl, out string _freetext_rxn, out string _freetext_stage)
        {
            string strRSN_CVT = "";
            string strRSN_FT_Rxn = "";
            string strRSN_FT_Stage = "";
            string strValue = "";
            try
            {
                if (rsntbl != null)
                {
                    if (rsntbl.Rows.Count > 0)
                    {
                        for (int i = 0; i < rsntbl.Rows.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(rsntbl.Rows[i]["CVT"].ToString().Trim()))
                            {
                                strValue = rsntbl.Rows[i]["CVT"].ToString().Trim();
                                strRSN_CVT = string.IsNullOrEmpty(strRSN_CVT.Trim()) ? strValue : strRSN_CVT + ", " + strValue;
                            }

                            if (!string.IsNullOrEmpty(rsntbl.Rows[i]["FREE_TEXT"].ToString().Trim()))
                            {
                                strValue = rsntbl.Rows[i]["FREE_TEXT"].ToString().Trim();
                                if (rsntbl.Rows[i]["NOTE_LEVEL"].ToString().Trim().ToUpper() == "REACTION")
                                {
                                    strRSN_FT_Rxn = string.IsNullOrEmpty(strRSN_FT_Rxn.Trim()) ? strValue : strRSN_FT_Rxn + ", " + strValue;
                                }
                                else if (rsntbl.Rows[i]["NOTE_LEVEL"].ToString().Trim().ToUpper() == "STAGE")
                                {
                                    strRSN_FT_Stage = string.IsNullOrEmpty(strRSN_FT_Stage.Trim()) ? strValue : strRSN_FT_Stage + ", " + strValue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            _freetext_rxn = strRSN_FT_Rxn;
            _freetext_stage = strRSN_FT_Stage;
            return strRSN_CVT;
        }

        private DataTable GetParticipantsForProdFormation(DataTable _partpntstbl, DataTable _condstbl, DataTable _rsntbl, DataTable _stagestbl)
        {
            DataTable dtPartpnts = null;
            try
            {
                if (_stagestbl != null)
                {
                    if (_stagestbl.Rows.Count > 0)
                    {
                        dtPartpnts = new DataTable();
                        dtPartpnts.Columns.Add("Stage", typeof(string));
                        dtPartpnts.Columns.Add("Agents", typeof(string));
                        dtPartpnts.Columns.Add("Solvents", typeof(string));
                        dtPartpnts.Columns.Add("Catalysts", typeof(string));
                        dtPartpnts.Columns.Add("Temperature", typeof(string));
                        dtPartpnts.Columns.Add("Time", typeof(string));
                        dtPartpnts.Columns.Add("Pressure", typeof(string));
                        dtPartpnts.Columns.Add("pH", typeof(string));
                        dtPartpnts.Columns.Add("RSN_CVT", typeof(string));
                        dtPartpnts.Columns.Add("RSN_FT_Reaction", typeof(string));
                        dtPartpnts.Columns.Add("RSN_FT_Stage", typeof(string));

                        string strAgent = "";
                        string strSolvent = "";
                        string strCatalyst = "";
                        string strTemp = "";
                        string strTime = "";
                        string strPressure = "";
                        string strPh = "";
                        string strRSN_CVT = "";
                        string strRSN_FT_RXN = "";
                        string strRSN_FT_Stage = "";

                        int intStageID = 0;
                        string strStageName = "";

                        DataTable dt_Agent = null;
                        DataTable dt_Solvent = null;
                        DataTable dt_Catalyst = null;
                        DataTable dt_Conds = null;
                        DataTable dt_RSN = null;

                        for (int i = 0; i < _stagestbl.Rows.Count; i++)
                        {
                            intStageID = Convert.ToInt32(_stagestbl.Rows[i]["RXN_STAGE_ID"].ToString());
                            strStageName = "Stage" + _stagestbl.Rows[i]["DISPLAY_ORDER"].ToString();

                            dt_Agent = GetPartpantDataFromTableOnStageID(_partpntstbl, intStageID, "AGENT");
                            dt_Solvent = GetPartpantDataFromTableOnStageID(_partpntstbl, intStageID, "SOLVENT");
                            dt_Catalyst = GetPartpantDataFromTableOnStageID(_partpntstbl, intStageID, "CATALYST");
                            dt_Conds = GetPartpantDataFromTableOnStageID(_condstbl, intStageID, "");
                            dt_RSN = GetPartpantDataFromTableOnStageID(_rsntbl, intStageID, "");

                            strAgent = GetParticipantStringFromTable(dt_Agent, "AGENT");
                            strSolvent = GetParticipantStringFromTable(dt_Solvent, "SOLVENT");
                            strCatalyst = GetParticipantStringFromTable(dt_Catalyst, "CATALYST");
                            strTemp = GetCondtionsStringFromTable(dt_Conds, out strTime, out strPressure, out strPh);
                            strRSN_CVT = GetRSNDetailsFromTable(dt_RSN, out strRSN_FT_RXN, out strRSN_FT_Stage);

                            DataRow dtRow = dtPartpnts.NewRow();
                            dtRow["Stage"] = strStageName;
                            dtRow["Agents"] = strAgent;
                            dtRow["Solvents"] = strSolvent;
                            dtRow["Catalysts"] = strCatalyst;
                            dtRow["Temperature"] = strTemp;
                            dtRow["Time"] = strTime;
                            dtRow["Pressure"] = strPressure;
                            dtRow["pH"] = strPh;
                            dtRow["RSN_CVT"] = strRSN_CVT;
                            dtRow["RSN_FT_Reaction"] = strRSN_FT_RXN;
                            dtRow["RSN_FT_Stage"] = strRSN_FT_Stage;
                            dtPartpnts.Rows.Add(dtRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Generic.Log.This(ex);
            }
            return dtPartpnts;
        }

        #region Participants String methods

        private RichTextBox GetPartpntsDataBindToRichTextBox()
        {
            RichTextBox rtbPartpnts = null;
            try
            {
                rtbPartpnts = new RichTextBox();

                //Participants, Conditions and RSN Information
                rtbPartpnts.Text = GetParticipantsStringFromTable();
                if (strRSN_CVT_Rxn.Trim() != "")
                {
                    string strFinalVal = rtbPartpnts.Text.Trim() + "\r\nRSN REACTION:\r\n             " + strRSN_CVT_Rxn.Trim();
                    rtbPartpnts.Text = strFinalVal;
                }
                if (strRSN_FT_Rxn.Trim() != "")
                {
                    if (strRSN_CVT_Rxn.Trim() != "")
                    {
                        rtbPartpnts.Text = rtbPartpnts.Text.Trim() + "\r\n           " + strRSN_FT_Rxn.Trim();
                    }
                    else
                    {
                        rtbPartpnts.Text = rtbPartpnts.Text.Trim() + "\r\nRSN REACTION:\r\n             " + strRSN_FT_Rxn.Trim();
                    }
                }
                if (strRSN_FT_Stage.Trim() != "")
                {
                    if (strRSN_FT_Rxn.Trim() == "")
                    {
                        rtbPartpnts.Text = rtbPartpnts.Text.Trim() + "\r\n" + strRSN_FT_Stage.Trim();
                    }
                    else
                    {
                        rtbPartpnts.Text = rtbPartpnts.Text.Trim() + "\r\n" + strRSN_FT_Stage.Trim();
                    }
                }

                //Replace New line with junk characters before coloring and replace the same with new line after coloring
                rtbPartpnts.Rtf = rtbPartpnts.Rtf.Replace("\r\n", "_|0|_");
                ColourRrbText(rtbPartpnts);
                rtbPartpnts.Rtf = rtbPartpnts.Rtf.Replace("_|0|_", "\r\n");
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return rtbPartpnts;
        }

        private void ColourRrbText(RichTextBox rtb)
        {
            try
            {
                Regex regEx_Stg = new Regex("Stage [0-9]{0,3}");
                foreach (Match match in regEx_Stg.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.DarkOrange;
                }

                Regex regEx_Empty = new Regex("No Agent / Catalyst / Solent / Conditions are available");
                foreach (Match match in regEx_Empty.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.LightGray;
                }

                Regex regEx_C = new Regex("TP:|TM:|PR:|PH:");
                foreach (Match match in regEx_C.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.Red;
                }

                Regex regEx_P = new Regex("AGENT|SOLVENT|CATALYST");
                foreach (Match match in regEx_P.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.LimeGreen;
                }

                Regex regEx_R = new Regex("RSN REACTION|RSN STAGE");
                foreach (Match match in regEx_R.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.DarkViolet;
                }

                Regex regEx_CVT = new Regex("CVT|FREE");
                foreach (Match match in regEx_CVT.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.DeepPink;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        private string GetParticipantsStringFromTable()
        {
            string strPartpnt = "";
            try
            {
                if (RxnParticipants != null)
                {
                    if (RxnParticipants.Rows.Count > 0)
                    {
                        for (int i = 0; i < RxnParticipants.Rows.Count; i++)
                        {
                            if (RxnParticipants.Rows[i]["Agents"].ToString().Trim() != "" ||
                                RxnParticipants.Rows[i]["Solvents"].ToString().Trim() != "" ||
                                RxnParticipants.Rows[i]["Catalysts"].ToString().Trim() != "" ||
                                RxnParticipants.Rows[i]["Temperature"].ToString().Trim() != "" ||
                                RxnParticipants.Rows[i]["Time"].ToString().Trim() != "" ||
                                RxnParticipants.Rows[i]["Pressure"].ToString().Trim() != "" ||
                                RxnParticipants.Rows[i]["pH"].ToString().Trim() != "")
                            {
                                strPartpnt = string.IsNullOrEmpty(strPartpnt.Trim()) ? RxnParticipants.Rows[i]["Stage"].ToString() + " - " : strPartpnt + "\r\n" + RxnParticipants.Rows[i]["Stage"].ToString() + " - ";

                                if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["Agents"].ToString().Trim()))
                                {
                                    strPartpnt = strPartpnt.Trim() + "  " + RxnParticipants.Rows[i]["Agents"].ToString().Trim();
                                }

                                if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["Solvents"].ToString().Trim()))
                                {
                                    strPartpnt = strPartpnt.Trim() + "  " + RxnParticipants.Rows[i]["Solvents"].ToString().Trim();
                                }

                                if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["Catalysts"].ToString().Trim()))
                                {
                                    strPartpnt = strPartpnt.Trim() + "  " + RxnParticipants.Rows[i]["Catalysts"].ToString().Trim();
                                }

                                if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["Temperature"].ToString().Trim()))
                                {
                                    strPartpnt = strPartpnt.Trim() + "  " + RxnParticipants.Rows[i]["Temperature"].ToString().Trim();
                                }

                                if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["Time"].ToString().Trim()))
                                {
                                    strPartpnt = strPartpnt.Trim() + "  " + RxnParticipants.Rows[i]["Time"].ToString().Trim();
                                }

                                if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["Pressure"].ToString().Trim()))
                                {
                                    strPartpnt = strPartpnt.Trim() + "  " + RxnParticipants.Rows[i]["Pressure"].ToString().Trim();
                                }

                                if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["pH"].ToString().Trim()))
                                {
                                    strPartpnt = strPartpnt.Trim() + "  " + RxnParticipants.Rows[i]["pH"].ToString().Trim();
                                }
                            }
                            else//No Agent/Solvent/Catalyst/Conditions are available
                            {
                                strPartpnt = string.IsNullOrEmpty(strPartpnt.Trim()) ? RxnParticipants.Rows[i]["Stage"].ToString() + " - " : strPartpnt + "\r\n" + RxnParticipants.Rows[i]["Stage"].ToString() + " - ";

                                strPartpnt = strPartpnt + " No Agent / Catalyst / Solent / Conditions are available";
                            }

                            if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["RSN_CVT"].ToString().Trim()))
                            {
                                strRSN_CVT_Rxn = string.IsNullOrEmpty(strRSN_CVT_Rxn.Trim()) ? "CVT: " + RxnParticipants.Rows[i]["RSN_CVT"].ToString().Trim() : strRSN_CVT_Rxn + ", " + RxnParticipants.Rows[i]["RSN_CVT"].ToString().Trim();
                            }

                            if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["RSN_FT_Reaction"].ToString().Trim()))
                            {
                                strRSN_FT_Rxn = string.IsNullOrEmpty(strRSN_FT_Rxn.Trim()) ? "FREE: " + RxnParticipants.Rows[i]["RSN_FT_Reaction"].ToString().Trim() : strRSN_FT_Rxn + ", " + RxnParticipants.Rows[i]["RSN_FT_Reaction"].ToString().Trim();
                            }

                            if (!string.IsNullOrEmpty(RxnParticipants.Rows[i]["RSN_FT_Stage"].ToString().Trim()))
                            {
                                strRSN_FT_Stage = string.IsNullOrEmpty(strRSN_FT_Stage.Trim()) ? "RSN STAGE: " + RxnParticipants.Rows[i]["RSN_FT_Stage"].ToString().Trim() : strRSN_FT_Stage + ", " + RxnParticipants.Rows[i]["RSN_FT_Stage"].ToString().Trim();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strPartpnt;
        }

        #endregion

        private int GetPanelControlsLength(DataTable _prod_react, string _srcgrid)
        {
            int intLen = 0;
            try
            {
                if (_prod_react != null)
                {
                    if (_prod_react.Rows.Count > 0)
                    {
                        DataTable dtCgm = CgmFileData.Copy();// CGMDataTbl.Copy();

                        for (int i = 0; i < _prod_react.Rows.Count; i++)
                        {
                            int intRegNo = Convert.ToInt32(_prod_react.Rows[i]["nrnreg"].ToString());
                            if (intRegNo > 0)
                            {
                                string strFCond = "<SUBSTANC><RN ID=" + "\"" + intRegNo + "\"" + ">*";
                                try
                                {
                                    DataTable dtoutput = dtCgm.AsEnumerable().Where(a => Regex.IsMatch(a[dtCgm.Columns[0].ColumnName].ToString(), strFCond)).CopyToDataTable();
                                    if (dtoutput != null)
                                    {
                                        if (dtoutput.Rows.Count > 0)
                                        {
                                            string cellValue = dtoutput.Rows[0][0].ToString();
                                            if (cellValue.Contains("<SIM>"))//Single structure
                                            {
                                                if (_srcgrid.ToUpper() == "REACTANT")
                                                {
                                                    intLen = intLen + 1;
                                                }
                                                else
                                                {
                                                    intLen = intLen + 1;
                                                }
                                            }
                                            else if (cellValue.Contains("<CSIM>"))//Multiple strucrures
                                            {
                                                string[] splitter = { "<CSIM>" };
                                                string[] strVals = cellValue.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                                                if (strVals != null)
                                                {
                                                    if (strVals.Length > 0)
                                                    {
                                                        //if (i == 0 && dt_prod_react.Rows.Count >= 1)
                                                        //{
                                                        //    intLen = intLen + strVals.Length;
                                                        //}
                                                        //else
                                                        //{
                                                        if (_srcgrid.ToUpper() == "REACTANT")
                                                        {
                                                            intLen = intLen + (strVals.Length - 1);
                                                        }
                                                        else
                                                        {
                                                            intLen = intLen + (strVals.Length - 1);
                                                        }
                                                        //}
                                                    }
                                                }
                                            }
                                            else //No Structure
                                            {
                                                if (_srcgrid.ToUpper() == "REACTANT")
                                                {
                                                    intLen = intLen + 1;
                                                }
                                                else
                                                {
                                                    intLen = intLen + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch//Exception for 8500 
                                {
                                    intLen = intLen + 1;
                                }
                            }
                            else// 8000 series Product
                            {
                                if (_srcgrid.ToUpper() == "PRODUCT")
                                {
                                    intLen = intLen + 1;
                                }
                                else if (_srcgrid.ToUpper() == "REACTANT")
                                {
                                    intLen = intLen + 1;
                                }
                            }
                        }

                        //if (_srcgrid.ToUpper() == "PRODUCT" && _prod_react.Rows.Count > 1)
                        //{
                        //    intLen = intLen + 1;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return intLen;
        }

        private string[] GetHexCodeOnRegNo(int _regno)
        {
            string[] strHexArr = null;
            try
            {
                if (CgmFileData != null)
                {
                    if (CgmFileData.Rows.Count > 0)
                    {
                        DataTable dtcdm = CgmFileData.Copy();// CGMDataTbl.Copy();
                        DataTable dt = new DataTable();

                        string strFCond = "<SUBSTANC><RN ID=" + "\"" + _regno + "\"" + ">*";
                        try
                        {
                            DataTable dtoutput = dtcdm.AsEnumerable().Where(a => Regex.IsMatch(a[dtcdm.Columns[0].ColumnName].ToString(), strFCond)).CopyToDataTable();
                            if (dtoutput != null)
                            {
                                if (dtoutput.Rows.Count > 0)
                                {
                                    string cellValue = dtoutput.Rows[0][0].ToString();
                                    if (cellValue.Contains("<SIM>"))//Single structure
                                    {
                                        string strSIM = cellValue.Substring(cellValue.IndexOf("<SIM>") + ("<SIM>".Length), ((cellValue.IndexOf("</SIM>") + 1) - cellValue.IndexOf("<SIM>")) - ("</SIM>".Length));
                                        strHexArr = new string[1];
                                        strHexArr[0] = strSIM;
                                        return strHexArr;
                                    }
                                    else if (cellValue.Contains("<CSIM>"))//Multiple strucrures
                                    {
                                        string[] splitter = { "<CSIM>" };
                                        string[] strValues = cellValue.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                                        ArrayList alstVals = new ArrayList();
                                        if (strValues != null)
                                        {
                                            if (strValues.Length > 0)
                                            {
                                                string[] splitter_End = { "</CSIM>" };
                                                for (int i = 0; i < strValues.Length; i++)
                                                {
                                                    if (!strValues[i].StartsWith("<SUBSTANC"))
                                                    {
                                                        string[] strValues_Hex = strValues[i].Split(splitter_End, StringSplitOptions.RemoveEmptyEntries);
                                                        if (strValues_Hex != null)
                                                        {
                                                            if (strValues_Hex.Length > 0)
                                                            {
                                                                alstVals.Add(strValues_Hex[0]);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (alstVals != null)
                                        {
                                            if (alstVals.Count > 0)
                                            {
                                                strHexArr = (String[])alstVals.ToArray(typeof(string));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch //Exception for series 8500 RegNos
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strHexArr;
        }

        //public static iTextSharp.text.Image GetChemistryImageOnHexCode(string hexcode, string regno)
        //{
        //    iTextSharp.text.Image imgChem = null;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(hexcode.Trim()))
        //        {
        //            HexToCgmFile objHex = new HexToCgmFile();
        //            string strCgmPath = AppDomain.CurrentDomain.BaseDirectory + regno + ".cgm";
        //            string strImagePath = AppDomain.CurrentDomain.BaseDirectory + regno + ".gif";

        //            objHex.Convert_HexToCgm_Save_In_File(hexcode, ref strCgmPath);

        //            CADViewXClass cs = null;
        //            cs = new CADViewXClass();
        //            try
        //            {
        //                cs.LoadFile(strCgmPath);
        //                cs.SaveToFile(strImagePath);
        //                cs.CloseFile();
        //            }
        //            catch (Exception ex)
        //            {
        //                Log.This(ex);
        //            }
        //            imgChem = iTextSharp.text.Image.GetInstance(strImagePath);

        //            //Resize image depend upon your need
        //            imgChem.ScaleToFit(280f, 260f);
        //            //Give space before image
        //            imgChem.SpacingBefore = 30f;
        //            //Give some space after the image
        //            imgChem.SpacingAfter = 1f;
        //            imgChem.Alignment = iTextSharp.text.Element.ALIGN_CENTER;

        //            if (File.Exists(strCgmPath))
        //            {
        //                File.Delete(strCgmPath);
        //            }
        //            if (File.Exists(strImagePath))
        //            {
        //                File.Delete(strImagePath);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.This(ex);
        //    }
        //    return imgChem;
        //}

        #region Comments related methods        

        private DataTable GetCommentsTableDefinition()
        {
            DataTable dtComments = new DataTable();
            dtComments.Columns.Add("TAN_COMMENT", typeof(string));
            dtComments.Columns.Add("COMMENT_TYPE", typeof(string));
            return dtComments;
        }

        private DataTable GetCommentsTableFromTANComments(string comments)
        {
            DataTable dtComments = null;
            try
            {
                if (!string.IsNullOrEmpty(comments))
                {
                    Regex regEx;
                    string strCmnts = "";
                    string[] saCmnts = null;

                    dtComments = GetCommentsTableDefinition();
                    DataRow dtRow = null;

                    //CAS Comments
                    regEx = new Regex("<CE>(.*?)</CE>");
                    var vCE = regEx.Match(comments);
                    strCmnts = vCE.Groups[1].ToString();
                    if (!string.IsNullOrEmpty(strCmnts))
                    {
                        saCmnts = strCmnts.Split(new string[] { "~CE~" }, StringSplitOptions.RemoveEmptyEntries);
                        if (saCmnts != null)
                        {
                            for (int i = 0; i < saCmnts.Length; i++)
                            {
                                dtRow = dtComments.NewRow();
                                dtRow["TAN_COMMENT"] = saCmnts[i].Trim();
                                dtRow["COMMENT_TYPE"] = "CAS";
                                dtComments.Rows.Add(dtRow);
                            }
                        }
                    }

                    //INDEXING Error Comments                   
                    regEx = new Regex("<IE>(.*?)</IE>");
                    var vIE = regEx.Match(comments);
                    strCmnts = vIE.Groups[1].ToString();
                    if (!string.IsNullOrEmpty(strCmnts))
                    {
                        saCmnts = strCmnts.Split(new string[] { "~IE~" }, StringSplitOptions.RemoveEmptyEntries);
                        if (saCmnts != null)
                        {
                            for (int i = 0; i < saCmnts.Length; i++)
                            {
                                dtRow = dtComments.NewRow();
                                dtRow["TAN_COMMENT"] = saCmnts[i].Trim();
                                dtRow["COMMENT_TYPE"] = "INDEXING";
                                dtComments.Rows.Add(dtRow);
                            }
                        }
                    }

                    //Author Error Comments                   
                    regEx = new Regex("<AE>(.*?)</AE>");
                    var vAE = regEx.Match(comments);
                    strCmnts = vAE.Groups[1].ToString();
                    if (!string.IsNullOrEmpty(strCmnts))
                    {
                        saCmnts = strCmnts.Split(new string[] { "~AE~" }, StringSplitOptions.RemoveEmptyEntries);
                        if (saCmnts != null)
                        {
                            for (int i = 0; i < saCmnts.Length; i++)
                            {
                                dtRow = dtComments.NewRow();
                                dtRow["TAN_COMMENT"] = saCmnts[i].Trim();
                                dtRow["COMMENT_TYPE"] = "AUTHOR";
                                dtComments.Rows.Add(dtRow);
                            }
                        }
                    }

                    //Other Comments 
                    if (comments.Trim().Contains("<OE>") && comments.Trim().Contains("</OE>"))
                    {
                        regEx = new Regex("<OE>(.*?)</OE>");
                        var vOE = regEx.Match(comments);
                        strCmnts = vOE.Groups[1].ToString();
                        if (!string.IsNullOrEmpty(strCmnts))
                        {
                            saCmnts = strCmnts.Split(new string[] { "~OE~" }, StringSplitOptions.RemoveEmptyEntries);
                            if (saCmnts != null)
                            {
                                for (int i = 0; i < saCmnts.Length; i++)
                                {
                                    string strDefalutCmnt = GetRequiredCommentsFromOtherCommentsString_New(saCmnts[i].Trim(), "DEFAULT");
                                    if (!string.IsNullOrEmpty(strDefalutCmnt.Trim()))
                                    {
                                        dtRow = dtComments.NewRow();
                                        dtRow["TAN_COMMENT"] = strDefalutCmnt.Trim();
                                        dtRow["COMMENT_TYPE"] = "DEFAULT";
                                        dtComments.Rows.Add(dtRow);
                                    }
                                    string strTempCmnt = GetRequiredCommentsFromOtherCommentsString_New(saCmnts[i].Trim(), "TEMPERATURE");
                                    if (!string.IsNullOrEmpty(strTempCmnt.Trim()))
                                    {
                                        dtRow = dtComments.NewRow();
                                        dtRow["TAN_COMMENT"] = strTempCmnt.Trim();
                                        dtRow["COMMENT_TYPE"] = "TEMPERATURE";
                                        dtComments.Rows.Add(dtRow);
                                    }
                                    string strOtherCmnt = GetRequiredCommentsFromOtherCommentsString_New(saCmnts[i].Trim(), "OTHER");
                                    if (!string.IsNullOrEmpty(strOtherCmnt.Trim()))
                                    {
                                        dtRow = dtComments.NewRow();
                                        dtRow["TAN_COMMENT"] = strOtherCmnt.Trim();
                                        dtRow["COMMENT_TYPE"] = "OTHER";
                                        dtComments.Rows.Add(dtRow);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        string strDefalutCmnt = GetRequiredCommentsFromOtherCommentsString_New(comments.Trim(), "DEFAULT");
                        if (!string.IsNullOrEmpty(strDefalutCmnt.Trim()))
                        {
                            dtRow = dtComments.NewRow();
                            dtRow["TAN_COMMENT"] = strDefalutCmnt.Trim();
                            dtRow["COMMENT_TYPE"] = "DEFAULT";
                            dtComments.Rows.Add(dtRow);
                        }
                        string strTempCmnt = GetRequiredCommentsFromOtherCommentsString_New(comments.Trim(), "TEMPERATURE");
                        if (!string.IsNullOrEmpty(strTempCmnt.Trim()))
                        {
                            dtRow = dtComments.NewRow();
                            dtRow["TAN_COMMENT"] = strTempCmnt.Trim();
                            dtRow["COMMENT_TYPE"] = "TEMPERATURE";
                            dtComments.Rows.Add(dtRow);
                        }
                        string strOtherCmnt = GetRequiredCommentsFromOtherCommentsString_New(comments.Trim(), "OTHER");
                        if (!string.IsNullOrEmpty(strOtherCmnt.Trim()))
                        {
                            dtRow = dtComments.NewRow();
                            dtRow["TAN_COMMENT"] = strOtherCmnt.Trim();
                            dtRow["COMMENT_TYPE"] = "OTHER";
                            dtComments.Rows.Add(dtRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return dtComments;
        }

        private string GetRequiredCommentsFromOtherCommentsString_New(string comments, string comments_type)
        {
            string strComments = "";
            try
            {
                if (comments.Trim().Contains("~~") && comments.Trim().Contains("`"))//Default~~Temperature`Other
                {
                    Regex rgOther = new Regex("^(?<defltcmnts>[^~]*)~~(?<tmpcmnts>[^`]*)`(?<usrcmnts>.*)$");
                    var vOUC = rgOther.Match(comments);
                    if (vOUC.Groups.Count == 4)
                    {
                        if (comments_type.ToUpper() == "DEFAULT")//Default Comments
                        {
                            strComments = vOUC.Groups["defltcmnts"].Value.ToString().Trim();
                        }
                        if (comments_type.ToUpper() == "TEMPERATURE")//Temperature Comments
                        {
                            strComments = vOUC.Groups["tmpcmnts"].Value.ToString().Trim();
                        }
                        if (comments_type.ToUpper() == "OTHER")//Other Comments
                        {
                            strComments = vOUC.Groups["usrcmnts"].Value.ToString().Trim();
                        }
                    }
                    else//Old TAN comments
                    {
                        if (comments_type.ToUpper() == "OTHER")
                            strComments = comments.Trim();
                    }
                }
                else if (comments.Trim().Contains("~~") && !comments.Trim().Contains("`"))//Default~~Temperature
                {
                    string[] saCmnts = comments.Trim().Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
                    if (saCmnts != null)
                    {
                        if (saCmnts.Length == 2)
                        {
                            if (comments_type.ToUpper() == "DEFAULT")//Default Comments
                            {
                                strComments = saCmnts[0].Trim();
                            }
                            else if (comments_type.ToUpper() == "TEMPERATURE")//Temperature Comments
                            {
                                strComments = saCmnts[1].Trim();
                            }
                        }
                        else if (saCmnts.Length == 1)
                        {
                            if (comments_type.ToUpper() == "TEMPERATURE")//Temperature Comments
                            {
                                strComments = saCmnts[0].Trim();
                            }
                        }
                    }
                }
                else if (!comments.Trim().Contains("~~") && comments.Trim().Contains("`"))//Default`Other
                {
                    string[] saCmnts = comments.Trim().Split(new string[] { "`" }, StringSplitOptions.RemoveEmptyEntries);
                    if (saCmnts != null)
                    {
                        if (saCmnts.Length == 2)
                        {
                            if (comments_type.ToUpper() == "DEFAULT")//Default Comments
                            {
                                strComments = saCmnts[0].Trim();
                            }
                            else if (comments_type.ToUpper() == "OTHER")//Other Comments
                            {
                                strComments = saCmnts[1].Trim();
                            }
                        }
                        else if (saCmnts.Length == 1)
                        {
                            if (comments_type.ToUpper() == "OTHER")//Other Comments
                            {
                                strComments = saCmnts[0].Trim();
                            }
                        }
                    }
                }
                else
                {
                    if (comments_type.ToUpper() == "OTHER")//Other Comments
                    {
                        strComments = comments.Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strComments;
        }

        private string GetCommentsForPdfFromTable(DataTable commentsData)
        {
            string strComments = "";
            try
            {
                if (commentsData != null && commentsData.Rows.Count > 0)
                {
                    for (int i = 0; i < commentsData.Rows.Count; i++)
                    {
                        strComments = strComments.Trim() + "\r\n" + commentsData.Rows[i]["COMMENT_TYPE"].ToString() + ": " + commentsData.Rows[i]["TAN_COMMENT"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return strComments.Trim();
        }

        private RichTextBox BindCommentsDataToRichTextBox(string comments)
        {
            RichTextBox rtbComments = null;
            try
            {
                if (!string.IsNullOrEmpty(comments))
                {
                    rtbComments = new RichTextBox();
                    rtbComments.Text = comments;

                    //Replace New line with junk characters before coloring and replace the same with new line after coloring
                    rtbComments.Rtf = rtbComments.Rtf.Replace("\r\n", "_|0|_");
                    ColourCommentsRichTextBoxText(rtbComments);
                    rtbComments.Rtf = rtbComments.Rtf.Replace("_|0|_", "\r\n");
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return rtbComments;
        }

        private void ColourCommentsRichTextBoxText(RichTextBox rtb)
        {
            try
            {
                Regex rgeOther = new Regex("OTHER:");
                foreach (Match match in rgeOther.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.DarkOrange;
                }

                Regex rgeDefault = new Regex("DEFAULT:");
                foreach (Match match in rgeDefault.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.Red;
                }

                Regex rgeIndexing = new Regex("INDEXING:");
                foreach (Match match in rgeIndexing.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.LimeGreen;
                }

                Regex rgeAuthor = new Regex("AUTHOR:");
                foreach (Match match in rgeAuthor.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.DarkViolet;
                }

                Regex rgeTemp = new Regex("TEMPERATURE:");
                foreach (Match match in rgeTemp.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.DeepPink;
                }

                Regex rgCASCmnts = new Regex("CAS:");
                foreach (Match match in rgCASCmnts.Matches(rtb.Text))
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }

        #endregion

        #endregion
    }
}
