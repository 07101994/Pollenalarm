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
using MvvmHelpers;

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

        private ObservableRangeCollection<Place> _Places;
        public ObservableRangeCollection<Place> Places
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
                    await RefreshAsync(true);
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

            Places = new ObservableRangeCollection<Place>();
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

            Places.ReplaceRange(_PlaceService.Places);

            // Update greeting header
            UpdateGreetingHeader();

            // Get place for current GPS position out of list of places
            var currentPosition = Places.FirstOrDefault(p => p.IsCurrentPosition);
            if (currentPosition != null && _SettingsService.CurrentSettings.UseCurrentLocation == false)
            {
                // Place for current position found but GPS has been disabled in the settings
                // Remove place from service
                await _PlaceService.DeletePlaceAsync(currentPosition);

                // Remove place from list
                Places.Remove(currentPosition);
            }

            // -------------------------------------------------------------------
            // 2. Step: Things to do only when refresh is needed
            // -------------------------------------------------------------------

            if (IsLoaded == false || force == true)
            {
                if (_SettingsService.CurrentSettings.UseCurrentLocation)
                {
                    // User activated use of GPS in the settings
                    var geolocation = await _PlaceService.GetCurrentGeoLocationAsync();
                    if (geolocation != null)
                    {
                        if (currentPosition == null)
                        {
                            // Create a place for current GPS position
                            currentPosition = new Place(_LocalizationService.GetString("CurrentPosition"), geolocation.Zip, true);
                            
                            // Add place to service
                            await _PlaceService.AddPlaceAsync(currentPosition);

                            // Add place to top of the list
                            Places.Insert(0, currentPosition);
                        }
                        else
                        {
                            currentPosition.Zip = geolocation.Zip;
                        }
                    }
                    else
                    {
                        // TODO: Hande failed GPS locating
                    }
                }
                

                // Update all places
                foreach (var place in Places)
                {
                    await _PollenService.GetPollutionsForPlaceAsync(place);
                    place.RecalculateMaxPollution();
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
