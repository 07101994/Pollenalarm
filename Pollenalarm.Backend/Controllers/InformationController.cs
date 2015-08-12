using System.Linq;
using System.Web.Http;
using Pollenalarm.Backend.Controllers.Base;

namespace Pollenalarm.Backend.Controllers
{
	[RoutePrefix("api/information")]
	public class InformationController : PollenalarmApiControllerBase
    {
		// GET: api/information
		/// <summary>
		/// Gets the latest general information object.
		/// </summary>
		[HttpGet]
		public IHttpActionResult GetGeneralInformation()
		{
			var informationTable = DataContext.GetTable<Information> ();

			var latestInformation = informationTable.Max (i => i.Date);
			return Ok (latestInformation);
		}
    }
}
