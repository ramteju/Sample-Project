using Client.Logging;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Common
{
    public static class PdfMerger
    {
        public static bool CombineMultiplePDFs(string[] fileNames,string TargetFile)
        {
            try
            {
                string outFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SamplePDFFolder");
                if (!Directory.Exists(outFile))
                {
                    Directory.CreateDirectory(outFile);
                }
                outFile = Path.Combine(outFile, "MergedPdf.pdf");
                // step 1: creation of a document-object
                Document document = new Document();

                // step 2: we create a writer that listens to the document
                PdfCopy writer = new PdfCopy(document, new FileStream(outFile, FileMode.Create));
                if (writer == null)
                {
                    return false;
                }

                // step 3: we open the document
                document.Open();

                foreach (string fileName in fileNames)
                {
                    // we create a reader for a certain document
                    PdfReader reader = new PdfReader(fileName);
                    reader.ConsolidateNamedDestinations();

                    // step 4: we add content
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        writer.AddPage(page);
                    }

                    PRAcroForm form = reader.AcroForm;
                    if (form != null)
                    {
                        writer.CopyAcroForm(reader);
                    }

                    reader.Close();
                }

                // step 5: we close the document and writer
                writer.Close();
                document.Close();

                File.Copy(outFile, TargetFile, true);
                File.Delete(outFile);
                return true;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return false;
            }
        }
    }
}
