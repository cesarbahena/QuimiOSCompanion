namespace QuimiOSCompanion.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Unit { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal? MinStock { get; set; }
        public decimal? MaxStock { get; set; }
        public bool IsActive { get; set; }
        public bool IsLowStock { get; set; }
    }
}
