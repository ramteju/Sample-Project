﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class DOCUMENT
{

    private viewType vIEWField;

    private string cANField;

    private string tANField;

    private string aNALYSTField;

    private string cOMMENTSField;

    private RXNGRP rXNGRPField;

    /// <remarks/>
    public viewType VIEW
    {
        get
        {
            return this.vIEWField;
        }
        set
        {
            this.vIEWField = value;
        }
    }

    /// <remarks/>
    public string CAN
    {
        get
        {
            return this.cANField;
        }
        set
        {
            this.cANField = value;
        }
    }

    /// <remarks/>
    public string TAN
    {
        get
        {
            return this.tANField;
        }
        set
        {
            this.tANField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    public string ANALYST
    {
        get
        {
            return this.aNALYSTField;
        }
        set
        {
            this.aNALYSTField = value;
        }
    }

    /// <remarks/>
    public string COMMENTS
    {
        get
        {
            return this.cOMMENTSField;
        }
        set
        {
            this.cOMMENTSField = value;
        }
    }

    /// <remarks/>
    public RXNGRP RXNGRP
    {
        get
        {
            return this.rXNGRPField;
        }
        set
        {
            this.rXNGRPField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "CAS_React_Schema.xsd")]
public enum viewType
{

    /// <remarks/>
    RXN,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class RXNGRP
{

    private RXN[] rXNField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("RXN")]
    public RXN[] RXN
    {
        get
        {
            return this.rXNField;
        }
        set
        {
            this.rXNField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class RXN
{

    private RXNID rXNIDField;

    private string rSDField;

    private XREFGRP xREFGRPField;

    private SUBDESC sUBDESCField;

    private RXNPROCESS rXNPROCESSField;

    private int noField;

    private bool noFieldSpecified;

    /// <remarks/>
    public RXNID RXNID
    {
        get
        {
            return this.rXNIDField;
        }
        set
        {
            this.rXNIDField = value;
        }
    }

    /// <remarks/>
    public string RSD
    {
        get
        {
            return this.rSDField;
        }
        set
        {
            this.rSDField = value;
        }
    }

    /// <remarks/>
    public XREFGRP XREFGRP
    {
        get
        {
            return this.xREFGRPField;
        }
        set
        {
            this.xREFGRPField = value;
        }
    }

    /// <remarks/>
    public SUBDESC SUBDESC
    {
        get
        {
            return this.sUBDESCField;
        }
        set
        {
            this.sUBDESCField = value;
        }
    }

    /// <remarks/>
    public RXNPROCESS RXNPROCESS
    {
        get
        {
            return this.rXNPROCESSField;
        }
        set
        {
            this.rXNPROCESSField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int NO
    {
        get
        {
            return this.noField;
        }
        set
        {
            this.noField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool NOSpecified
    {
        get
        {
            return this.noFieldSpecified;
        }
        set
        {
            this.noFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class RXNID
{

    private string rXNNUMField;

    private string rXNSEQField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    public string RXNNUM
    {
        get
        {
            return this.rXNNUMField;
        }
        set
        {
            this.rXNNUMField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    public string RXNSEQ
    {
        get
        {
            return this.rXNSEQField;
        }
        set
        {
            this.rXNSEQField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class XREFGRP
{

    private XREFGRPNRN[] nRNField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("NRN")]
    public XREFGRPNRN[] NRN
    {
        get
        {
            return this.nRNField;
        }
        set
        {
            this.nRNField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
public partial class XREFGRPNRN
{

    private string nRNNUMField;

    private string nRNREGField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    public string NRNNUM
    {
        get
        {
            return this.nRNNUMField;
        }
        set
        {
            this.nRNNUMField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    public string NRNREG
    {
        get
        {
            return this.nRNREGField;
        }
        set
        {
            this.nRNREGField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class SUBDESC
{

    private SUBDESCSUBDEFN[] sUBDEFNField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SUBDEFN")]
    public SUBDESCSUBDEFN[] SUBDEFN
    {
        get
        {
            return this.sUBDEFNField;
        }
        set
        {
            this.sUBDEFNField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
public partial class SUBDESCSUBDEFN
{

    private string nRNNUMField;

    private string sUBLOCField;

    private string sUBNAMEField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    public string NRNNUM
    {
        get
        {
            return this.nRNNUMField;
        }
        set
        {
            this.nRNNUMField = value;
        }
    }

    /// <remarks/>
    public string SUBLOC
    {
        get
        {
            return this.sUBLOCField;
        }
        set
        {
            this.sUBLOCField = value;
        }
    }

    /// <remarks/>
    public string SUBNAME
    {
        get
        {
            return this.sUBNAMEField;
        }
        set
        {
            this.sUBNAMEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class RXNPROCESS
{

    private RSN[] rSNField;

    private STAGE[] sTAGEField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("RSN")]
    public RSN[] RSN
    {
        get
        {
            return this.rSNField;
        }
        set
        {
            this.rSNField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("STAGE")]
    public STAGE[] STAGE
    {
        get
        {
            return this.sTAGEField;
        }
        set
        {
            this.sTAGEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class RSN
{

    private string tYPEField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string TYPE
    {
        get
        {
            return this.tYPEField;
        }
        set
        {
            this.tYPEField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string[] Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class STAGE
{

    private RSN[] rSNField;

    private SUBSTAGE[] sUBSTAGEField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("RSN")]
    public RSN[] RSN
    {
        get
        {
            return this.rSNField;
        }
        set
        {
            this.rSNField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SUBSTAGE")]
    public SUBSTAGE[] SUBSTAGE
    {
        get
        {
            return this.sUBSTAGEField;
        }
        set
        {
            this.sUBSTAGEField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class SUBSTAGE
{

    private COND[] cONDField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("COND")]
    public COND[] COND
    {
        get
        {
            return this.cONDField;
        }
        set
        {
            this.cONDField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class COND
{

    private condType tYPEField;

    private bool tYPEFieldSpecified;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public condType TYPE
    {
        get
        {
            return this.tYPEField;
        }
        set
        {
            this.tYPEField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TYPESpecified
    {
        get
        {
            return this.tYPEFieldSpecified;
        }
        set
        {
            this.tYPEFieldSpecified = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string[] Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace = "CAS_React_Schema.xsd")]
public enum condType
{

    /// <remarks/>
    TM,

    /// <remarks/>
    TP,

    /// <remarks/>
    PR,

    /// <remarks/>
    PH,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "CAS_React_Schema.xsd")]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "CAS_React_Schema.xsd", IsNullable = false)]
public partial class RXNFILE
{

    private string sOURCEField;

    private string fILENUMField;

    private string vERSIONField;

    private DOCUMENT[] dOCUMENTField;

    /// <remarks/>
    public string SOURCE
    {
        get
        {
            return this.sOURCEField;
        }
        set
        {
            this.sOURCEField = value;
        }
    }

    /// <remarks/>
    public string FILENUM
    {
        get
        {
            return this.fILENUMField;
        }
        set
        {
            this.fILENUMField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType = "positiveInteger")]
    public string VERSION
    {
        get
        {
            return this.vERSIONField;
        }
        set
        {
            this.vERSIONField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("DOCUMENT")]
    public DOCUMENT[] DOCUMENT
    {
        get
        {
            return this.dOCUMENTField;
        }
        set
        {
            this.dOCUMENTField = value;
        }
    }
}