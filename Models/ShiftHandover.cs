using System;
using System.Collections.Generic;

namespace QuimiOSCompanion.Models
{
    public class ShiftHandover
    {
        public string Id { get; set; }
        public string ShiftType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TechnicianName { get; set; }
        public List<Sample> PendingSamples { get; set; }
        public string Notes { get; set; }
        public int TotalSamplesReceived { get; set; }
        public int TotalSamplesCompleted { get; set; }

        public ShiftHandover()
        {
            PendingSamples = new List<Sample>();
        }
    }
}
