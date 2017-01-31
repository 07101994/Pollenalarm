using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Frontend.Shared.Misc;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class SettingsViewModel : AsyncViewModelBase
	{
		private INavigationService _NavigationService;
        private SettingsService _SettingsService;

        private Settings _Settings;
        public Settings Settings
        {
            get { return _Settings; }
            set { _Settings = value; RaisePropertyChanged(); }
        }

        private RelayCommand _NavigateToPollenSelectionCommand;
        public RelayCommand NavigateToPollenSelectionCommand
        {
            get
            {
                return _NavigateToPollenSelectionCommand ?? (_NavigateToPollenSelectionCommand = new RelayCommand(() =>
                {
                    _NavigationService.NavigateTo(ViewNames.PollenSelection);
                }));
            }
        }

        private RelayCommand _NavigateToAboutCommand;
		public RelayCommand NavigateToAboutCommand
		{
			get
			{
				return _NavigateToAboutCommand ?? (_NavigateToAboutCommand = new RelayCommand(() =>
				{
					_NavigationService.NavigateTo(ViewNames.About);
				}));
			}
		}

        public SettingsViewModel(INavigationService navigationService, SettingsService settingsService)
		{
			_NavigationService = navigationService;
            _SettingsService = settingsService;
		}

        public async Task InitializeAsync()
        {
			IsBusy = true;

            await _SettingsService.LoadSettingsAsync();
            Settings = _SettingsService.CurrentSettings.Clone();

			IsBusy = false;
        }

        public async Task SaveChangesAsnyc()
        {
			IsBusy = true;

            // Trigger MainViewModel refresh on specific changes
            if (_SettingsService.CurrentSettings.UseCurrentLocation != Settings.UseCurrentLocation)
            {
                var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                mainViewModel.IsLoaded = false;
            }

            _SettingsService.CurrentSettings = Settings;
            await _SettingsService.SaveSettingsAsync();

			IsBusy = false;
        }
	}
}
