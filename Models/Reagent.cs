namespace QuimiOSCompanion.Models
{
    public class Reagent
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int CalibrationConsumption { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public bool IsActive { get; set; }
    }
}
