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
    public class PollenController : TableController<PollenDto>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<PollenDto>(context, Request, enableSoftDelete: true);
        }

        // GET tables/Pollen
        public IQueryable<PollenDto> GetAllPollenDto()
        {
            return Query();
        }

        // GET tables/Pollen/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<PollenDto> GetPollenDto(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Pollen/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<PollenDto> PatchPollenDto(string id, Delta<PollenDto> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Pollen
        public async Task<IHttpActionResult> PostPollenDto(PollenDto item)
        {
            PollenDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Pollen/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePollenDto(string id)
        {
             return DeleteAsync(id);
        }
    }
}
