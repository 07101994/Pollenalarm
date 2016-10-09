using Newtonsoft.Json;
using Pollenalarm.Core;
using Pollenalarm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Services
{
    public class PollenService
    {
        private string _BaseUrl;
        private IHttpService _HttpService;
		private SettingsService _SettingsService;

		public PollenService(IHttpService httpService, SettingsService settingsService)
        {
            _HttpService = httpService;
			_SettingsService = settingsService;
            _BaseUrl = "https://pollenalarm.azurewebsites.net/api/pollution?zip";
        }

        public async Task<bool> GetPollutionsForPlaceAsync(Place place)
        {
			// Download pollutions
            var result = await _HttpService.GetStringAsync($"{_BaseUrl}/pollutions&zip={place.Zip}");
            if (result != null)
            {
				// Parse pollutions
                var update = JsonConvert.DeserializeObject<Place>(result);
                place.PollutionToday = update.PollutionToday;
                place.PollutionTomorrow = update.PollutionTomorrow;
                place.PollutionAfterTomorrow = update.PollutionAfterTomorrow;

				// Init settings
				await _SettingsService.LoadSettingsAsync();

				// Update pollen selection
				foreach (var pollution in place.PollutionToday)
					pollution.Pollen.IsSelected = 
						_SettingsService.CurrentSettings.SelectedPollen.ContainsKey(pollution.Pollen.Id) ? 
						_SettingsService.CurrentSettings.SelectedPollen[pollution.Pollen.Id] : true;

				foreach (var pollution in place.PollutionTomorrow)
					pollution.Pollen.IsSelected =
						_SettingsService.CurrentSettings.SelectedPollen.ContainsKey(pollution.Pollen.Id) ?
						_SettingsService.CurrentSettings.SelectedPollen[pollution.Pollen.Id] : true;

				foreach (var pollution in place.PollutionAfterTomorrow)
					pollution.Pollen.IsSelected =
						_SettingsService.CurrentSettings.SelectedPollen.ContainsKey(pollution.Pollen.Id) ?
						_SettingsService.CurrentSettings.SelectedPollen[pollution.Pollen.Id] : true;

                return true;
            }

            return false;
        }

		public async Task<List<Pollen>> GetAllPollenAsync()
		{
			// Download pollutions
			var result = await _HttpService.GetStringAsync($"{_BaseUrl}/pollen");
			if (result != null)
			{
				// Parse pollen
				var pollenList = JsonConvert.DeserializeObject<List<Pollen>>(result);

				// Update pollen selection
				foreach (var pollen in pollenList)
					pollen.IsSelected =
						_SettingsService.CurrentSettings.SelectedPollen.ContainsKey(pollen.Id) ?
						_SettingsService.CurrentSettings.SelectedPollen[pollen.Id] : true;

				return pollenList;
			}

			return null;
		}
	}
}
