using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace QuimiOSCompanion
{
    public partial class TestPdfParser
    {
        private static void CreateMissingPeriodPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Equipment: ARCHITECT c8000"));
            document.Add(new Paragraph("Report date range: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("GLUMTY 45 12 8 3"));
            document.Add(new Paragraph("Total consumptions: 45"));
            document.Close();
        }

        private static void CreateMissingAnyPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("GLUMTY 45 12 8 3"));
            document.Add(new Paragraph("Total consumptions: 45"));
            document.Close();
        }

        private static void CreateMissingTotalPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("GLUMTY 45 12 8 3"));
            document.Add(new Paragraph("TSHMTY 23 5 4 0"));
            document.Add(new Paragraph("End of report"));
            document.Close();
        }

        private static void CreateInvalidDatesPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: invalid-date to also-invalid"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("GLUMTY 45 12 8 3"));
            document.Add(new Paragraph("Total consumptions: 45"));
            document.Close();
        }

        private static void CreateMissingValuesPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("GLUMTY 45 12"));
            document.Add(new Paragraph("TSHMTY 23 5 4 0"));
            document.Add(new Paragraph("Total consumptions: 45"));
            document.Close();
        }

        private static void CreateExtraValuesPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("GLUMTY 45 12 8 3 99"));
            document.Add(new Paragraph("TSHMTY 23 5 4 0"));
            document.Add(new Paragraph("Total consumptions: 45"));
            document.Close();
        }

        private static void CreateNonNumericPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("GLUMTY abc def ghi jkl"));
            document.Add(new Paragraph("TSHMTY 23 5 4 0"));
            document.Add(new Paragraph("Total consumptions: 45"));
            document.Close();
        }

        private static void CreateEmptyDataPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("Total consumptions: 0"));
            document.Close();
        }

        private static void CreateMixedPdf(string filePath)
        {
            using var writer = new PdfWriter(filePath);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("Laboratory Consumption Report"));
            document.Add(new Paragraph("Report period: 04/18/2022 to 04/22/2022"));
            document.Add(new Paragraph("any"));
            document.Add(new Paragraph("GLUMTY 45 12 8 3"));
            document.Add(new Paragraph("INVALID abc def"));
            document.Add(new Paragraph("TSHMTY 23 5 4 0"));
            document.Add(new Paragraph("BADCODE 1 2 3 4"));
            document.Add(new Paragraph("COLHMTY 67 15 11 2"));
            document.Add(new Paragraph("Total consumptions: 195"));
            document.Close();
        }
    }
}
