using Client.Common;
using Client.Logging;
using Client.XML;
using DTO;
using Entities;
using MDL.Draw.Renderer;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace Client.Util
{
    public static class ImageUtil
    {
        private static Renderer renderer = new Renderer();
        public static BitmapImage GetImage(ChemicalType ChemicalType, String ImagePath, String RegNumber, string MolString, bool isLarge = false)
        {

            try
            {
                if (ChemicalType == ChemicalType.NUM && !string.IsNullOrEmpty(ImagePath))
                {
                    string path = Path.Combine(C.SHAREDPATH, ImagePath);
                    if (File.Exists(path))
                        return BitMapToImage(new Bitmap(path), isLarge);
                }
                else if (ChemicalType == ChemicalType.S8000 && !string.IsNullOrEmpty(MolString))
                    return MolToBitmapImage(MolString, isLarge);
                else if (ChemicalType == ChemicalType.S8500 || ChemicalType == ChemicalType.S9000)
                {
                    string path = S.MolImagePath(RegNumber);
                    if (File.Exists(path))
                        return BitMapToImage(new Bitmap(path), isLarge);
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }


        public static iTextSharp.text.Image GetItextImageFromPath(ChemicalType ChemicalType, String ImagePath)
        {
            iTextSharp.text.Image imgChem = null;
            try
            {
                if (ChemicalType == ChemicalType.NUM && !string.IsNullOrEmpty(ImagePath))
                {
                    string path = Path.Combine(C.SHAREDPATH, ImagePath);
                    if (File.Exists(path))
                    {
                        string TempPath = Path.Combine(Path.GetTempPath(), "tempImage.gif");
                        if (File.Exists(TempPath))
                            File.Delete(TempPath);
                        File.Copy(path, TempPath);
                        using (FileStream fstream = File.Open(TempPath, FileMode.Open, FileAccess.Read))
                        {
                            BinaryReader br = new BinaryReader(fstream);
                            byte[] imgData = br.ReadBytes((int)fstream.Length);
                            using (MemoryStream stream = new MemoryStream(imgData))
                            {
                                imgChem = iTextSharp.text.Image.GetInstance(stream);
                            }
                            br.Close();
                        }
                        if (File.Exists(TempPath))
                            File.Delete(TempPath);
                    }

                    //Resize image depend upon your need
                    imgChem.ScaleToFit(280f, 260f);
                    //Give space before image
                    imgChem.SpacingBefore = 30f;
                    //Give some space after the image
                    imgChem.SpacingAfter = 1f;
                    imgChem.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return imgChem;
        }

        public static BitmapImage MolToBitmapImage(string molString, bool isLarge = false)
        {

            try
            {
                renderer.MolfileString = molString;
                return BitMapToImage(renderer.Image, isLarge);
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }

        private static BitmapImage BitMapToImage(Bitmap sourceBitmap, bool isLarge = false)
        {

            try
            {
                var bitmap = isLarge ? FixedSize(sourceBitmap, 400, 400) : FixedSize(sourceBitmap, 196, 196);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    var format = System.Drawing.Imaging.ImageFormat.Gif;
                    bitmap.Save(memoryStream, format);
                    memoryStream.Position = 0;
                    BitmapImage bitmapimage = new BitmapImage();
                    bitmapimage.BeginInit();
                    bitmapimage.StreamSource = memoryStream;
                    bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapimage.EndInit();
                    return bitmapimage;
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }

        public static Image FixedSize(Image imgPhoto, int Width, int Height)
        {

            try
            {
                Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
                int sourceWidth = imgPhoto.Width;
                int sourceHeight = imgPhoto.Height;
                int sourceX = 0;
                int sourceY = 0;
                int destX = 0;
                int destY = 0;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = ((float)Width / (float)sourceWidth);
                nPercentH = ((float)Height / (float)sourceHeight);
                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentH;
                    destX = System.Convert.ToInt16((Width -
                                  (sourceWidth * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentW;
                    destY = System.Convert.ToInt16((Height -
                                  (sourceHeight * nPercent)) / 2);
                }

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                                 imgPhoto.VerticalResolution);

                Graphics grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.Clear(Color.MintCream);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto,
                    new Rectangle(destX, destY, destWidth, destHeight),
                    new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                    GraphicsUnit.Pixel);

                grPhoto.Dispose();
                return bmPhoto;
            }
            catch (Exception ex)
            {
                Log.This(ex);
                return null;
            }
        }
    }
}
