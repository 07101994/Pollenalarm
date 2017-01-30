using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IDialogService = Pollenalarm.Frontend.Shared.Services.IDialogService;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class AddEditPlaceViewModel : AsyncViewModelBase
    {
        private INavigationService _NavigationService;
        private IFileSystemService _FileSystemService;
        private IDialogService _DialogService;
        private ILocalizationService _LocalizationService;
        private PlaceService _PlaceService;

        public event InvalidEntriesEventHandler OnInvalidEntries;
        public delegate void InvalidEntriesEventHandler(object sender, EventArgs e);

        public event LocationFailedEventHandler OnLocationFailed;
        public delegate void LocationFailedEventHandler(object sender, EventArgs e);


        private Place _CurrentPlace;
        public Place CurrentPlace
        {
            get { return _CurrentPlace; }
            set { _CurrentPlace = value; RaisePropertyChanged(); }
        }

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

        private RelayCommand _AddEditPlaceCommand;
        public RelayCommand AddEditPlaceCommand
        {
            get
            {
                return _AddEditPlaceCommand ?? (_AddEditPlaceCommand = new RelayCommand(async () =>
                {
                    // Check if entered field are valid
                    if (string.IsNullOrWhiteSpace(_PlaceName) || !Regex.IsMatch(_PlaceZip, "^[0-9]*$") || _PlaceZip.Trim().Length != 5)
                    {
                        // Invalid entries
                        OnInvalidEntries?.Invoke(this, null);
                        return;
                    }

                    IsLoading = true;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();

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
                    await _FileSystemService.SaveObjectToFileAsync("places.json", mainViewModel.Places.ToList());

                    _PlaceName = string.Empty;
                    _PlaceZip = string.Empty;
                    _NavigationService.GoBack();

                    IsLoading = false;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();
                }, () => !IsLoading));
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

                    IsLoading = true;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();

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

                    IsLoading = false;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();
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
                    IsLoading = true;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();

                    var geolocation = await _PlaceService.GetCurrentGeoLocationAsync();
                    if (geolocation == null)
                    {
                        OnLocationFailed?.Invoke(this, null);
                        IsLoading = false;
                        AddEditPlaceCommand.RaiseCanExecuteChanged();
                        DeletePlaceCommand.RaiseCanExecuteChanged();
                        GetCurrentPositionCommand.RaiseCanExecuteChanged();
                        return;
                    }

                    // Update place fields
                    PlaceName = geolocation.Name;
                    PlaceZip = geolocation.Zip;

                    IsLoading = false;
                    AddEditPlaceCommand.RaiseCanExecuteChanged();
                    DeletePlaceCommand.RaiseCanExecuteChanged();
                    GetCurrentPositionCommand.RaiseCanExecuteChanged();
                }, () => !IsLoading));
            }
        }

        public AddEditPlaceViewModel(INavigationService navigationService, IFileSystemService fileSystemService, IDialogService dialogService, ILocalizationService localizationService, PlaceService placeService)
        {
            _NavigationService = navigationService;
            _FileSystemService = fileSystemService;
            _DialogService = dialogService;
            _LocalizationService = localizationService;
            _PlaceService = placeService;
        }
    }
}
