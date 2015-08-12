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
            var pollenTable = DataContext.GetTable<Pollen>();
            return Ok(pollenTable.ToList());
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
            var pollenTable = DataContext.GetTable<Pollen>();

            var pollen = pollenTable.FirstOrDefault(p => p.Id == id);
            if (pollen == null)
                return NotFound();

            return Ok(pollen);
        }
    }
}
