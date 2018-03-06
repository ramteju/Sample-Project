using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class TaskDTO
    {
        public int Id { get; set; }
        public string TanName { get; set; }
        public int NUMsCount { get; set; }
        public int RXNsCount { get; set; }
        public string Shipment { get; set; }
        public string Analyst { get; set; }
        public string Status { get; set; }
        public int BatchNo { get; set; }
        public string Reviewer { get; set; }
        public string QC { get; set; }
        public int Version { get; set; }
        public string TanCompletionDate { get; set; }
        public string ProcessingNote { get; set; }
        public bool NearToTargetDate { get; set; }
    }

    public class PullTask
    {
        public int TanId { get; set; }
        public string TanNumber { get; set; }
        public List<RankHolder> TanRanks { get; set; }
        public List<RankHolder> UserRanks { get; set; }

        public PullTask()
        {
            TanRanks = new List<RankHolder>();
            UserRanks = new List<RankHolder>();
        }

    }

    public class RankHolder : IComparable
    {
        /// <summary>
        /// Username, TanNumber
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Rank
        /// </summary>
        public int Rank { get; set; }
        /// <summary>
        /// NUMs,RXN Density
        /// </summary>
        public object Score { get; set; }

        public int CompareTo(object other)
        {
            RankHolder otherRank = other as RankHolder;
            if (otherRank == null)
                return -1;
            return this.Rank.CompareTo(otherRank.Rank);
        }
    }
}
