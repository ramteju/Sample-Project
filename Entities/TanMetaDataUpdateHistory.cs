using Newtonsoft.Json;
using ProductTracking.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class TanMetaDataUpdateHistory
    {
        public int Id { get; set; }
        [JsonIgnore]
        public virtual Tan Tan { get; set; }
        [ForeignKey("Tan")]
        public int TanID { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public TanMetaDataUpdateAction TanMetaDataUpdateAction { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("User")]
        public string UpdatedUserId { get; set; }
        public string ActionMessage { get; set; }
        public string UserComment { get; set; }
    }

    public enum TanMetaDataUpdateAction
    {
        [Description("Tan Manual Allocation")]
        TANMANUALALLOCATION = 1,
        [Description("Tan Manual Reallocation")]
        TANMANUALREALLOCATION = 2,
        [Description("QC Required Status Updation")]
        QCREQUIREDSTATUSUPDATION = 3,
        [Description("Target Date Update Action")]
        TARGETDATEUPDATEACTION = 4
    }
}
