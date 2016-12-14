using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Pollenalarm.Backend.AspNet.Controllers
{
    [RoutePrefix("api/pollen")]
    public class PollenController : ApiController
    {
        [HttpGet]
        //[ResponseType(typeof(List<Pollen>))]
        public IHttpActionResult GetAllPollen()
        {
            return Ok("Hallo Pollen");
        }
    }
}
