using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Pollenalarm.Backend.Helper;
using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services.Base;
using Pollenalarm.Shared.ViewModels;

namespace Pollenalarm.Backend.Services
{
    public class PollutionService : ServiceBase
    {
        private Table<Pollen> pollenTable;
        private Table<Pollution> pollutionTable;
        private Table<City> cityTable;
        private UpdateService updateService;

        public PollutionService()
        {
            pollutionTable = DataContext.GetTable<Pollution>();
            pollenTable = DataContext.GetTable<Pollen>();
            cityTable = DataContext.GetTable<City>();
            updateService = new UpdateService();
        }

        /// <summary>
        /// Checks if the last update for this city is older than 12 hours and updates it if needed.
        /// Provides the pollution values afterwards
        /// </summary>
        /// <param name="zip">Zip code of the city</param>
        /// <returns>List of pollution values</returns>
        public ServiceResult<List<PollutionViewModel>> GetPollutionsForCity(string zip)
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
                // Upadate this city
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

            var result = pollutions.Select(p => p.pollution.ToPollutionViewModel(p.city, p.pollen));

            return new ServiceResult<List<PollutionViewModel>>(result.ToList());
        }
    }
}