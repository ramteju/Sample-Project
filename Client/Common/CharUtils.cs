using System;
using System.Text;
using System.Windows;
using Entities;
using Client.Logging;

namespace Client.Common
{
    public class CharRange
    {
        public int from, to;
    }

    public class CharUtils
    {
        public static bool NotAValidUniCodeString(string inputstring)
        {
            bool blStatus = false;
            try
            {
                //Check Latin characters in String
                if (NotAValidLatinString(inputstring))
                {
                    blStatus = true;
                }
                else
                {
                    String originalString = inputstring;
                    Byte[] bytes = Encoding.UTF32.GetBytes(originalString);
                    uint codepoint = 0;

                    //U+0000 – U+0008(decimal is 0000-0008) (Hex is &#x0000; - &#x0008;) - Disallowed by Xml - Characters found: 9
                    //U+000B – U+000C(decimal is 0011-0012) (Hex is &#x000B; - &#x000C;) - Disallowed by Xml - Characters found: 2
                    //U+000E – U+001F (decimal is 0014-0031) (Hex is &#x000E; - &#x001F;) - Disallowed by Xml - Characters found: 18
                    //U+D800 - U+DBFF decimal range is 55296-56319 - Disallowed by Xml - Characters found: 1024
                    //U+DC00 - U+DFFF (decimal is 56320-57343)(Hex is &#xDC00; - &#xDFFF;)-Disallowed by Xml - Characters found: 1024

                    //U+E000 - U+F8FF (decimal is - 57344-63743) (Hex is - &#xE000; - &#xF8FF;) disallowed by CAS (Unicode Private Use Area)- Characters found: 6400
                    //U+007F - U+009F (decimal is 0127-0159) (Hex is &#x007F; - &#x009F;) - disallowed by CAS (control codes) -Characters found: 33
                    //U+10000 - U+10FFFF (decimal is 65536-0) (Hex is &#x10000; - ;) - disallowed by CAS (outside of Unicode Basic Multilingual Plane) -Characters found: -

                    //New validations on 11th Aug 2014
                    //U+2019 = '’' - Copy paste problem of ' - 8217
                    //U+2013 = '–' - Copy paste problem of - - 8211
                    //U+FB01 = 'ﬁ' - Copy paste problem of 'fi' - 64257, 64258
                    //U+2032 = '′'(Prime) - Copy paste problem of ' - 8242
                    //U+0080 - U+00FF  - (decimal is 128 - 255) Latin-1 Supplement 0080—00FF - Characters found 128

                    //U+0100 - U+017F - (Decimal 256 - 383) Latin Extended-A - 128 code points
                    //U+0180 - U+024F - (decimal is 384 - 591) Latin Extended-B - 208 code points
                    //U+0250 - U+02AF - (decimal is 592 - 687) IPA Extensions - 96 code points

                    //U+2C60 - U+2C7F - (decimal is 11360 - 11391) Latin Extended-C - 32 code points
                    //U+A720 - U+A7FF - (decimal is 42784 - 43007) Latin Extended-D - 224 code points
                    //U+AB30 - U+AB6F - (decimal is 43824 - 43887) Latin Extended-E - 64 code points

                    //U+1E00 - U+1EFF - (decimal is 7680 - 7935) Latin Extended Additional - 256 code points
                    //U+2150 - U+218F - (decimal is 8528 - 8591) Number Forms - 64 code points
                    //U+FB00 - U+FB4F - (decimal is 64256 - 64335) Alphabetic Presentation Forms - 80 code points
                    //U+0000 - U+007F - (decimal is 0 - 127) C0 Controls and Basic Latin - 128 code points


                    for (int idx = 0; idx < bytes.Length; idx += 4)
                    {
                        codepoint = 0;
                        codepoint = BitConverter.ToUInt32(bytes, idx);

                        char c = System.BitConverter.ToChar(bytes, idx);

                        //Not Allowed Unicodes
                        if ((codepoint >= 0000 && codepoint <= 0008) || (codepoint >= 0011 && codepoint <= 0012) || (codepoint >= 0014 && codepoint <= 0031) ||
                            (codepoint >= 55296 && codepoint <= 56319) || (codepoint >= 56320 && codepoint <= 57343) || (codepoint >= 57344 && codepoint <= 63743) ||
                            (codepoint >= 0127 && codepoint <= 0159) || (codepoint >= 65536) || ((codepoint >= 0128 && codepoint <= 0255) && codepoint != 176 && codepoint != 197) ||
                             codepoint == 8217 || codepoint == 8211 || codepoint == 64257 || codepoint == 8242)
                        {
                            blStatus = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return blStatus;
        }

        private static bool NotAValidLatinString(string inputstring)
        {
            bool blStatus = false;
            try
            {
                //U + 0000 - U + 007F - (decimal is 0 - 127) C0 Controls and Basic Latin - 128 code points
                //U + 0080 - U + 00FF - (decimal is 128 - 255) Latin - 1 Supplement 0080—00FF - Characters found 128
                //U + 0100 - U + 017F - (Decimal 256 - 383) Latin Extended-A - 128 code points
                //U + 0180 - U + 024F - (decimal is 384 - 591) Latin Extended-B - 208 code points
                //U + 0250 - U + 02AF - (decimal is 592 - 687) IPA Extensions -96 code points
                //U + 02B0 - U + 02FF(decimal is 688 - 767) Spacing Modifier Letters - 80 code points

                //U + 1D00 - U + 1D7F - (decinal is 7424 - 7551) Phonetic Extensions -128 code points
                //U + 1D80 - U + 1DBF - (decimal is 7552 - 7615)-Phonetic Extensions Supplement 64 code points
                //U + 1E00 - U + 1EFF - (decimal is 7680 - 7935) Latin Extended Additional - 256 code points
                //U + 2070 - U + 209F - (decimal is 8304 - 8351) Unicode subscripts and superscripts

                //U + 2100 - U + 214F - (decimal is 8448 - 8527) Letterlike Symbols -80 code points
                //U + 2150 - U + 218F - (decimal is 8528 - 8591) Number Forms -64 code points
                //U + 2C60 - U + 2C7F - (decimal is 11360 - 11391) Latin Extended-C - 32 code points
                //U + A720 - U + A7FF - (decimal is 42784 - 43007) Latin Extended-D - 224 code points
                //U + AB30 - U + AB6F - (decimal is 43824 - 43887) Latin Extended-E - 64 code points
                //U + FB00 - U + FB4F - (decimal is 64256 - 64335) Alphabetic Presentation Forms - 80 code points
                //U + FF00 – U + FFEF - (decimal is 65280 - 65519) Halfwidth and fullwidth forms

                String originalString = inputstring;
                Byte[] bytes = Encoding.UTF32.GetBytes(originalString);
                uint codepoint = 0;

                for (int idx = 0; idx < bytes.Length; idx += 4)
                {
                    codepoint = 0;
                    codepoint = BitConverter.ToUInt32(bytes, idx);

                    char c = System.BitConverter.ToChar(bytes, idx);

                    //Not Allowed Unicodes
                    if ((codepoint >= 128 && codepoint <= 767) || (codepoint >= 7424 && codepoint <= 7615) || (codepoint >= 7680 && codepoint <= 7935) ||
                        (codepoint >= 8304 && codepoint <= 8351) || (codepoint >= 8448 && codepoint <= 8591) || (codepoint >= 11360 && codepoint <= 11391) ||
                        (codepoint >= 42784 && codepoint <= 43007) || (codepoint >= 43824 && codepoint <= 43887) || (codepoint >= 64256 && codepoint <= 64335) ||
                        (codepoint >= 65280 && codepoint <= 65519))
                    {
                        blStatus = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.This(ex);
            }
            return blStatus;
        }
    }
}
