using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Pollenalarm.Backend.Helper;
using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services.Base;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Backend.Services
{
    public class PollutionService : ServiceBase
    {
        private Table<PollenEntity> pollenTable;
        private Table<PollutionEntity> pollutionTable;
        private Table<CityEntity> cityTable;
        private UpdateService updateService;

        public PollutionService()
        {
            pollutionTable = DataContext.GetTable<PollutionEntity>();
            pollenTable = DataContext.GetTable<PollenEntity>();
            cityTable = DataContext.GetTable<CityEntity>();
            updateService = new UpdateService();
        }

        /// <summary>
        /// Checks if the last update for this city is older than 12 hours and updates it if needed.
        /// Provides the pollution values afterwards
        /// </summary>
        /// <param name="zip">Zip code of the city</param>
        /// <returns>List of pollution values</returns>
        public ServiceResult<Place> GetPollutionsForPlace(string zip)
        {
            // Get the latest update for this city
            var latestUpdate =
                (from p in pollutionTable
                 where p.City_Zip == zip
                 orderby p.TimeStamp descending
                 select p.TimeStamp).FirstOrDefault();

            // Check if pollution is available and up to date
            var lastAllowedDate = DateTime.Now.AddHours(-12);
            if (latestUpdate == null || latestUpdate < lastAllowedDate)
            {
                // Upadate this place
                var updateResult = updateService.GetUpdatedPollutions(zip, pollenTable.ToList());
                if (updateResult.Success)
                    // Use the DataTableHelper for Bulk Inserts, because SubmitChanges() updates each row as a single transaction
                    updateResult.Content.BulkCopyToDatabase(DataContext);
            }

            // Get latest pollution for this city
            var pollutions =
                from pollution in pollutionTable
                join pollen in pollenTable on pollution.Pollen_Id equals pollen.Id
                join city in cityTable on pollution.City_Zip equals city.Zip into joined
                where pollution.City_Zip == zip
                from c in joined.DefaultIfEmpty()
                group new
                {
                    pollution,
                    pollen,
                    city = c
                }
                by pollution.Pollen_Id into grp
                select grp.OrderByDescending(g => g.pollution.TimeStamp).First();

            var place = new Place();
            place.Zip = zip;

            foreach (var p in pollutions)
            {
                place.PollutionToday.Add(new Pollution { Pollen = p.pollen.ToPollen(), Intensity = p.pollution.ValueToday, Date = p.pollution.TimeStamp });
                place.PollutionTomorrow.Add(new Pollution { Pollen = p.pollen.ToPollen(), Intensity = p.pollution.ValueTomorrow, Date = p.pollution.TimeStamp });
                place.PollutionAfterTomorrow.Add(new Pollution { Pollen = p.pollen.ToPollen(), Intensity = p.pollution.ValueAfterTomorrow, Date = p.pollution.TimeStamp });
            }

            return new ServiceResult<Place>(place);
        }
    }
}