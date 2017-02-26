using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Frontend.Shared.Services
{
    public interface IPollenService
    {
        //Task InitializeAsync();
        Task<List<Pollen>> GetAllPollenAsync();
        Task<bool> GetPollutionsForPlaceAsync(Place place);
        void UpdatePollenSelection(Place place);
    }
}
