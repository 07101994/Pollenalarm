using System;
using System.Collections.Generic;
using System.Data.Entity;
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

            var today = DateTime.Now.Date;
            var tomorrow = DateTime.Now.Date.AddDays(1);
            var afterTomorrow = DateTime.Now.Date.AddDays(2);

            var pollutionQuery =
                from p in _Context.PollutionTable
                where p.Zip == zip && (DbFunctions.TruncateTime(p.Date) == today ||
                                       DbFunctions.TruncateTime(p.Date) == tomorrow ||
                                       DbFunctions.TruncateTime(p.Date) == afterTomorrow)
                select p;

            // Check if pollution is available and younger than 12 hours
            if (true) //!pollutionQuery.Any() || pollutionQuery.Min(p => p.Updated) < DateTime.Now.AddHours(-12))
            {
                // Existing update is not exitant or too old. Update this place
                pollutions = _UpdateService.GetUpdatedPollutions(zip);

                foreach (var pollution in pollutions)
                {
                    var existingPollution = pollutionQuery.Where(p => p.Date == pollution.Date).FirstOrDefault();
                    if (existingPollution != null)
                        _Context.Set<PollutionDto>().Remove(existingPollution);
                }

                // Insert updated pollutions to the database
                if (pollutions.Any())
                {
                    // Add updated pollutions
                    _Context.Set<PollutionDto>().AddRange(pollutions);
                    _Context.SaveChanges();
                }
            }
            else
            {
                pollutions = pollutionQuery.ToList();
            }

            return pollutions;
        }
    }
}