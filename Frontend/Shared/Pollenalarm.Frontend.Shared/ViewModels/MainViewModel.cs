using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Misc;
using Pollenalarm.Frontend.Shared.Services;
using Pollenalarm.Frontend.Shared.Models;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class MainViewModel : AsyncViewModelBase
    {
        private INavigationService _NavigationService;
        private IFileSystemService _FileSystemService;
        private ILocalizationService _LocalizationService;
        private IPollenService _PollenService;
        private SettingsService _SettingsService;
        private PlaceService _PlaceService;

        private ObservableCollection<PlaceRowViewModel> _Places;
        public ObservableCollection<PlaceRowViewModel> Places
        {
            get { return _Places; }
            set { _Places = value; RaisePropertyChanged(); }
        }

        private string _GreetingHeader;
        public string GreetingHeader
        {
            get { return _GreetingHeader; }
            set { _GreetingHeader = value; RaisePropertyChanged(); }
        }

        private RelayCommand _RefreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return _RefreshCommand ?? (_RefreshCommand = new RelayCommand(async () =>
                {
                    await RefreshAsync();
                }));
            }
        }

        private RelayCommand _NavigateToSettingsCommand;
        public RelayCommand NavigateToSettingsCommand
        {
            get
            {
                return _NavigateToSettingsCommand ?? (_NavigateToSettingsCommand = new RelayCommand(() =>
                {
                    _NavigationService.NavigateTo(ViewNames.Settings);
                }));
            }
        }

        private RelayCommand _NavigateToAddPlaceCommand;
        public RelayCommand NavigateToAddPlaceCommand
        {
            get
            {
                return _NavigateToAddPlaceCommand ?? (_NavigateToAddPlaceCommand = new RelayCommand(() =>
                {
                    _PlaceService.CurrentPlace = null;
                    _NavigationService.NavigateTo(ViewNames.AddEditPlace);
                }));
            }
        }

        private RelayCommand<string> _NavigateToPlaceCommand;
        public RelayCommand<string> NavigateToPlaceCommand
        {
            get
            {
                return _NavigateToPlaceCommand ?? (_NavigateToPlaceCommand = new RelayCommand<string>((string placeId) =>
                {
                    _PlaceService.CurrentPlace = _PlaceService.Places.FirstOrDefault(p => p.Id == placeId);
                    _NavigationService.NavigateTo(ViewNames.Place);
                }));
            }
        }

        private RelayCommand _NavigateToSearchCommand;
        public RelayCommand NavigateToSearchCommand
        {
            get
            {
                return _NavigateToSearchCommand ?? (_NavigateToSearchCommand = new RelayCommand(() =>
                {
                    _NavigationService.NavigateTo(ViewNames.Search);
                }));
            }
        }

        public MainViewModel(INavigationService navigationService, IFileSystemService fileSystemService, ILocalizationService localizationService, SettingsService settingsService, IPollenService pollenService, PlaceService placeService)
        {
            _NavigationService = navigationService;
            _FileSystemService = fileSystemService;
            _LocalizationService = localizationService;
            _PollenService = pollenService;
            _SettingsService = settingsService;
            _PlaceService = placeService;

            Places = new ObservableCollection<PlaceRowViewModel>();
        }

        public async Task RefreshAsync(bool force = false)
        {
            IsBusy = true;

            // -------------------------------------------------------------------
            // 1. Step: Things to do every time the user navigates to the MainPage
            // -------------------------------------------------------------------

            // Initialize Services if needed
            await _SettingsService.InitializeAsync();
            await _PlaceService.InitializeAsync();

            // Add places from PlaceService to list and update existing ones
            foreach (var place in _PlaceService.Places)
            {
                var placeViewModel = Places.FirstOrDefault(p => p.Id == place.Id);

                // Update place if it has already been existant
                placeViewModel?.UpdateProperties(place);

                // Add ViewModel if not existant
                if (placeViewModel == null)
                {
                    placeViewModel = new PlaceRowViewModel(_PollenService, place);
                    Places.Add(placeViewModel);
                }
            }

            // Remove deleted places from list
            foreach (var placeViewModel in Places)
            {
                var place = _PlaceService.Places.FirstOrDefault(p => p.Id == placeViewModel.Id);
                if (place == null)
                    Places.Remove(placeViewModel);
            }

            // Update greeting header
            UpdateGreetingHeader();

            // -------------------------------------------------------------------
            // 2. Step: Things to do only when refresh is needed
            // -------------------------------------------------------------------
            if (IsLoaded == false || force == true)
            {
                // Add or remove current position
                // Should be done after loading places from local storage as they contain the current position
                var currentPosition = _PlaceService.Places.FirstOrDefault(p => p.IsCurrentPosition);
                var currentPositionViewModel = currentPosition != null ? Places.FirstOrDefault(vm => vm.Id == currentPosition.Id) : null;

                if (_SettingsService.CurrentSettings.UseCurrentLocation)
                {
                    // Get current user location's zip code
                    var geolocation = await _PlaceService.GetCurrentGeoLocationAsync();
                    if (geolocation == null && currentPosition != null)
                    {
                        // Fetching zip code failed. Remove current position's view model
                        Places.Remove(currentPositionViewModel);
                    }
                    else
                    {
                        // Fetching zip code succeded
                        if (currentPosition != null)
                        {
                            // Current position has aleady been in the list. Just update
                            currentPosition.Zip = geolocation.Zip;
                            currentPositionViewModel.UpdateProperties(currentPosition);
                        }
                        else
                        {
                            currentPosition = new Place(_LocalizationService.GetString("CurrentPosition"), geolocation.Zip, true);
                            currentPositionViewModel = new PlaceRowViewModel(_PollenService, currentPosition);
                        }
                    }
                }
                else if (currentPositionViewModel != null)
                {
                    // Usage of current position has been disabled but view model is still there, so remove it
                    Places.Remove(currentPositionViewModel);
                }


                // Update all places
                foreach (var placeViewModel in Places)
                {
                    await placeViewModel.RefreshAsync();
                }
            }

            IsBusy = false;
            IsLoaded = true;
        }


        private void UpdateGreetingHeader()
        {
            var now = DateTime.Now;
            if (now.Hour > 5 && now.Hour < 11)
                GreetingHeader = _LocalizationService.GetString("GoodMorning");
            else if (now.Hour >= 11 && now.Hour < 14)
                GreetingHeader = _LocalizationService.GetString("GoodDay");
            else if (now.Hour >= 14 && now.Hour < 18)
                GreetingHeader = _LocalizationService.GetString("GoodEvening");
            else
                GreetingHeader = _LocalizationService.GetString("GoodNight");
        }
    }
}
