using System.Web.Http;
using Microsoft.Azure.Mobile.Server.Config;
using Pollenalarm.Backend.AspNet.Models;
using Pollenalarm.Backend.AspNet.Services;

namespace Pollenalarm.Backend.AspNet.Controllers
{
    [MobileAppController]
    [RoutePrefix("api/update")]
    public class UpdateController : ApiController
    {
        private InformationService _InformationService;

        public UpdateController()
        {
            MobileServiceContext context = new MobileServiceContext();
            _InformationService = new InformationService(context);
        }

        // GET api/Update
        /// <summary>
        /// Updates the information table by downloading the latest message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("information")]
        public IHttpActionResult UpdateInformation()
        {
            var updateResult = _InformationService.UpdateInformation();
            if (updateResult.Update == null)
                return InternalServerError();

            return Ok(updateResult);
        }
    }
}
