using System;
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

        private RelayCommand _NavigateToEditPlaceCommand;
        public RelayCommand NavigateToEditPlaceCommand
        {
            get
            {
                return _NavigateToEditPlaceCommand ?? (_NavigateToEditPlaceCommand = new RelayCommand(() =>
                {
                    var addEditPlaceViewModel = SimpleIoc.Default.GetInstance<AddEditPlaceViewModel>();
                    addEditPlaceViewModel.CurrentPlace = _CurrentPlace;
                    addEditPlaceViewModel.PlaceName = CurrentPlace.Name;
                    addEditPlaceViewModel.PlaceZip = CurrentPlace.Zip;

                    _NavigationService.NavigateTo(ViewNames.AddEditPlace);
                }, () =>
                {
                    // Only execute, if place is not current position and already exists
                    var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                    return !_CurrentPlace.IsCurrentPosition && mainViewModel.Places.Contains(_CurrentPlace);
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

        private RelayCommand _SavePlaceCommand;
        public RelayCommand SavePlaceCommand
        {
            get
            {
                return _SavePlaceCommand ?? (_SavePlaceCommand = new RelayCommand(() =>
                {
                    var addEditPlaceViewModel = SimpleIoc.Default.GetInstance<AddEditPlaceViewModel>();
                    addEditPlaceViewModel.CurrentPlace = null;
                    addEditPlaceViewModel.PlaceName = CurrentPlace.Name;
                    addEditPlaceViewModel.PlaceZip = CurrentPlace.Zip;

                    _NavigationService.NavigateTo(ViewNames.AddEditPlace);
                }, () =>
                {
                    // Only execute, if place does not already exist
                    var mainViewModel = SimpleIoc.Default.GetInstance<MainViewModel>();
                    return !mainViewModel.Places.Contains(_CurrentPlace);
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

        public async Task RefreshAsync()
        {
            IsLoading = true;
            IsLoaded = false;

            if (!CurrentPlace.PollutionToday.Any() ||
                !CurrentPlace.PollutionTomorrow.Any() ||
                !CurrentPlace.PollutionAfterTomorrow.Any())
            {
                await _PollenService.GetPollutionsForPlaceAsync(CurrentPlace);
            }
            else
                _PollenService.UpdatePollenSelection(CurrentPlace);

            IsLoaded = true;
            IsLoading = false;
        }
	}
}

