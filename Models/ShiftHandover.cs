using System;
using System.Collections.Generic;

namespace QuimiOSCompanion.Models
{
    public class ShiftHandover
    {
        public int Id { get; set; }
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime HandoverDate { get; set; }
        public string Notes { get; set; }
        public int PendingSamplesCount { get; set; }
        public List<PendingSample> PendingSamples { get; set; }

        public ShiftHandover()
        {
            PendingSamples = new List<PendingSample>();
        }
    }

    public class PendingSample
    {
        public int SampleId { get; set; }
        public int? Folio { get; set; }
        public string Reason { get; set; }
    }
}
