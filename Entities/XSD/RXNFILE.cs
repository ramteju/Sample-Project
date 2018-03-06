using System.Xml.Serialization;

namespace xsd
{
    public class RXNFILE
    {
        public string SOURCE;
        public int FILENUM;
        public int VERSION;
        [XmlElement("DOCUMENT")]
        public DOCUMENT[] DOCUMENT;
    }
    public class DOCUMENT
    {
        public string VIEW;
        public string CAN;
        public string TAN;
        public string ANALYST;
        public string COMMENTS;
        public RXNGRP RXNGRP;
    }
    public class RXNGRP
    {
        [XmlElement("RXN")]
        public RXN[] RXN;
    }
    public class RXN
    {
        public RXNID RXNID;
        public string RSD;
        public XREFGRP XREFGRP;
        public SUBDESC SUBDESC;
        public RXNPROCESS RXNPROCESS;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int NO;
    }
    public class RXNID
    {
        public int RXNNUM;
        public int RXNSEQ;
    }
    public class RXNPROCESS
    {
        [XmlElement("RSN")]
        public RSN[] RSN;
        [XmlElement("STAGE")]
        public STAGE[] STAGE;
    }
    public class RSN
    {
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TYPE;
    }
    public class STAGE
    {
        [XmlElement("SUBSTAGE")]
        public SUBSTAGE[] SUBSTAGE;
    }
    public class SUBSTAGE
    {
        [XmlElement("COND")]
        public COND[] COND;
    }
    public class COND
    {
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value;
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string TYPE;
    }
    public class SUBDESC
    {
        [XmlElement("SUBDEFN")]
        public SUBDEFN[] SUBDEFN;
    }
    public class SUBDEFN
    {

        public string NRNNUM;
        public string SUBLOC;
        public string SUBNAME;


    }
    public class XREFGRP
    {
        [XmlElement("NRN")]
        public NRN[] NRN;
    }
    public class NRN
    {
        public int NRNNUM;
        public int NRNREG;
    }
    public static class CondType
    {
        public static readonly string TM = "TM";
        public static readonly string TP = "TP";
        public static readonly string PR = "PR";
        public static readonly string PH = "PH";
    }    
}
