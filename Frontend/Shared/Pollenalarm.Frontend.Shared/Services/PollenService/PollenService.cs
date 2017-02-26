using System.Collections.Generic;
using System.Threading.Tasks;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Frontend.Shared.Services
{
    public abstract class PollenService : IPollenService
    {
        protected SettingsService _SettingsService;

        public PollenService(SettingsService settingsService)
        {
            _SettingsService = settingsService;
        }

        //public abstract Task InitializeAsync();
        public abstract Task<List<Pollen>> GetAllPollenAsync();
        public abstract Task<bool> GetPollutionsForPlaceAsync(Place place);

        /// <summary>
        /// Updates a place's list of pollen's 'IsSelected' flag using locally stored user settings
        /// </summary>
        /// <param name="place">Place with updated list of pollen</param>
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
    }
}
