using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.XML
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class Series9000
    {

        private Series9000Table1[] table1Field;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Table1")]
        public Series9000Table1[] Table1
        {
            get
            {
                return this.table1Field;
            }
            set
            {
                this.table1Field = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class Series9000Table1
    {

        private ushort nUMField;

        private string rEG_NOField;

        private string oRGREF_NAMEField;

        private ushort oRGREF_IDField;

        private string mOL_FILEField;

        private string iNCHI_KEYField;

        /// <remarks/>
        public ushort NUM
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
        public string REG_NO
        {
            get
            {
                return this.rEG_NOField;
            }
            set
            {
                this.rEG_NOField = value;
            }
        }

        /// <remarks/>
        public string ORGREF_NAME
        {
            get
            {
                return this.oRGREF_NAMEField;
            }
            set
            {
                this.oRGREF_NAMEField = value;
            }
        }

        /// <remarks/>
        public ushort ORGREF_ID
        {
            get
            {
                return this.oRGREF_IDField;
            }
            set
            {
                this.oRGREF_IDField = value;
            }
        }

        /// <remarks/>
        public string MOL_FILE
        {
            get
            {
                return this.mOL_FILEField;
            }
            set
            {
                this.mOL_FILEField = value;
            }
        }

        /// <remarks/>
        public string INCHI_KEY
        {
            get
            {
                return this.iNCHI_KEYField;
            }
            set
            {
                this.iNCHI_KEYField = value;
            }
        }
    }


}
