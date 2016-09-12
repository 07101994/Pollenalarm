using Pollenalarm.Frontend.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
    public interface IGeoLoactionService
    {
        Task<GeoLocation> GetCurrentLocationAsync();
    }
}
