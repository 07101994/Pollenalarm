using System;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace Pollenalarm.Backend.Services
{
    public static class PdfService
    {
		public static string ExtractTextFromPdf(Uri path)
        {
            using (var reader = new PdfReader(path))
            {
                var text = new StringBuilder();

                for (var i = 1; i <= reader.NumberOfPages; i++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                }

                return text.ToString();
            }
        }
    }
}
