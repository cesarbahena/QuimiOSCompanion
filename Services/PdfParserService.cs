using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using QuimiOSCompanion.Models;

namespace QuimiOSCompanion.Services
{
    public class PdfParserService
    {
        public (DateTime startDate, DateTime endDate, List<ParsedRow> rows) ParseConsumptionPdf(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("PDF file not found", filePath);

            using var pdfReader = new PdfReader(filePath);
            using var pdfDocument = new PdfDocument(pdfReader);

            var extractedText = string.Empty;
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                var page = pdfDocument.GetPage(i);
                var strategy = new SimpleTextExtractionStrategy();
                extractedText += PdfTextExtractor.GetTextFromPage(page, strategy);
            }

            return ParseExtractedText(extractedText);
        }

        private (DateTime startDate, DateTime endDate, List<ParsedRow> rows) ParseExtractedText(string text)
        {
            var tokens = text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var periodIndex = Array.FindIndex(tokens, t => t.Equals("period:", StringComparison.OrdinalIgnoreCase));
            if (periodIndex == -1)
                throw new Exception("Could not find 'period:' keyword in PDF");

            var startDate = DateTime.Parse(tokens[periodIndex + 1]);
            var endDate = DateTime.Parse(tokens[periodIndex + 3]);

            var anyIndex = Array.FindIndex(tokens, t => t.Equals("any", StringComparison.OrdinalIgnoreCase));
            var totalIndex = Array.FindIndex(tokens, t => t.Equals("Total", StringComparison.OrdinalIgnoreCase));

            if (anyIndex == -1 || totalIndex == -1)
                throw new Exception("Could not find data boundaries (any/Total) in PDF");

            var dataStartIndex = anyIndex + 1;
            var dataEndIndex = totalIndex;
            var dataTokens = tokens.Skip(dataStartIndex).Take(dataEndIndex - dataStartIndex).ToArray();

            var rows = new List<ParsedRow>();
            for (int i = 0; i < dataTokens.Length; i += 5)
            {
                if (i + 4 >= dataTokens.Length)
                    break;

                var code = dataTokens[i];

                if (!decimal.TryParse(dataTokens[i + 1], out var res) ||
                    !decimal.TryParse(dataTokens[i + 2], out var rep) ||
                    !decimal.TryParse(dataTokens[i + 3], out var qc) ||
                    !decimal.TryParse(dataTokens[i + 4], out var man))
                {
                    continue;
                }

                rows.Add(new ParsedRow
                {
                    ReagentCode = code,
                    ResearchConsumption = res,
                    RepeatConsumption = rep,
                    QCConsumption = qc,
                    ManualConsumption = man
                });
            }

            return (startDate, endDate, rows);
        }

        public class ParsedRow
        {
            public string ReagentCode { get; set; }
            public decimal ResearchConsumption { get; set; }
            public decimal RepeatConsumption { get; set; }
            public decimal QCConsumption { get; set; }
            public decimal ManualConsumption { get; set; }
        }
    }
}
