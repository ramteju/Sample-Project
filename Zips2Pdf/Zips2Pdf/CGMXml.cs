
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class CASDVW
{

    private CASDVWARTICLE[] aRTICLEField;

    private CASDVWSUBSTANC[] sUBSTANCField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ARTICLE")]
    public CASDVWARTICLE[] ARTICLE
    {
        get
        {
            return this.aRTICLEField;
        }
        set
        {
            this.aRTICLEField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SUBSTANC")]
    public CASDVWSUBSTANC[] SUBSTANC
    {
        get
        {
            return this.sUBSTANCField;
        }
        set
        {
            this.sUBSTANCField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWARTICLE
{

    private string anField;

    private string tANField;

    private CASDVWARTICLETI tiField;

    private CASDVWARTICLEJBIB jBIBField;

    private string dtField;

    private ushort cASPYField;

    private string cDPYField;

    private CASDVWARTICLEAB abField;

    private string kWHField;

    private string[] kwField;

    private string iEHField;

    private CASDVWARTICLEGSIE[] gSIEField;

    private CASDVWARTICLECSIE[] cSIEField;

    private string[] textField;

    private string tYPEField;

    /// <remarks/>
    public string AN
    {
        get
        {
            return this.anField;
        }
        set
        {
            this.anField = value;
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
    public CASDVWARTICLETI TI
    {
        get
        {
            return this.tiField;
        }
        set
        {
            this.tiField = value;
        }
    }

    /// <remarks/>
    public CASDVWARTICLEJBIB JBIB
    {
        get
        {
            return this.jBIBField;
        }
        set
        {
            this.jBIBField = value;
        }
    }

    /// <remarks/>
    public string DT
    {
        get
        {
            return this.dtField;
        }
        set
        {
            this.dtField = value;
        }
    }

    /// <remarks/>
    public ushort CASPY
    {
        get
        {
            return this.cASPYField;
        }
        set
        {
            this.cASPYField = value;
        }
    }

    /// <remarks/>
    public string CDPY
    {
        get
        {
            return this.cDPYField;
        }
        set
        {
            this.cDPYField = value;
        }
    }

    /// <remarks/>
    public CASDVWARTICLEAB AB
    {
        get
        {
            return this.abField;
        }
        set
        {
            this.abField = value;
        }
    }

    /// <remarks/>
    public string KWH
    {
        get
        {
            return this.kWHField;
        }
        set
        {
            this.kWHField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("KW")]
    public string[] KW
    {
        get
        {
            return this.kwField;
        }
        set
        {
            this.kwField = value;
        }
    }

    /// <remarks/>
    public string IEH
    {
        get
        {
            return this.iEHField;
        }
        set
        {
            this.iEHField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("GSIE")]
    public CASDVWARTICLEGSIE[] GSIE
    {
        get
        {
            return this.gSIEField;
        }
        set
        {
            this.gSIEField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("CSIE")]
    public CASDVWARTICLECSIE[] CSIE
    {
        get
        {
            return this.cSIEField;
        }
        set
        {
            this.cSIEField = value;
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
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWARTICLETI
{

    private byte[] sUBField;

    private byte sUPField;

    private bool sUPFieldSpecified;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SUB")]
    public byte[] SUB
    {
        get
        {
            return this.sUBField;
        }
        set
        {
            this.sUBField = value;
        }
    }

    /// <remarks/>
    public byte SUP
    {
        get
        {
            return this.sUPField;
        }
        set
        {
            this.sUPField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool SUPSpecified
    {
        get
        {
            return this.sUPFieldSpecified;
        }
        set
        {
            this.sUPFieldSpecified = value;
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
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWARTICLEJBIB
{

    private object[] itemsField;

    private ItemsChoiceType[] itemsElementNameField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("AU", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("CS", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("JT", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("LANG", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("PY", typeof(ushort))]
    [System.Xml.Serialization.XmlElementAttribute("TEXT", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType[] ItemsElementName
    {
        get
        {
            return this.itemsElementNameField;
        }
        set
        {
            this.itemsElementNameField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
public enum ItemsChoiceType
{

    /// <remarks/>
    AU,

    /// <remarks/>
    CS,

    /// <remarks/>
    JT,

    /// <remarks/>
    LANG,

    /// <remarks/>
    PY,

    /// <remarks/>
    TEXT,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWARTICLEAB
{

    private object[] itemsField;

    private ItemsChoiceType1[] itemsElementNameField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("B", typeof(byte))]
    [System.Xml.Serialization.XmlElementAttribute("IT", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUB", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUP", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType1[] ItemsElementName
    {
        get
        {
            return this.itemsElementNameField;
        }
        set
        {
            this.itemsElementNameField = value;
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
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
public enum ItemsChoiceType1
{

    /// <remarks/>
    B,

    /// <remarks/>
    IT,

    /// <remarks/>
    SUB,

    /// <remarks/>
    SUP,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWARTICLEGSIE
{

    private string gSHField;

    private string gSMField;

    private string[] textField;

    /// <remarks/>
    public string GSH
    {
        get
        {
            return this.gSHField;
        }
        set
        {
            this.gSHField = value;
        }
    }

    /// <remarks/>
    public string GSM
    {
        get
        {
            return this.gSMField;
        }
        set
        {
            this.gSMField = value;
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
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWARTICLECSIE
{

    private CASDVWARTICLECSIERN rnField;

    private byte nUMField;

    private string cSHField;

    private string cSMField;

    private string[] textField;

    /// <remarks/>
    public CASDVWARTICLECSIERN RN
    {
        get
        {
            return this.rnField;
        }
        set
        {
            this.rnField = value;
        }
    }

    /// <remarks/>
    public byte NUM
    {
        get
        {
            return this.nUMField;
        }
        set
        {
            this.nUMField = value;
        }
    }

    /// <remarks/>
    public string CSH
    {
        get
        {
            return this.cSHField;
        }
        set
        {
            this.cSHField = value;
        }
    }

    /// <remarks/>
    public string CSM
    {
        get
        {
            return this.cSMField;
        }
        set
        {
            this.cSMField = value;
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
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWARTICLECSIERN
{

    private uint rIDField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint RID
    {
        get
        {
            return this.rIDField;
        }
        set
        {
            this.rIDField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANC
{

    private CASDVWSUBSTANCRN rnField;

    private string iNHField;

    private CASDVWSUBSTANCIN inField;

    private string iNOHField;

    private CASDVWSUBSTANCINO[] iNOField;

    private string oNHField;

    private CASDVWSUBSTANCSYN[] sYNField;

    private string mFHField;

    private CASDVWSUBSTANCMF mfField;

    private string[] eLSGField;

    private string[] tISField;

    private string[] nTEField;

    private string[] nSEQField;

    private CASDVWSUBSTANCPSEQ pSEQField;

    private CASDVWSUBSTANCCOMP[] cOMPField;

    private string sMSGField;

    private string sTEMSGField;

    private string[] sIMField;

    /// <remarks/>
    public CASDVWSUBSTANCRN RN
    {
        get
        {
            return this.rnField;
        }
        set
        {
            this.rnField = value;
        }
    }

    /// <remarks/>
    public string INH
    {
        get
        {
            return this.iNHField;
        }
        set
        {
            this.iNHField = value;
        }
    }

    /// <remarks/>
    public CASDVWSUBSTANCIN IN
    {
        get
        {
            return this.inField;
        }
        set
        {
            this.inField = value;
        }
    }

    /// <remarks/>
    public string INOH
    {
        get
        {
            return this.iNOHField;
        }
        set
        {
            this.iNOHField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("INO")]
    public CASDVWSUBSTANCINO[] INO
    {
        get
        {
            return this.iNOField;
        }
        set
        {
            this.iNOField = value;
        }
    }

    /// <remarks/>
    public string ONH
    {
        get
        {
            return this.oNHField;
        }
        set
        {
            this.oNHField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SYN")]
    public CASDVWSUBSTANCSYN[] SYN
    {
        get
        {
            return this.sYNField;
        }
        set
        {
            this.sYNField = value;
        }
    }

    /// <remarks/>
    public string MFH
    {
        get
        {
            return this.mFHField;
        }
        set
        {
            this.mFHField = value;
        }
    }

    /// <remarks/>
    public CASDVWSUBSTANCMF MF
    {
        get
        {
            return this.mfField;
        }
        set
        {
            this.mfField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("ELS", IsNullable = false)]
    public string[] ELSG
    {
        get
        {
            return this.eLSGField;
        }
        set
        {
            this.eLSGField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("TISLIN", IsNullable = false)]
    public string[] TIS
    {
        get
        {
            return this.tISField;
        }
        set
        {
            this.tISField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("NTELIN", IsNullable = false)]
    public string[] NTE
    {
        get
        {
            return this.nTEField;
        }
        set
        {
            this.nTEField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("NLIN", IsNullable = false)]
    public string[] NSEQ
    {
        get
        {
            return this.nSEQField;
        }
        set
        {
            this.nSEQField = value;
        }
    }

    /// <remarks/>
    public CASDVWSUBSTANCPSEQ PSEQ
    {
        get
        {
            return this.pSEQField;
        }
        set
        {
            this.pSEQField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("COMP")]
    public CASDVWSUBSTANCCOMP[] COMP
    {
        get
        {
            return this.cOMPField;
        }
        set
        {
            this.cOMPField = value;
        }
    }

    /// <remarks/>
    public string SMSG
    {
        get
        {
            return this.sMSGField;
        }
        set
        {
            this.sMSGField = value;
        }
    }

    /// <remarks/>
    public string STEMSG
    {
        get
        {
            return this.sTEMSGField;
        }
        set
        {
            this.sTEMSGField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SIM")]
    public string[] SIM
    {
        get
        {
            return this.sIMField;
        }
        set
        {
            this.sIMField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCRN
{

    private uint idField;

    private string valueField;

    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public uint ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTextAttribute()]
    public string Value
    {
        get
        {
            return this.valueField;
        }
        set
        {
            this.valueField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCIN
{

    private object[] itemsField;

    private ItemsChoiceType2[] itemsElementNameField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("IT", typeof(CASDVWSUBSTANCINIT))]
    [System.Xml.Serialization.XmlElementAttribute("SCP", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUB", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUP", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType2[] ItemsElementName
    {
        get
        {
            return this.itemsElementNameField;
        }
        set
        {
            this.itemsElementNameField = value;
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
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCINIT
{

    private string sUBField;

    private string[] textField;

    /// <remarks/>
    public string SUB
    {
        get
        {
            return this.sUBField;
        }
        set
        {
            this.sUBField = value;
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
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
public enum ItemsChoiceType2
{

    /// <remarks/>
    IT,

    /// <remarks/>
    SCP,

    /// <remarks/>
    SUB,

    /// <remarks/>
    SUP,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCINO
{

    private object[] itemsField;

    private ItemsChoiceType3[] itemsElementNameField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("IT", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SCP", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUB", typeof(byte))]
    [System.Xml.Serialization.XmlElementAttribute("SUP", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUPSUP", typeof(byte))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType3[] ItemsElementName
    {
        get
        {
            return this.itemsElementNameField;
        }
        set
        {
            this.itemsElementNameField = value;
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
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
public enum ItemsChoiceType3
{

    /// <remarks/>
    IT,

    /// <remarks/>
    SCP,

    /// <remarks/>
    SUB,

    /// <remarks/>
    SUP,

    /// <remarks/>
    SUPSUP,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCSYN
{

    private object[] itemsField;

    private ItemsChoiceType4[] itemsElementNameField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("IT", typeof(CASDVWSUBSTANCSYNIT))]
    [System.Xml.Serialization.XmlElementAttribute("SCP", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUB", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUP", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType4[] ItemsElementName
    {
        get
        {
            return this.itemsElementNameField;
        }
        set
        {
            this.itemsElementNameField = value;
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
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCSYNIT
{

    private string[] sUPField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SUP")]
    public string[] SUP
    {
        get
        {
            return this.sUPField;
        }
        set
        {
            this.sUPField = value;
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
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(IncludeInSchema = false)]
public enum ItemsChoiceType4
{

    /// <remarks/>
    IT,

    /// <remarks/>
    SCP,

    /// <remarks/>
    SUB,

    /// <remarks/>
    SUP,
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCMF
{

    private object[] itemsField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SUB", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("SUP", typeof(byte))]
    public object[] Items
    {
        get
        {
            return this.itemsField;
        }
        set
        {
            this.itemsField = value;
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
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCPSEQ
{

    private string pLINField;

    /// <remarks/>
    public string PLIN
    {
        get
        {
            return this.pLINField;
        }
        set
        {
            this.pLINField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCCOMP
{

    private CASDVWSUBSTANCCOMPCMF cMFField;

    private string cSMSGField;

    private string cSTEMSGField;

    private string[] cSIMField;

    /// <remarks/>
    public CASDVWSUBSTANCCOMPCMF CMF
    {
        get
        {
            return this.cMFField;
        }
        set
        {
            this.cMFField = value;
        }
    }

    /// <remarks/>
    public string CSMSG
    {
        get
        {
            return this.cSMSGField;
        }
        set
        {
            this.cSMSGField = value;
        }
    }

    /// <remarks/>
    public string CSTEMSG
    {
        get
        {
            return this.cSTEMSGField;
        }
        set
        {
            this.cSTEMSGField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("CSIM")]
    public string[] CSIM
    {
        get
        {
            return this.cSIMField;
        }
        set
        {
            this.cSIMField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class CASDVWSUBSTANCCOMPCMF
{

    private byte[] sUBField;

    private string[] textField;

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("SUB")]
    public byte[] SUB
    {
        get
        {
            return this.sUBField;
        }
        set
        {
            this.sUBField = value;
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

