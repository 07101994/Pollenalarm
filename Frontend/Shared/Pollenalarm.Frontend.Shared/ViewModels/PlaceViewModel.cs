﻿using System;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Pollenalarm.Frontend.Shared.Misc;
using Pollenalarm.Frontend.Shared.Services;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IDialogService = Pollenalarm.Frontend.Shared.Services.IDialogService;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class PlaceViewModel : AsyncViewModelBase
	{
        private INavigationService _NavigationService;
        private IFileSystemService _FileSystemService;
        private IDialogService _DialogService;
        private ILocalizationService _LocalizationService;
        private PlaceService _PlaceService;
        private PollenService _PollenService;

        private Place _CurrentPlace;
		public Place CurrentPlace
		{
			get { return _CurrentPlace; }
			set { _CurrentPlace = value; RaisePropertyChanged(); }
        }

        #region Add / Edit fields

        private string _PlaceName;
        public string PlaceName
        {
            get { return _PlaceName; }
            set { _PlaceName = value; RaisePropertyChanged(); }
        }

        private string _PlaceZip;
        public string PlaceZip
        {
            get { return _PlaceZip; }
            set { _PlaceZip = value; RaisePropertyChanged(); }
        }

        #endregion

        public event InvalidEntriesEventHandler OnInvalidEntries;
        public delegate void InvalidEntriesEventHandler(object sender, EventArgs e);

        public event LocationFailedEventHandler OnLocationFailed;
        public delegate void LocationFailedEventHandler(object sender, EventArgs e);

        private RelayCommand _AddEditPlaceCommand;
        public RelayCommand AddEditPlaceCommand
        {
            get
            {
                return _AddEditPlaceCommand ?? (_AddEditPlaceCommand = new RelayCommand(() =>
                {
                    // Check if entered field are valid
                    if (string.IsNullOrWhiteSpace(_PlaceName) || !Regex.IsMatch(_PlaceZip, "^[0-9]*$") || _PlaceZip.Trim().Length != 5)
                    {
                        // Invalid entries
                        OnInvalidEntries?.Invoke(this, null);
                        return;
                    }

                    var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();

                    if (_CurrentPlace != null)
                    {
                        // Update existing place
                        var existingPlace = mainViewModel.Places.FirstOrDefault(x => x.Id == _CurrentPlace.Id);
                        if (existingPlace != null)
                        {
                            existingPlace.Name = _PlaceName;
                            existingPlace.Zip = _PlaceZip;
                            _CurrentPlace = existingPlace;
                        }
                    }
                    else
                    {
                        // Add new place
                        _CurrentPlace = new Place();
                        _CurrentPlace.Name = _PlaceName;
                        _CurrentPlace.Zip = _PlaceZip;
                        mainViewModel.Places.Add(_CurrentPlace);
                        // Set IsLoaded to false to force MainViewModel to refresh and load pollen for the new place
                        mainViewModel.IsLoaded = false;
                        _CurrentPlace = null;
                    }

                    // Save places
                    _FileSystemService.SaveObjectToFileAsync("places.json", mainViewModel.Places.ToList());

                    _PlaceName = string.Empty;
                    _PlaceZip = string.Empty;
                    _NavigationService.GoBack();
                }));
            }
        }

        private RelayCommand _DeletePlaceCommand;
        public RelayCommand DeletePlaceCommand
        {
            get
            {
                return _DeletePlaceCommand ?? (_DeletePlaceCommand = new RelayCommand(async () =>
                {
                    // Let user confirm deletion
                    if (!await _DialogService.DisplayConfirmationAsync(_LocalizationService.GetString("DeletePlaceTitle"), _LocalizationService.GetString("DeletePlaceMessage"), _LocalizationService.GetString("Delete"), _LocalizationService.GetString("Cancel")))
                        return;

                    var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();

                    if (_CurrentPlace != null)
                    {
                        var existingPlace = mainViewModel.Places.FirstOrDefault(x => x.Id == _CurrentPlace.Id);
                        if (existingPlace != null)
                        {
                            mainViewModel.Places.Remove(existingPlace);
                            await _FileSystemService.SaveObjectToFileAsync("places.json", mainViewModel.Places.ToList());
                            _CurrentPlace = null;
                            _PlaceName = string.Empty;
                            _PlaceZip = string.Empty;
                            _NavigationService.GoBack();
                        }
                    }
                }));
            }
        }

        private RelayCommand _NavigateToEditPlaceCommand;
        public RelayCommand NavigateToEditPlaceCommand
        {
            get
            {
                return _NavigateToEditPlaceCommand ?? (_NavigateToEditPlaceCommand = new RelayCommand(() =>
                {
                    _PlaceName = _CurrentPlace.Name;
                    _PlaceZip = _CurrentPlace.Zip;
                    _NavigationService.NavigateTo(ViewNames.AddEditPlace);
                }));
            }
        }

        private RelayCommand<Pollen> _NavigateToPollenCommand;
        public RelayCommand<Pollen> NavigateToPollenCommand
        {
            get
            {
                return _NavigateToPollenCommand ?? (_NavigateToPollenCommand = new RelayCommand<Pollen>((Pollen pollen) =>
                {
                    var pollenViewModel = SimpleIoc.Default.GetInstance<PollenViewModel>();
                    pollenViewModel.CurrentPollen = pollen;
                    _NavigationService.NavigateTo(ViewNames.Pollen);
                }));
            }
        }

        private RelayCommand _GetCurrentPositionCommand;
        public RelayCommand GetCurrentPositionCommand
        {
            get
            {
                return _GetCurrentPositionCommand ?? (_GetCurrentPositionCommand = new RelayCommand(async () =>
                {
                    var geolocation = await _PlaceService.GetCurrentGeoLocationAsync();
                    if (geolocation == null)
                    {
                        OnLocationFailed?.Invoke(this, null);
                        return;
                    }

                    // Update place fields
                    PlaceName = geolocation.Name;
					PlaceZip = geolocation.Zip;
                }));
            }
        }

        public PlaceViewModel(INavigationService navigationService, IFileSystemService fileSystemService, IDialogService dialogService, ILocalizationService localizationService, PlaceService placeService, PollenService pollenService)
		{
            _NavigationService = navigationService;
            _FileSystemService = fileSystemService;
            _DialogService = dialogService;
            _LocalizationService = localizationService;
            _PlaceService = placeService;
            _PollenService = pollenService;
        }

        public void Update()
        {
            _PollenService.UpdatePollenSelection(CurrentPlace);
        }
	}
}
