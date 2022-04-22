namespace QuimiOSCompanion.Models
{
    public class ConsumptionData
    {
        public int ReagentId { get; set; }
        public string ReagentCode { get; set; }
        public string ReagentName { get; set; }
        public decimal ResearchConsumption { get; set; }
        public decimal RepeatConsumption { get; set; }
        public decimal QCConsumption { get; set; }
        public decimal ManualConsumption { get; set; }
        public decimal CalibrationConsumption { get; set; }
        public decimal TotalConsumption { get; set; }
        public decimal FinalTotal { get; set; }
    }
}
