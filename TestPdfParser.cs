using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using QuimiOSCompanion.Services;

namespace QuimiOSCompanion
{
    public partial class TestPdfParser
    {
        public static void Main(string[] args)
        {
            var tempPath = Path.GetTempPath();
            var parser = new PdfParserService();

            var tests = new(string name, string desc, Action<string> create)[]
            {
                ("valid", "Valid PDF with all correct data", CreateValidPdf),
                ("missing_period", "Missing 'period:' keyword", CreateMissingPeriodPdf),
                ("missing_any", "Missing 'any' boundary keyword", CreateMissingAnyPdf),
                ("missing_total", "Missing 'Total' boundary keyword", CreateMissingTotalPdf),
                ("invalid_dates", "Invalid date format", CreateInvalidDatesPdf),
                ("missing_values", "Row with only 3 values", CreateMissingValuesPdf),
                ("extra_values", "Row with 5 values", CreateExtraValuesPdf),
                ("non_numeric", "Non-numeric values", CreateNonNumericPdf),
                ("empty_data", "Empty data section", CreateEmptyDataPdf),
                ("mixed_valid_invalid", "Mix of valid/invalid rows", CreateMixedPdf)
            };

            for (int i = 0; i < tests.Length; i++)
            {
                var (name, desc, create) = tests[i];
                var pdfPath = Path.Combine(tempPath, $"test_{name}.pdf");

                Console.WriteLine($"\n{new string('=', 60)}");
                Console.WriteLine($"TEST {i + 1}/{tests.Length}: {desc}");
                Console.WriteLine(new string('=', 60));

                try
                {
                    Console.Write("Creating PDF... ");
                    create(pdfPath);
                    Console.WriteLine($"OK: {pdfPath}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"FAIL: {ex.Message}");
                    continue;
                }

                try
                {
                    Console.Write("Parsing PDF... ");
                    var (startDate, endDate, rows) = parser.ParseConsumptionPdf(pdfPath);
                    Console.WriteLine("SUCCESS");
                    Console.WriteLine($"  Period: {startDate:MM/dd/yyyy} to {endDate:MM/dd/yyyy}");
                    Console.WriteLine($"  Parsed {rows.Count} rows");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("FAILED (Expected)");
                    Console.WriteLine($"  {ex.GetType().Name}: {ex.Message}");
                }
            }

            Console.WriteLine($"\n{new string('=', 60)}");
            Console.WriteLine("All tests completed!");
            Console.WriteLine($"{new string('=', 60)}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void CreateValidPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Equipment: ARCHITECT c8000"));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph("Consumption by reagent:"));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph("any"));

            document.Add(new Paragraph("GLUMTY 45 12 8 3"));
            document.Add(new Paragraph("TSHMTY 23 5 4 0"));
            document.Add(new Paragraph("COLHMTY 67 15 11 2"));
            document.Add(new Paragraph("CREAMTY 34 8 6 1"));
            document.Add(new Paragraph("AMILMTY 28 6 3 0"));
            document.Add(new Paragraph("FERRMTY 19 4 2 1"));

            document.Add(new Paragraph("Total consumptions: 195"));
            document.Add(new Paragraph(""));
            document.Add(new Paragraph("End of report"));

            document.Close();
        }
    }
}
