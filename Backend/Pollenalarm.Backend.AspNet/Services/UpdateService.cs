using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Pollenalarm.Backend.AspNet.DataObjects;
using Pollenalarm.Backend.AspNet.Models;

namespace Pollenalarm.Backend.AspNet.Services
{
    public class UpdateService
    {
        private MobileServiceContext context;
        private PdfService pdfService;

        public UpdateService(MobileServiceContext context)
        {
            this.context = context;
            this.pdfService = new PdfService();
        }

        public List<PollutionDto> GetUpdatedPollutions(string zip)
        {

            // Download pollen PDF
            //var uri = new Uri("http://www.allergie.hexal.de/pollenflug/vorhersage/pdf_create.php?plz=" + zip);
            var uri = new Uri(ConfigurationManager.AppSettings["PollenInformationSourceUrl"].ToString() + zip);
            var pdfContent = pdfService.ExtractTextFromPdf(uri);

            // Extract pollution information
            var pollutionList = ExtractPollutionFromPdfContent(pdfContent, zip);
            return new List<PollutionDto>(pollutionList);
        }

        private List<PollutionDto> ExtractPollutionFromPdfContent(string pdfContent, string zip)
        {
            var pollutionList = new List<PollutionDto>();
            var pollenList = context.PollenTable.ToArray();

            // Shrink PDF string down to the table only, which starts with 'Ambrosia' and ends with 'Stand'
            pdfContent = pdfContent.Substring(pdfContent.IndexOf("Ambrosia"), (pdfContent.IndexOf("Stand") - (pdfContent.IndexOf("Ambrosia") + 0)));
            var pdfPollutionTable = pdfContent.Split('\n');

            // Get the pollution information for each pollen
            foreach (var pollutionString in pdfPollutionTable)
            {
                // Skip empty ines
                var trimmedPollutionString = pollutionString.Trim();
                if (trimmedPollutionString.Length <= 1)
                    continue;

                // Split table lines into rows
                var columns = trimmedPollutionString.Split(' ');

                // Get the related entry from the pollen list
                var pollen = pollenList.FirstOrDefault(p => p.Name.Equals(columns[0]));
                if (pollen == null)
                    continue;

                // Create three pollutions for today, tomorrow and after tomorrow
                for (var i = 0; i < 3; i++)
                {
                    pollutionList.Add(new PollutionDto
                    {
                        PollenId = pollen.Id,
                        Zip = zip,
                        Date = DateTime.Now.Date.AddDays(i),
                        Intensity = GetIntensity(columns[i + 1])
                    });
                }
            }

            return pollutionList;
        }

        private int GetIntensity(string value)
        {
            if (value.Contains("stark"))
                return 3;
            if (value.Contains("mäßig"))
                return 2;
            if (value.Contains("schwach"))
                return 1;

            return 0;
        }
    }
}