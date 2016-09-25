using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Pollenalarm.Backend.Controllers.Base;
using Pollenalarm.Backend.Helper;
using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services;

namespace Pollenalarm.Backend.Controllers
{
    [RoutePrefix("api/update")]
    public class UpdateController : PollenalarmApiControllerBase
    {
        private UpdateService updateService;

        public UpdateController()
        {
            updateService = new UpdateService();
        }

        public IHttpActionResult UpdateFavoriteCities()
        {
            var result = updateService.UpdateFavorites();
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Content);
        }

        public IHttpActionResult UpdateInformation()
        {
            var result = updateService.UpdateInformation();
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok();
        }
    }
}
