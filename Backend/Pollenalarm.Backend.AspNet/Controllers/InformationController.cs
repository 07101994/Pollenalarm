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
    public class InformationController : TableController<InformationDto>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<InformationDto>(context, Request);
        }

        // GET tables/Information
        public IQueryable<InformationDto> GetAllInformationDto()
        {
            return Query();
        }

        // GET tables/Information/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<InformationDto> GetInformationDto(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Information/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<InformationDto> PatchInformationDto(string id, Delta<InformationDto> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Information
        public async Task<IHttpActionResult> PostInformationDto(InformationDto item)
        {
            InformationDto current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Information/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteInformationDto(string id)
        {
             return DeleteAsync(id);
        }
    }
}
