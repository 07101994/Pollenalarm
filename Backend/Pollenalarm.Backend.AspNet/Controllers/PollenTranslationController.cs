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
    public class PollenTranslationController : TableController<PollenTranslationDto>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<PollenTranslationDto>(context, Request, enableSoftDelete: true);
        }

        // GET tables/PollenTranslation
        public IQueryable<PollenTranslationDto> GetAllPollenTranslationDto()
        {
            return Query();
        }

        // GET tables/PollenTranslation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<PollenTranslationDto> GetPollenTranslationDto(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/PollenTranslation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<PollenTranslationDto> PatchPollenTranslationDto(string id, Delta<PollenTranslationDto> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/PollenTranslation
        public async Task<IHttpActionResult> PostPollenTranslationDto(PollenTranslationDto item)
        {
            PollenTranslationDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/PollenTranslation/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePollenTranslationDto(string id)
        {
             return DeleteAsync(id);
        }
    }
}
