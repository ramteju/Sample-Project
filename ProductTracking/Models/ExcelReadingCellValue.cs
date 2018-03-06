using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ProductTracking.Models
{
    public class ExcelReadingCellValue
    {
        public static string value;
        public static int count;
        /// <summary>
        /// Get cell value by passing below params
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="sheetName"></param>
        /// <param name="addressName"></param>
        /// <author name="Varaprasad"></author>
        /// <returns></returns>
        public static string GetCellValue(string fileName, string sheetName, string addressName)
        {
            try
            {
                string[] excelcolumn = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
                value = null;
                count = 0;
                string value1 = null;

                // Open the spreadsheet document for read-only access.
                using (SpreadsheetDocument document =
                    SpreadsheetDocument.Open(fileName, false))
                {
                    // Retrieve a reference to the workbook part.
                    WorkbookPart wbPart = document.WorkbookPart;

                    // Find the sheet with the supplied name, and then use that 
                    // Sheet object to retrieve a reference to the first worksheet.
                    Sheet theSheet = wbPart.Workbook.Descendants<Sheet>().
                      Where(s => s.Name == sheetName).FirstOrDefault();

                    // Throw an exception if there is no sheet.
                    if (theSheet == null)
                    {
                        throw new ArgumentException("sheetName");
                    }

                    // Retrieve a reference to the worksheet part.
                    WorksheetPart wsPart =
                        (WorksheetPart)(wbPart.GetPartById(theSheet.Id));

                    // Use its Worksheet property to get a reference to the cell 
                    // whose address matches the address you supplied.
                    Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                      Where(c => c.CellReference == addressName).FirstOrDefault();





                    // If the cell does not exist, return an empty string.
                    if (theCell != null)
                    {
                       
                         value = theCell.InnerText;
                        if (addressName.Contains("F"))
                        {
                            value = DateTime.FromOADate(double.Parse(value)).ToString("dd/MM/yy");
                        }
                        //getting merged cells from the sheet
                        count = wsPart.Worksheet.Elements<MergeCells>().Count();

                        if (count > 0)
                        {

                            MergeCells mergedCells = wsPart.Worksheet.Elements<MergeCells>().First();
                            foreach (MergeCell mc in mergedCells)
                            {
                                var ltr = new List<string>();
                                var num = new List<int>();
                                string cell = mc.Reference.InnerText;
                                string[] letter = cell.Split(':').ToArray();
                                foreach (string l in letter)
                                {
                                    var numAlpha = new Regex("(?<Alpha>[a-zA-Z]*)(?<Numeric>[0-9]*)");
                                    var match = numAlpha.Match(l);
                                    var alpha = match.Groups["Alpha"].Value;
                                    var number = match.Groups["Numeric"].Value;
                                    ltr.Add(alpha);
                                    num.Add(int.Parse(number));
                                }

                                int ltrfistin = Array.IndexOf(excelcolumn, ltr[0]);
                                int ltrlsttin = Array.IndexOf(excelcolumn, ltr[ltr.Count - 1]);
                                int numfirstin = num[0];
                                int numlstin = num[num.Count - 1];
                                //buiding string for cell addresss
                                StringBuilder builder = new StringBuilder();
                                for (int i = ltrfistin; i <= ltrlsttin; i++)
                                {
                                    for (int j = numfirstin; j <= numlstin; j++)
                                    {
                                        builder.Append(excelcolumn[i]).Append(j).Append(":");
                                    }
                                }

                                string cellfulladdress = builder.ToString().TrimEnd(':');


                                if (cellfulladdress.Contains(addressName))
                                {
                                    string celaddress = cellfulladdress;
                                    string rffaddress = celaddress.Split(':').First();
                                    // if (mc.Reference.InnerText.StartsWith(addressName))
                                    if (celaddress.Contains(addressName))
                                    {

                                        theCell = wsPart.Worksheet.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>().
                                           Where(c => c.CellReference == rffaddress).FirstOrDefault();
                                        value1 = GetCellValue1(document, theCell);
                                    }
                                }

                            }
                        }



                        //value =  theCell.CellValue;
                        // value = theCell.LastChild.InnerText;

                        // If the cell represents an integer number, you are done. 
                        // For dates, this code returns the serialized value that 
                        // represents the date. The code handles strings and 
                        // Booleans individually. For shared strings, the code 
                        // looks up the corresponding value in the shared string 
                        // table. For Booleans, the code converts the value into 
                        // the words TRUE or FALSE.
                        if (theCell.DataType != null)
                        {
                            switch (theCell.DataType.Value)
                            {
                                case CellValues.SharedString:

                                    // For shared strings, look up the value in the
                                    // shared strings table.
                                    var stringTable =
                                        wbPart.GetPartsOfType<SharedStringTablePart>()
                                        .FirstOrDefault();

                                    // If the shared string table is missing, something 
                                    // is wrong. Return the index that is in
                                    // the cell. Otherwise, look up the correct text in 
                                    // the table.
                                    if (stringTable != null)
                                    {
                                        value1 =
                                            stringTable.SharedStringTable
                                            .ElementAt(int.Parse(value)).InnerText;
                                    }
                                    break;

                                case CellValues.Boolean:
                                    switch (value)
                                    {
                                        case "0":
                                            value = "FALSE";
                                            break;
                                        default:
                                            value = "TRUE";
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                if (value1 != null)
                {
                    return value1;
                }
                else
                {

                    return value;

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
        /// <summary>
        /// Getcell value of Merged cells by passing document
        /// </summary>
        /// <param name="document"></param>
        /// <param name="cell"></param>
        /// <author name="Varaprasad"></author>
        /// <returns></returns>
        public static string GetCellValue1(SpreadsheetDocument document, DocumentFormat.OpenXml.Spreadsheet.Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value2 = string.Empty;
            if (cell.CellValue != null)
            {
                value2 = cell.CellValue.InnerXml;
                value = value2;
                if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                {
                    value2 = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                }

            }
            return value2;
        }
    }
}