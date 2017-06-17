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

            // Get exisitng pollutions for the next three days
            var today = DateTime.Now.Date;
            var tomorrow = DateTime.Now.Date.AddDays(1);
            var afterTomorrow = DateTime.Now.Date.AddDays(2);
            var pollutionQuery =
                from p in _Context.PollutionTable
                where p.Zip == zip && (
                    DbFunctions.TruncateTime(p.Date) == today ||
                    DbFunctions.TruncateTime(p.Date) == tomorrow ||
                    DbFunctions.TruncateTime(p.Date) == afterTomorrow)
                select p;

            // Check if pollution is available and younger than 12 hours
            if (!pollutionQuery.Any() || DateTime.Compare(pollutionQuery.Min(p => p.Updated), DateTime.Now.AddHours(-12)) > 0)
            {
                try
                {
                    // Existing update is not exitant or too old, so get updated ones
                    pollutions = _UpdateService.GetUpdatedPollutions(zip);
                }
                catch (IndexOutOfRangeException)
                {
                    // Parsing PDF failed, return existing pollutions from the database when available
                    pollutions = pollutionQuery.ToList();
                }

                if (pollutions.Any())
                {
                    // Delete existing pollutions from database
                    foreach (var pollution in pollutions)
                    {
                        var existingPollution = pollutionQuery.Where(p => p.PollenId == pollution.PollenId && p.Date == pollution.Date).FirstOrDefault();
                        if (existingPollution != null)
                            _Context.Set<PollutionDto>().Remove(existingPollution);
                    }

                    // Add updated pollutions to database
                    _Context.Set<PollutionDto>().AddRange(pollutions);
                    _Context.SaveChanges();
                }
            }
            else
            {
                // Existing pollutions are updated less than 12h ago, so re-use them
                pollutions = pollutionQuery.ToList();
            }

            return pollutions;
        }
    }
}