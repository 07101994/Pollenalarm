using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Pollenalarm.Backend.AspNet.DataObjects;
using Pollenalarm.Backend.AspNet.Models;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Backend.AspNet.Services
{
    public class PollutionService
    {
        private MobileServiceContext _Context;
        private UpdateService _UpdateService;

        public PollutionService(MobileServiceContext context)
        {
            _Context = context;
            _UpdateService = new UpdateService(context);
        }

        public List<PollutionDto> GetPollutionForPlace(string zip)
        {
            List<PollutionDto> pollutions;

            // Get the latest update for this city
            var latestUpdate = (
                from p in _Context.PollutionTable
                where p.Zip == zip
                orderby p.Date descending
                select p.Date).FirstOrDefault();

            // Check if pollution is available and younger than 12 hours
            if (latestUpdate == null || latestUpdate < DateTime.Now.AddHours(-12))
            {
                // Existing update is not exitant or too old. Update this place
                pollutions = _UpdateService.GetUpdatedPollutions(zip);

                // Insert updated pollutions to the database
                if (pollutions.Any())
                {
                    _Context.Set<PollutionDto>().AddRange(pollutions);
                    _Context.SaveChanges();
                }
            }
            else
            {
                // Last update is younger than 12 hours, so get the latest pollutions from the database
                var today = DateTime.Now.Date;
                var tomorrow = DateTime.Now.Date.AddDays(1);
                var afterTomorrow = DateTime.Now.Date.AddDays(2);

                var pollutionQuery =
                    from p in _Context.PollutionTable
                    where p.Zip == zip && (p.Date.Date == today || p.Date.Date == tomorrow || p.Date.Date == afterTomorrow)
                    select p;

                pollutions = pollutionQuery.ToList();
            }

            return pollutions;
        }
    }
}