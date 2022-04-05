using System;

namespace QuimiOSCompanion.Models
{
    public class InventoryItem
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal MinimumStock { get; set; }
        public string Unit { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Location { get; set; }
    }
}
