using Entities;
using iTextSharp.text.pdf.parser;
using ProductTracking.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductTracking.Models
{
    public class AppLocationTextExtractionStrategy : LocationTextExtractionStrategy
    {
        public int pageNumber { get; set; }
        public StringBuilder textResult { get; set; }

        public AppLocationTextExtractionStrategy()
        {
            textResult = new StringBuilder();
        }

        //Automatically called for each chunk of text in the PDF
        public override void RenderText(TextRenderInfo renderInfo)
        {
            try
            {
                base.RenderText(renderInfo);
                textResult.Append(renderInfo.GetText().ToUpper());
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
