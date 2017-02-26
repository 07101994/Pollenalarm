using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class PollenViewModel : AsyncViewModelBase
    {
		private IPollenService _PollenService;
		private SettingsService _SettingsService;

		private ObservableCollection<Pollen> _Pollen;
		public ObservableCollection<Pollen> Pollen
		{
			get { return _Pollen; }
			set { _Pollen = value; RaisePropertyChanged(); }
		}

        private Pollen _CurrentPollen;
        public Pollen CurrentPollen
        {
            get { return _CurrentPollen; }
            set { _CurrentPollen = value; RaisePropertyChanged(); }
        }

		public PollenViewModel(IPollenService pollenService, SettingsService settingsService)
        {
			_PollenService = pollenService;
			_SettingsService = settingsService;
            _Pollen = new ObservableCollection<Pollen>();
        }

        public async Task RefreshAsync()
        {
            IsBusy = true;
            IsLoaded = false;

            var allPollen = await _PollenService.GetAllPollenAsync();
            if (allPollen != null)
            {
                _Pollen.Clear();

                foreach (var pollen in allPollen)
                    _Pollen.Add(pollen);

                IsLoaded = true;
            }

            IsBusy = false;
        }

        public async Task SaveChangesAsync(Pollen changedPollen = null)
        {
            await _SettingsService.LoadSettingsAsync();

            if (changedPollen != null)
            {
                // If only a single pollen has been changed
                _SettingsService.CurrentSettings.SelectedPollen[changedPollen.Id] = changedPollen.IsSelected;
            }
            else
            {
                // Update all Pollen in pollen list
                foreach (var pollen in _Pollen)
                    _SettingsService.CurrentSettings.SelectedPollen[pollen.Id] = pollen.IsSelected;
            }

            // If CurrentPollen is available (PollenPage), override its entry
            if (CurrentPollen != null)
                _SettingsService.CurrentSettings.SelectedPollen[CurrentPollen.Id] = CurrentPollen.IsSelected;

            // Invalidate IsLoaded flag, as selections have changed
            IsLoaded = false;

            await _SettingsService.SaveSettingsAsync();
        }
    }
}
