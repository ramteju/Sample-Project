using System;


namespace ProductTracking.Models.Core
{
    public class AuthorErrors
    {
        public Guid Id { get; set; }
        public int AuthorNum { get; set; }
        public int AuthorPage { get; set; }
        public string AuthorPara { get; set; }
        public int AuthorLine { get; set; }
        public int AuthorColumn { get; set; }
        public string AuthorTable { get; set; }
        public string AuthorFigure { get; set; }
        public string AuthorSchemes { get; set; }
        public string AuthorSheet { get; set; }
        public string AuthorFootNote { get; set; }
        public string AuthorComment { get; set; }
        public string FinalComment { get; set; }

    }

    public class OtherErrors
    {
        public Guid Id { get; set; }
        public string OtherComment { get; set; }
    }

    public class TemperatureErrors
    {
        public Guid Id { get; set; }
        public string TemperatureComment { get; set; }
    }

    public class DefaultErrors
    {
        public Guid Id { get; set; }
        public string DefaultComment { get; set; }
    }
    public class CasConsultedForErrors
    {
        public Guid Id { get; set; }
        public string CasConsultedComment { get; set; }
    }
}

