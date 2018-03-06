using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductTracking.Models
{
    public class Substance
    {
        public String RegisterNumber { get; set; }
        public String Formula { get; set; }
        public String IUPAC { get; set; }
        public String StereoMessage { get; set; }
        public String PeptideSequence { get; set; }
        public String NuclicAcid { get; set; }
        public String OtherName { get; set; }
        public String SIM { get; set; }
        public List<SubstanceImagePaths> ListImagepaths { get; set; }
    }
    public enum ArticleType
    {
        Journal,
        Patent,
        Conference,
        PrePrint
    }
}