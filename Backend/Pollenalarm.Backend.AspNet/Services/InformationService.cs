using System;
using System.Linq;
using Pollenalarm.Backend.AspNet.DataObjects;
using Pollenalarm.Backend.AspNet.Models;

namespace Pollenalarm.Backend.AspNet.Services
{
    public class InformationService
    {
        private MobileServiceContext _Context;
        private UpdateService _UpdateService;

        public InformationService(MobileServiceContext context)
        {
            _Context = context;
            _UpdateService = new UpdateService(context);
        }

        public UpdateResult<InformationDto> UpdateInformation()
        {
            UpdateResult<InformationDto> result = new UpdateResult<InformationDto>();

            // Get the latest update for this city
            var latestInformation = (
                from i in _Context.InformationTable
                where i.Language == "DE"
                orderby i.Date descending
                select i).FirstOrDefault();

            // Check if information is available and from today
            if (latestInformation == null || latestInformation.Date.Date != DateTime.Now.Date)
            {
                // Information is not exitant or too old
                latestInformation = _UpdateService.GetUpdatedInformation();
                _Context.Set<InformationDto>().Add(latestInformation);
                _Context.SaveChanges();

                result.UpdateNeeded = true;
            }

            result.Update = latestInformation;

            return result;
        }
    }
}