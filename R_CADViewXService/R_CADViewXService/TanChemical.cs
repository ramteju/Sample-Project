//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace R_CADViewXService
{
    using System;
    using System.Collections.Generic;
    
    public partial class TanChemical
    {
        public System.Guid Id { get; set; }
        public int TanId { get; set; }
        public int ChemicalType { get; set; }
        public int NUM { get; set; }
        public string RegNumber { get; set; }
        public string Formula { get; set; }
        public string ABSSterio { get; set; }
        public string PeptideSequence { get; set; }
        public string NuclicAcidSequence { get; set; }
        public string OtherName { get; set; }
        public string ImagePath { get; set; }
        public string Name { get; set; }
        public string CompoundNo { get; set; }
        public string GenericName { get; set; }
        public string MolString { get; set; }
    
        public virtual Tan Tan { get; set; }
    }
}
