using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Pollenalarm.Backend.AspNet.Models;
using Pollenalarm.Backend.AspNet.Services;

namespace Pollenalarm.Backend.AspNet.Controllers
{
    [MobileAppController]
    public class PollutionController : ApiController
    {
        private readonly PollutionService pollutionService;

        public PollutionController()
        {
            MobileServiceContext context = new MobileServiceContext();
            pollutionService = new PollutionService(context);
        }

        //GET api/Pollution
        public IHttpActionResult Get(string zip)
        {
            var pollutions = pollutionService.GetPollutionForPlace(zip);
            if (pollutions == null)
                return InternalServerError();

            return Ok(pollutions);
        }
    }
}
