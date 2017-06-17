using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Description;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Web.Http;
using Pollenalarm.Backend.Controllers.Base;
using Pollenalarm.Backend.Models;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Backend.Controllers
{
    [RoutePrefix("api/pollen")]
    public class PollenController : PollenalarmApiControllerBase
    {
        // GET: api/Pollen
        /// <summary>
        /// Gets all available pollen information
        /// </summary>
        /// <returns>List of pollen</returns>
        [HttpGet]
        [ResponseType(typeof(List<Pollen>))]
        public IHttpActionResult GetAllPollen()
        {
            var pollenList = new List<Pollen>();
            var pollenTable = DataContext.GetTable<PollenEntity>();
            foreach (var pollenEntity in pollenTable)
            {
                pollenList.Add(pollenEntity.ToPollen());
            }

            return Ok(pollenList);
        }

        // GET: api/Pollen/5
        /// <summary>
        /// Gets a single pollen item by id
        /// </summary>
        /// <param name="id">ID of the pollen</param>
        /// <returns>Single pollen information</returns>
        [HttpGet]
        [ResponseType(typeof(Pollen))]
        public IHttpActionResult Get(int id)
        {
            var pollenTable = DataContext.GetTable<PollenEntity>();
            var pollenEntity = pollenTable.FirstOrDefault(p => p.Id == id);
            if (pollenEntity == null)
                return NotFound();

            return Ok(pollenEntity.ToPollen());
        }
    }
}
