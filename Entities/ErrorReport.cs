using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ErrorReport
    {
        public int Id { get; set; }
        public virtual ApplicationUser Role1 { get; set; }
        [ForeignKey("Role1")]
        [Index("IX_ROLE1_ROLE2_TAN", IsUnique = true, Order = 1)]
        public string Role1Id { get; set; }
        public string Role1Name { get; set; }
        public virtual ApplicationUser Role2 { get; set; }
        [ForeignKey("Role2")]
        [Index("IX_ROLE1_ROLE2_TAN", IsUnique = true, Order = 2)]
        public string Role2Id { get; set; }
        public string Role2Name { get; set; }
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        [Index("IX_ROLE1_ROLE2_TAN", IsUnique = true, Order = 3)]
        public int TanId { get; set; }
        public string TanNumber { get; set; }
        public int AddedReactions { get; set; }
        public int DeletedReactions { get; set; }
        public int CommonReactions { get; set; }
        public int AddedStages { get; set; }
        public int DeletedStages { get; set; }
        public int CommonStages { get; set; }
        public int AddedProducts { get; set; }
        public int DeletedProducts { get; set; }
        public int UpdatedProducts { get; set; }
        public int CommonProducts { get; set; }
        public int AddedReactants { get; set; }
        public int DeletedReactants { get; set; }
        public int UpdatedReactants { get; set; }
        public int CommonReactants { get; set; }
        public int AddedSolvents { get; set; }
        public int DeletedSolvents { get; set; }
        public int UpdatedSolvents { get; set; }
        public int CommonSolvents { get; set; }
        public int AddedAgents { get; set; }
        public int DeletedAgents { get; set; }
        public int UpdatedAgents { get; set; }
        public int CommonAgents { get; set; }
        public int AddedCatalysts { get; set; }
        public int DeletedCatalysts { get; set; }
        public int UpdatedCatalysts { get; set; }
        public int CommonCatalysts { get; set; }
        public int AddedRsns { get; set; }
        public int DeletedRsns { get; set; }
        public int UpdatedRsns { get; set; }
        public int AddedCVTS { get; set; }
        public int DeletedCVTS { get; set; }
        public int CommonCVTS { get; set; }
        public int AddedFreeText { get; set; }
        public int DeletedFreeText { get; set; }
        public int CommonFreeText { get; set; }
        public int CommonRsns { get; set; }
        public int AddedTime { get; set; }
        public int DeletedTime { get; set; }
        public int UpdatedTime { get; set; }
        public int CommonTime { get; set; }
        public int AddedTemperature { get; set; }
        public int DeletedTemperature { get; set; }
        public int UpdatedTemperature { get; set; }
        public int CommonTemperature { get; set; }
        public int AddedPressure { get; set; }
        public int DeletedPressure { get; set; }
        public int UpdatedPressure { get; set; }
        public int CommonPressure { get; set; }
        public int AddedpH { get; set; }
        public int DeletedpH { get; set; }
        public int UpdatedpH { get; set; }
        public int CommonpH { get; set; }
        public int AddedComments { get; set; }
        public int DeletedComments { get; set; }
        public int UpdatedComments { get; set; }
        public int CommonComments { get; set; }
        public DateTime RecordedDate { get; set; }
    }
}
