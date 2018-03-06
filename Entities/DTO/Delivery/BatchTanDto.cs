using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DTO
{
    public class BatchTanDto
    {
        public int Id { get; set; }
        public int BatchNumber { get; set; }
        public string TanNumber { get; set; }
        public TanCategory TanCategory { get; set; }
        public string TanType { get; set; }
        public int Nums { get; set; }
        public int Rxns { get; set; }
        public int Stages { get; set; }
        public string Curator { get; set; }
        public Role CurrentRole { get; set; }
        public string Reviewer { get; set; }
        public string QC { get; set; }
        public int Version { get; set; }
        public TanState? TanState { get; set; }
        public bool NearToTargetDate { get; set; }
        public string TargetDate { get; set; }
        public bool IsDoubtRaised { get; set; }
        public string ProcessingNote { get; set; }
    }
}
