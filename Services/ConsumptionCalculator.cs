using System.Collections.Generic;
using System.Linq;
using QuimiOSCompanion.Models;

namespace QuimiOSCompanion.Services
{
    public class ConsumptionCalculator
    {
        public List<ConsumptionData> CalculateConsumptions(
            List<PdfParserService.ParsedRow> parsedRows,
            List<Reagent> reagentCatalog,
            List<Reagent> selectedCalibrations)
        {
            var consumptions = new List<ConsumptionData>();

            foreach (var row in parsedRows)
            {
                var reagent = reagentCatalog.FirstOrDefault(r =>
                    r.Code.Equals(row.ReagentCode, System.StringComparison.OrdinalIgnoreCase));

                if (reagent == null)
                    continue;

                var calibrationAmount = selectedCalibrations
                    .FirstOrDefault(c => c.Id == reagent.Id)?.CalibrationConsumption ?? 0;

                var total = row.ResearchConsumption + row.RepeatConsumption +
                           row.QCConsumption + row.ManualConsumption;

                var finalTotal = total + calibrationAmount;

                consumptions.Add(new ConsumptionData
                {
                    ReagentId = reagent.Id,
                    ReagentCode = reagent.Code,
                    ReagentName = reagent.Name,
                    ResearchConsumption = row.ResearchConsumption,
                    RepeatConsumption = row.RepeatConsumption,
                    QCConsumption = row.QCConsumption,
                    ManualConsumption = row.ManualConsumption,
                    CalibrationConsumption = calibrationAmount,
                    TotalConsumption = total,
                    FinalTotal = finalTotal
                });
            }

            return consumptions;
        }

        public bool ValidateInventoryStock(List<ConsumptionData> consumptions, List<InventoryItem> inventory)
        {
            foreach (var consumption in consumptions)
            {
                var item = inventory.FirstOrDefault(i =>
                    i.Code.Equals(consumption.ReagentCode, System.StringComparison.OrdinalIgnoreCase));

                if (item == null || item.CurrentStock < consumption.FinalTotal)
                    return false;
            }

            return true;
        }

        public List<string> GetValidationErrors(List<ConsumptionData> consumptions, List<InventoryItem> inventory)
        {
            var errors = new List<string>();

            foreach (var consumption in consumptions)
            {
                var item = inventory.FirstOrDefault(i =>
                    i.Code.Equals(consumption.ReagentCode, System.StringComparison.OrdinalIgnoreCase));

                if (item == null)
                {
                    errors.Add($"No inventory found for {consumption.ReagentName}");
                }
                else if (item.CurrentStock < consumption.FinalTotal)
                {
                    errors.Add($"Insufficient stock for {consumption.ReagentName}. Available: {item.CurrentStock}, Required: {consumption.FinalTotal}");
                }
            }

            return errors;
        }
    }
}
