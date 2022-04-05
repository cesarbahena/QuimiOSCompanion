using System;

namespace QuimiOSCompanion.Models
{
    public class Sample
    {
        public string Id { get; set; }
        public string ClientName { get; set; }
        public string SampleType { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Status { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Priority { get; set; }
    }
}
