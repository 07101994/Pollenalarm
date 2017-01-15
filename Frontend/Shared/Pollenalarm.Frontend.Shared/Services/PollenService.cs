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
        private List<Pollen> _PollenList;

		public PollenService(IHttpService httpService, SettingsService settingsService)
        {
            _HttpService = httpService;
			_SettingsService = settingsService;
            _BaseUrl = "https://pollenalarm.azurewebsites.net/api";
        }

        public async Task<bool> GetPollutionsForPlaceAsync(Place place)
        {
            // Download pollutions
            var url = $"{_BaseUrl}/pollution?zip={place.Zip}";
            var result = await _HttpService.GetStringAsync(url);
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
                UpdatePollenSelection(place);

                return true;
            }

            return false;
        }

		public async Task<List<Pollen>> GetAllPollenAsync()
		{
            if (_PollenList == null || !_PollenList.Any())
            {
                // Download pollen
                var result = await _HttpService.GetStringAsync($"{_BaseUrl}/pollen");
                if (result != null)
                {
                    // Parse pollen
                    _PollenList = JsonConvert.DeserializeObject<List<Pollen>>(result);
                }
            }

            // Update pollen selection
            foreach (var pollen in _PollenList)
                pollen.IsSelected =
                    _SettingsService.CurrentSettings.SelectedPollen.ContainsKey(pollen.Id) ?
                    _SettingsService.CurrentSettings.SelectedPollen[pollen.Id] : true;

            return _PollenList;
		}

        public void UpdatePollenSelection(Place place)
        {
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
        }

		public async Task<IEnumerable<ISearchResult>> SearchAsnyc(string searchTerm)
		{			
			var result = await _HttpService.GetStringAsync($"{_BaseUrl}/search?term={searchTerm.Trim()}");
			if (result != null)
			{
				var resultList = JsonConvert.DeserializeObject<List<ISearchResult>>(result);
				return resultList;
			}

			return null;
		}
	}
}
