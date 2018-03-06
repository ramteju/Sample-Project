using Client.Logging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client.Common
{
    public static class PdfAnnotsReplacer
    {
        private static PdfName subTypeName = new PdfName("Subtype");
        public static void ApplyProperties(string inputPdfPath, string outputPdfPath)
        {
            try
            {
                string author = string.Empty;
                using (PdfReader reader = new PdfReader(inputPdfPath))
                {

                    PdfStamper stamp = new PdfStamper(reader, new FileStream(outputPdfPath, FileMode.Create), reader.PdfVersion);
                    List<AnnotationMark> marks = new List<AnnotationMark>();
                    for (int pageIndex = 1; pageIndex <= reader.NumberOfPages; pageIndex++)
                    {
                        PdfDictionary pageDict = reader.GetPageN(pageIndex);
                        PdfArray annotArray = pageDict.GetAsArray(PdfName.ANNOTS);
                        //PdfArray removeannotarray 
                        PdfGState gstate = new PdfGState();
                        gstate.FillOpacity = 0.1f;
                        gstate.StrokeOpacity = (0.1f);
                        var pcb = stamp.GetOverContent(pageIndex);
                        pcb.SetGState(gstate);

                        if (annotArray != null)
                        {
                            List<int> ind = new List<int>();
                            List<string> strind = new List<string>();
                            List<int> rind = new List<int>();
                            for (int i = 0; i < annotArray.Count(); ++i)
                            {
                                PdfDictionary annotation = annotArray.GetAsDict(i);

                                if (annotation != null)
                                {
                                    PdfArray r = annotation.GetAsArray(PdfName.RECT);
                                    var subType = annotation.Get(subTypeName);
                                    if (subType != null && !String.IsNullOrEmpty(subType.ToString()) && subType.ToString() == "/FreeText")
                                    {
                                        var content = annotation.GetAsString(PdfName.CONTENTS);
                                        if (content != null && (content.ToString().ToUpper().Contains("RXN") || Regex.IsMatch(content.ToString(), S.S8000_MARKUP_REG_EXP)))
                                        {
                                            string status = content.ToString();

                                            PdfArray rect = annotation.GetAsArray(PdfName.RECT);
                                            var newRect = new iTextSharp.text.Rectangle(rect.GetAsNumber(0).FloatValue, rect.GetAsNumber(1).FloatValue, rect.GetAsNumber(2).FloatValue + 15, rect.GetAsNumber(3).FloatValue);
                                            pcb.SetFontAndSize(FontFactory.GetFont(FontFactory.COURIER).BaseFont, 12);
                                            var newAnnotation = PdfAnnotation.CreateFreeText(stamp.Writer, newRect, status.ToString(), pcb);
                                            if (newAnnotation != null)
                                            {
                                                newAnnotation.Put(PdfName.T, new PdfString(String.Empty));
                                                newAnnotation.Put(new PdfName("IT"), new PdfString("FreeTextTypeWriter"));
                                                newAnnotation.Put(new PdfName("DS"), new PdfString("font: Courier 12pt; text-align:left; margin:0pt; line-height:13.8pt; color:#FF0000"));
                                                newAnnotation.Border = new PdfBorderArray(0, 0, 0);
                                                newAnnotation.Put(new PdfName("DA"), new PdfString("0 0 0 rg /Cour 12 Tf"));
                                                newAnnotation.Put(new PdfName("Subj"), new PdfString("Typewritten Text"));
                                                newAnnotation.Put(new PdfName("RC"), new PdfString($"<?xml version=\"1.0\"?><body xmlns:xfa=\"http://www.xfa.org/schema/xfa-data/1.0/\" xfa:contentType=\"text/html\" xfa:APIVersion=\"BluebeamPDFRevu:12.1.0\" xfa:spec=\"2.2.0\" style=\"font:Courier 12pt; text-align:left; margin:0pt; line-height:13.8pt; color:#FF0000\" xmlns=\"http://www.w3.org/1999/xhtml\">{status.ToString()}</body>"));
                                                marks.Add(new AnnotationMark { Annotation = newAnnotation, PageIndex = pageIndex });
                                                rind.Add(i);
                                            }
                                        }
                                    }
                                }
                            }
                            for (int i = rind.Count(); i > 0; i--)
                                annotArray.Remove(rind[i - 1]);
                        }
                    }
                    foreach (var annotationMark in marks)
                        stamp.AddAnnotation(annotationMark.Annotation, annotationMark.PageIndex);
                    stamp.Close();
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
        }
    }

    class AnnotationMark
    {
        public int PageIndex { get; set; }
        public PdfAnnotation Annotation { get; set; }
    }

    public class Coordinate
    {
        public int PageIndex { get; set; }
        public string Content { get; set; }
        public int ForNo { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double x1 { get; set; }
        public double y1 { get; set; }
    }
}
