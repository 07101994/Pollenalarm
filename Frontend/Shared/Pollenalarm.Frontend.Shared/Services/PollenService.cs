using Newtonsoft.Json;
using Pollenalarm.Core;
using Pollenalarm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
    public class PollenService
    {
        private string _BaseUrl;
        private IHttpService _HttpService;

        public PollenService(IHttpService httpService)
        {
            _HttpService = httpService;
            _BaseUrl = "https://pollenalarm.azurewebsites.net/api/pollution?zip";
        }

        public async Task<bool> GetPollutionsForPlaceAsync(Place place)
        {
            var result = await _HttpService.GetStringAsync($"{_BaseUrl}/pollutions&zip={place.Zip}");
            if (result != null)
            {
                var update = JsonConvert.DeserializeObject<Place>(result);
                place.PollutionToday = update.PollutionToday;
                place.PollutionTomorrow = update.PollutionTomorrow;
                place.PollutionAfterTomorrow = update.PollutionAfterTomorrow;

                return true;
            }

            return false;
        }
    }
}
