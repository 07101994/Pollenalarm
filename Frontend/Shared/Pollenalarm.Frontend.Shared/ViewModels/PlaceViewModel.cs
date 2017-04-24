using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Misc;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;
using IDialogService = Pollenalarm.Frontend.Shared.Services.IDialogService;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class PlaceViewModel : AsyncViewModelBase
    {
        private INavigationService _NavigationService;
        private IPollenService _PollenService;
        private ILocalizationService _LocalizationService;
        private PlaceService _PlaceService;

        public string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<PollutionGroup> _PollutionToday;
        public ObservableCollection<PollutionGroup> PollutionToday
        {
            get { return _PollutionToday; }
            set { _PollutionToday = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<PollutionGroup> _PollutionTomorrow;
        public ObservableCollection<PollutionGroup> PollutionTomorrow
        {
            get { return _PollutionTomorrow; }
            set { _PollutionTomorrow = value; RaisePropertyChanged(); }
        }

        public ObservableCollection<PollutionGroup> _PollutionAfterTomorrow;
        public ObservableCollection<PollutionGroup> PollutionAfterTomorrow
        {
            get { return _PollutionAfterTomorrow; }
            set { _PollutionAfterTomorrow = value; RaisePropertyChanged(); }
        }

        public bool ShowNoPlacesWarningToday { get { return !PollutionToday.Any() && !IsBusy; } }
        public bool ShowNoPlacesWarningTomorrow { get { return !PollutionTomorrow.Any() && !IsBusy; } }
        public bool ShowNoPlacesWarningAfterTomorrow { get { return !PollutionAfterTomorrow.Any() && !IsBusy; } }

        private RelayCommand _NavigateToEditPlaceCommand;
        public RelayCommand NavigateToEditPlaceCommand
        {
            get
            {
                return _NavigateToEditPlaceCommand ?? (_NavigateToEditPlaceCommand = new RelayCommand(() =>
                {
                    _NavigationService.NavigateTo(ViewNames.AddEditPlace);
                }, () =>
                {
                    // Only execute, if place is not current position and already exists
                    //var places = await _PlaceService.GetPlacesAsync();
                    return !_PlaceService.CurrentPlace.IsCurrentPosition && _PlaceService.Places.Contains(_PlaceService.CurrentPlace);
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
                    _PollenService.CurrentPollen = pollen;
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
                    _NavigationService.NavigateTo(ViewNames.AddEditPlace);
                }, () =>
                {
                    // Only execute, if place does not already exist
                    return !_PlaceService.Places.Contains(_PlaceService.CurrentPlace);
                }));
            }
        }

        public PlaceViewModel(INavigationService navigationService, IPollenService pollenService, ILocalizationService localizationService, PlaceService placeService)
        {
            _NavigationService = navigationService;
            _PollenService = pollenService;
            _LocalizationService = localizationService;
            _PlaceService = placeService;

            PollutionToday = new ObservableCollection<PollutionGroup>();
            PollutionTomorrow = new ObservableCollection<PollutionGroup>();
            PollutionAfterTomorrow = new ObservableCollection<PollutionGroup>();
        }

        public async Task RefreshAsync()
        {
            IsBusy = true;
            IsLoaded = false;

            RaisePropertyChanged(nameof(ShowNoPlacesWarningToday));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningTomorrow));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningAfterTomorrow));

            Name = _PlaceService.CurrentPlace.Name;

            if (!PollutionToday.Any() || !PollutionTomorrow.Any() || !PollutionAfterTomorrow.Any())
                await _PollenService.GetPollutionsForPlaceAsync(_PlaceService.CurrentPlace);
            else
                _PollenService.UpdatePollenSelection(_PlaceService.CurrentPlace);

            PollutionToday.Clear();
            PollutionTomorrow.Clear();
            PollutionAfterTomorrow.Clear();

            // Today
            var blooming = new PollutionGroup(_LocalizationService.GetString("BloomingGroupName"));
            var nonBlooming = new PollutionGroup(_LocalizationService.GetString("NonBloomingGroupName"));
            foreach (var pollution in _PlaceService.CurrentPlace.PollutionToday)
            {
                if (pollution.Intensity > 0)
                    blooming.Add(pollution);
                else
                    nonBlooming.Add(pollution);
            }

            if (blooming.Any())
                PollutionToday.Add(blooming);
            if (nonBlooming.Any())
                PollutionToday.Add(nonBlooming);

            // Tomorrow
            blooming = new PollutionGroup(_LocalizationService.GetString("BloomingGroupName"));
            nonBlooming = new PollutionGroup(_LocalizationService.GetString("NonBloomingGroupName"));
            foreach (var pollution in _PlaceService.CurrentPlace.PollutionTomorrow)
            {
                if (pollution.Intensity > 0)
                    blooming.Add(pollution);
                else
                    nonBlooming.Add(pollution);
            }
            if (blooming.Any())
                PollutionTomorrow.Add(blooming);
            if (nonBlooming.Any())
                PollutionTomorrow.Add(nonBlooming);

            // After Tomorrow
            blooming = new PollutionGroup(_LocalizationService.GetString("BloomingGroupName"));
            nonBlooming = new PollutionGroup(_LocalizationService.GetString("NonBloomingGroupName"));
            foreach (var pollution in _PlaceService.CurrentPlace.PollutionAfterTomorrow)
            {
                if (pollution.Intensity > 0)
                    blooming.Add(pollution);
                else
                    nonBlooming.Add(pollution);
            }
            if (blooming.Any())
                PollutionAfterTomorrow.Add(blooming);
            if (nonBlooming.Any())
                PollutionAfterTomorrow.Add(nonBlooming);


            RaisePropertyChanged(nameof(ShowNoPlacesWarningToday));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningTomorrow));
            RaisePropertyChanged(nameof(ShowNoPlacesWarningAfterTomorrow));

            IsLoaded = true;
            IsBusy = false;
        }
    }
}

