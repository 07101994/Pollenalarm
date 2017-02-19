using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using Pollenalarm.Backend.AspNet.DataObjects;
using Pollenalarm.Backend.AspNet.Models;

namespace Pollenalarm.Backend.AspNet.Controllers
{
    public class PlaceController : TableController<PlaceDto>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<PlaceDto>(context, Request);
        }

        // GET tables/Place
        public IQueryable<PlaceDto> GetAllPlaceDto()
        {
            return Query();
        }

        // GET tables/Place/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<PlaceDto> GetPlaceDto(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Place/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<PlaceDto> PatchPlaceDto(string id, Delta<PlaceDto> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Place
        public async Task<IHttpActionResult> PostPlaceDto(PlaceDto item)
        {
            PlaceDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Place/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePlaceDto(string id)
        {
             return DeleteAsync(id);
        }
    }
}
