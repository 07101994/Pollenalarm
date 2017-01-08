using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Misc;
using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDialogService = Pollenalarm.Frontend.Shared.Services.IDialogService;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class MainViewModel : AsyncViewModelBase
    {
        private INavigationService _NavigationService;
        private IFileSystemService _FileSystemService;
		private ILocalizationService _LocalizationService;
        private PollenService _PollenService;
        private SettingsService _SettingsService;
        private PlaceService _PlaceService;
        private PlaceViewModel _PlaceViewModel;

        private ObservableCollection<Place> _Places;
        public ObservableCollection<Place> Places
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
                    _PlaceViewModel.CurrentPlace = null;
                    _NavigationService.NavigateTo(ViewNames.AddEditPlace);
                }));
            }
        }

        private RelayCommand<Place> _NavigateToPlaceCommand;
        public RelayCommand<Place> NavigateToPlaceCommand
        {
            get
            {
                return _NavigateToPlaceCommand ?? (_NavigateToPlaceCommand = new RelayCommand<Place>((Place place) =>
                {
                    _PlaceViewModel.CurrentPlace = place;
                    _NavigationService.NavigateTo(ViewNames.Place);
                }));
            }
        }

		public MainViewModel(INavigationService navigationService, IFileSystemService fileSystemService, ILocalizationService localizationService, SettingsService settingsService, PollenService pollenService, PlaceService placeService, PlaceViewModel placeViewModel)
        {
            _NavigationService = navigationService;
            _FileSystemService = fileSystemService;
			_LocalizationService = localizationService;
            _PollenService = pollenService;
            _SettingsService = settingsService;
            _PlaceService = placeService;
            _PlaceViewModel = placeViewModel;

            Places = new ObservableCollection<Place>();
        }

        public async Task RefreshAsync()
        {
            IsLoading = true;

			// Update greeting header
			var now = DateTime.Now;
			if (now.Hour > 5 && now.Hour < 11)
				GreetingHeader = _LocalizationService.GetString("GoodMorning");
			else if (now.Hour >= 11 && now.Hour < 14)
				GreetingHeader = _LocalizationService.GetString("GoodDay");
			else if (now.Hour >= 14 && now.Hour < 18)
				GreetingHeader = _LocalizationService.GetString("GoodEvening");
			else
				GreetingHeader = _LocalizationService.GetString("GoodNight");

			// Clear places
            Places.Clear();

            // Check settings
            await _SettingsService.LoadSettingsAsync();
            if (_SettingsService.CurrentSettings.UseCurrentLocation)
            {
                // Add current location
                var geolocation = await _PlaceService.GetCurrentGeoLocationAsync();
                if (geolocation != null)
                {
                    var currentPlace = new Place();
                    currentPlace.Name = "Current position";
                    currentPlace.Zip = geolocation.Zip;
                    currentPlace.IsCurrentPosition = true;
                    Places.Add(currentPlace);
                }
            }

            // Load locally saved places
            var savedPlaces = await _FileSystemService.ReadObjectFromFileAsync<List<Place>>("places.json");
            if (savedPlaces != null)
            {
                foreach (var place in savedPlaces)
                {
                    Places.Add(place);
                }
            }

            // Update places
            foreach (var place in Places)
            {
                var success = await _PollenService.GetPollutionsForPlaceAsync(place);
                place.RecalculateMaxPollution();
            }

            IsLoading = false;
            IsLoaded = true;
        }
    }
}
