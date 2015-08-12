using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using Pollenalarm.Backend.Controllers.Base;
using Pollenalarm.Backend.Models;
using Pollenalarm.Backend.Services;
using Pollenalarm.Shared.ViewModels;

namespace Pollenalarm.Backend.Controllers
{
    [RoutePrefix("api/customer")]
    public class CityController : PollenalarmApiControllerBase
    {
        private CityService cityService;

        public CityController()
        {
            cityService = new CityService();
        }

        // GET: api/City?zip=52080
        /// <summary>
        /// Gets a single city
        /// </summary>
        /// <param name="zip">Zip code of the city</param>
        /// <returns>Single city</returns>
        [HttpGet]
        [ResponseType(typeof(List<City>))]
        public IHttpActionResult GetCityByZip(string zip)
        {
            var result = cityService.GetCity(zip);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Content);
        }

        // GET: api/City
        /// <summary>
        /// Gets all map cities
        /// </summary>
        /// <returns>List of cities that are marked as favorite</returns>
        [HttpGet]
        [ResponseType(typeof(List<City>))]
        public IHttpActionResult GetMapCities()
        {
            var result = cityService.GetMapCities();
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Content);
        }

        // POST: api/City
        /// <summary>
        /// Adds a new city to the database
        /// </summary>
        /// <param name="newCity">New city to add</param>
        [HttpPost]
        [ResponseType(typeof(CityViewModel))]
        public IHttpActionResult Post([FromBody]CityViewModel newCity)
        {
            var result = cityService.AddCity(newCity);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Content);
        }
    }
}
