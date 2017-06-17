using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Pollenalarm.Backend.AspNet.Models;
using Pollenalarm.Backend.AspNet.Services;

namespace Pollenalarm.Backend.AspNet.Controllers
{
    [MobileAppController]
    public class PollutionController : ApiController
    {
        private readonly PollutionService _PollutionService;

        public PollutionController()
        {
            MobileServiceContext context = new MobileServiceContext();
            _PollutionService = new PollutionService(context);
        }

        //GET api/Pollution
        public IHttpActionResult Get(string zip)
        {
            var pollutions = _PollutionService.GetPollutionForPlace(zip);
            if (pollutions == null)
                return InternalServerError();

            return Ok(pollutions);
        }
    }
}
