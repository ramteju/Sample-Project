using System;

namespace ProductTracking.Models.Core
{
    public class IndexingErrors
    {
        public Guid Id { get; set; }
        public int IndexNum { get; set; }
        public int IndexPage { get; set; }
        public string IndexPara { get; set; }
        public int IndexLine { get; set; }
        public int IndexColumn { get; set; }
        public string IndexTable { get; set; }
        public string IndexFigure { get; set; }
        public string IndexSchemes { get; set; }
        public string IndexSheet { get; set; }
        public string IndexFootNote { get; set; }
        public string IndexComment { get; set; }
        public string FinalComment { get; set; }

    }
}
