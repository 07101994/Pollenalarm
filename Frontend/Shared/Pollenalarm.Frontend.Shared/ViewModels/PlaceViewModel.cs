using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Misc;
using Pollenalarm.Frontend.Shared.Services;
using IDialogService = Pollenalarm.Frontend.Shared.Services.IDialogService;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class PlaceViewModel : AsyncViewModelBase
	{
        private INavigationService _NavigationService;
        private PollenService _PollenService;

        private Place _CurrentPlace;
		public Place CurrentPlace
		{
			get { return _CurrentPlace; }
			set { _CurrentPlace = value; RaisePropertyChanged(); }
        }

        public bool ShowNoPlacesWarningToday { get { return !CurrentPlace.PollutionToday.Any() && !IsBusy; }}
        public bool ShowNoPlacesWarningTomorrow { get { return !CurrentPlace.PollutionTomorrow.Any() && !IsBusy; }}
        public bool ShowNoPlacesWarningAfterTomorrow { get { return !CurrentPlace.PollutionAfterTomorrow.Any() && !IsBusy; } }


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

        public PlaceViewModel(INavigationService navigationService, PollenService pollenService)
		{
            _NavigationService = navigationService;
            _PollenService = pollenService;
        }

        public async Task RefreshAsync()
        {
            IsBusy = true;
            IsLoaded = false;

            RaisePropertyChanged(nameof(ShowNoPlacesWarningToday));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningTomorrow));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningAfterTomorrow));

            if (!CurrentPlace.PollutionToday.Any() ||
                !CurrentPlace.PollutionTomorrow.Any() ||
                !CurrentPlace.PollutionAfterTomorrow.Any())
            {
                await _PollenService.GetPollutionsForPlaceAsync(CurrentPlace);
            }
            else
                _PollenService.UpdatePollenSelection(CurrentPlace);

            RaisePropertyChanged(nameof(ShowNoPlacesWarningToday));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningTomorrow));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningAfterTomorrow));

            IsLoaded = true;
            IsBusy = false;
        }
	}
}

