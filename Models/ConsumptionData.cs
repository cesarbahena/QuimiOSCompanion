namespace QuimiOSCompanion.Models
{
    public class ConsumptionData
    {
        public int ReagentId { get; set; }
        public string ReagentCode { get; set; } = string.Empty;
        public string ReagentName { get; set; } = string.Empty;

        public decimal ResearchConsumption { get; set; }
        public decimal RepeatConsumption { get; set; }
        public decimal QCConsumption { get; set; }
        public decimal ManualConsumption { get; set; }
        public decimal CalibrationConsumption { get; set; }
        public decimal TotalConsumption { get; set; }
        public decimal FinalTotal { get; set; }

        public int Px { get; set; }
        public int Rep { get; set; }
        public int QC { get; set; }
        public int Cal { get; set; }
        public int Canc { get; set; }
        public string Motivo { get; set; } = "[Seleccione]";
    }
}
