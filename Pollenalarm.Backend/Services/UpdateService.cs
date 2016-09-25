using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Pollenalarm.Backend.Helper;
using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services.Base;

namespace Pollenalarm.Backend.Services
{
    public class UpdateService : ServiceBase
    {
        private readonly Table<PollenEntity> pollenTable;
        private Table<CityEntity> cityTable;
        private Table<PollutionUpdate> pollutionUpdateTable;
        private Table<InformationEntity> informationTable;

        public UpdateService()
        {
            pollenTable = DataContext.GetTable<PollenEntity>();
            cityTable = DataContext.GetTable<CityEntity>();
            pollutionUpdateTable = DataContext.GetTable<PollutionUpdate>();
            informationTable = DataContext.GetTable<InformationEntity>();
        }

        public ServiceResult<List<PollutionEntity>> GetUpdatedPollutions(string zip, List<PollenEntity> pollenList = null)
        {
            if (pollenList == null)
                pollenList = pollenTable.ToList();

            // Download pollen PDF
            var uri = new Uri("http://www.allergie.hexal.de/pollenflug/vorhersage/pdf_create.php?plz=" + zip);
            var pdfContent = PdfService.ExtractTextFromPdf(uri);

            // Extract pollution information
            var pollutionList = ExtractPollutionFromPdfContent(pdfContent, zip, pollenList);
            return new ServiceResult<List<PollutionEntity>>(pollutionList);
        }

        public ServiceResult<PollutionUpdate> UpdateFavorites()
        {
            var startTime = DateTime.Now;
            var pollutions = new List<PollutionEntity>();
            var favCities = cityTable.Where(c => c.IsFavorite == 1);

            foreach (var city in favCities)
            {
                var pollutionResult = GetUpdatedPollutions(city.Zip);
                if (pollutionResult.Success)
                    pollutions.AddRange(pollutionResult.Content);
            }

            var crawlingDuration = Convert.ToInt32((DateTime.Now - startTime).TotalMilliseconds);
            startTime = DateTime.Now;

            // Use the DataTableHelper for Bulk Inserts, because SubmitChanges() updates each row as a single transaction
            pollutions.BulkCopyToDatabase(DataContext);

            // Get update duration
            var transactionDuration = Convert.ToInt32((DateTime.Now - startTime).TotalMilliseconds);
            startTime = DateTime.Now;

            var update = new PollutionUpdate { Date = DateTime.Now, CrawlingDuration = crawlingDuration, TransactionDuration = transactionDuration, OverallDuration = crawlingDuration + transactionDuration };
            pollutionUpdateTable.InsertOnSubmit(update);
            DataContext.SubmitChanges();

            return new ServiceResult<PollutionUpdate>(update);
        }

		public ServiceResult UpdateInformation()
		{
			// Download pollen PDF
			var uri = new Uri ("http://www.allergie.hexal.de/pollenflug/vorhersage/pdf_create.php?plz=" + "40764");
			var pdfContent = PdfService.ExtractTextFromPdf(uri);

			// Prepare information
			var information = new InformationEntity();
			information.Date = DateTime.Now;

			try
			{
				// Extract Information
				information.Text =
					pdfContent.Substring(
						pdfContent.IndexOf("Für Deutschland:") + 17,
						(pdfContent.IndexOf("Für Postleitzahl") - (pdfContent.IndexOf("Für Deutschland:") + 17)));
			}
			catch (Exception)
			{
				// Recovery Text
				information.Text = "Derzeit sind keine weiteren Informationen vorhanden. Wir wünschen eine Pollenfreie Zeit!";
			}

            informationTable.InsertOnSubmit(information);
            DataContext.SubmitChanges();

			return new ServiceResult(true);
		}

        private List<PollutionEntity> ExtractPollutionFromPdfContent(string pdfContent, string zip, List<PollenEntity> pollenList)
        {
            // Split PDF string
            pdfContent = pdfContent.Substring(pdfContent.IndexOf("Ambrosia"), (pdfContent.IndexOf("Stand") - (pdfContent.IndexOf("Ambrosia") + 0)));
            var pollutionTable = pdfContent.Split('\n');

            var pollutionList = new List<PollutionEntity>();

            // Get the pollution information for each pollen
            foreach (var pollutionString in pollutionTable)
            {
                var trimmedPollutionString = pollutionString.Trim();
                if (trimmedPollutionString.Length <= 1)
                    continue;

                var data = trimmedPollutionString.Split(' ');
                var name = data[0];

                // Get the related entry from the pollen list
                var pollen = pollenList.FirstOrDefault(p => p.Name.Equals(name));
                if (pollen == null)
                    continue;

                // Create pollution
                var pollution = new PollutionEntity();
                pollution.City_Zip = zip;
                pollution.TimeStamp = DateTime.Now;
                pollution.Pollen_Id = pollen.Id;
                pollution.ValueToday = ConvertStringToPollutionValue(data[1]);
                pollution.ValueToday = ConvertStringToPollutionValue(data[2]);
                try
                {
                    pollution.ValueAfterTomorrow = ConvertStringToPollutionValue(data[3]);
                }
                catch (IndexOutOfRangeException)
                {

                    pollution.ValueAfterTomorrow = 0;
                }


                pollutionList.Add(pollution);
            }

            return pollutionList;
        }

        private static int ConvertStringToPollutionValue(string value)
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
