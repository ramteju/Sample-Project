using System;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Diagnostics;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Zips2Pdf
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            string xmlPath = @"D:\Documents\Reactions\bat.800426\bat.800426.sgml\rxnfile.800426.cgm";
            //Zips2Pdf();
            Environment.Exit(0);
        }

        void Zips2Pdf()
        {
            string source = @"D:\Documents\Reactions\bat.800426";
            string destination = @"D:\Documents\Reactions\Shipments\800426";

            if (!@Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            var imageZipPaths = Directory.GetFiles(source, "*.images.*");
            foreach (var imageZipPath in imageZipPaths)
            {
                using (ZipArchive zip = ZipFile.Open(imageZipPath, ZipArchiveMode.Read))
                {
                    var tifEntries = zip.Entries.Where(ze => ze.FullName.ToLower().EndsWith(".tif"));
                    var tanNumberWiseTifs = tifEntries.GroupBy(te => te.FullName.Substring(0, 9)).ToDictionary(d => d.Key, d => d.ToList());
                    foreach (var tanNumberWiseEntry in tanNumberWiseTifs)
                    {
                        string tanNumber = tanNumberWiseEntry.Key;
                        string pdfPath = Path.Combine(destination, tanNumber + ".pdf");
                        using (var pdfStream = new FileStream(pdfPath, FileMode.OpenOrCreate, FileAccess.Write))
                        using (var doc = new Document())
                        {
                            PdfWriter.GetInstance(doc, pdfStream);
                            doc.Open();
                            int i = 0;
                            foreach (var tifEntry in tanNumberWiseEntry.Value)
                            {
                                using (var tifStream = tifEntry.Open())
                                {
                                    i++;
                                    var docImage = Image.GetInstance(tifStream);
                                    docImage.ScaleToFit(doc.PageSize.Width, doc.PageSize.Height);
                                    doc.Add(docImage);

                                    if (i + 1 < tanNumberWiseEntry.Value.Count())
                                        doc.NewPage();
                                }
                            }
                            doc.Close();
                        }
                    }
                }
            }
        }
    }
}
