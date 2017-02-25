using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Pollenalarm.Core.Models;
using Pollenalarm.Frontend.Shared.Misc;
using Pollenalarm.Frontend.Shared.Models;
using Pollenalarm.Frontend.Shared.Services;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
	public class SearchViewModel : AsyncViewModelBase
	{
		private INavigationService _NavigationService;
		private PollenService _PollenService;
		private GoogleMapsService _GoogleMapsService;

		private string _SearchTerm;
		public string SearchTerm
		{
			get { return _SearchTerm; }
			set { _SearchTerm = value; RaisePropertyChanged(); }
		}


		private ObservableCollection<SearchResultGroup> _SearchResults;
		public ObservableCollection<SearchResultGroup> SearchResults
		{
			get { return _SearchResults; }
			set { _SearchResults = value; RaisePropertyChanged(); }
		}

		private RelayCommand _SearchCommand;
		public RelayCommand SearchCommand
		{
			get
			{
				return _SearchCommand ?? (_SearchCommand = new RelayCommand(async () =>
				{
					IsBusy = true;
					SearchCommand.RaiseCanExecuteChanged();

					SearchResults.Clear();

					// Trim
					var trimmedSearchTerm = SearchTerm.Trim();
					if (trimmedSearchTerm.Length == 0)
					{
						IsBusy = false;
						SearchCommand.RaiseCanExecuteChanged();
						return;
					}

					// Search pollen
					var pollenResults = new SearchResultGroup("Pollen");
					var allPollen = await _PollenService.GetAllPollenAsync();
					var foundPollen = allPollen.Where(p => p.Name.ToLower().Contains(trimmedSearchTerm.ToLower()));

					if (foundPollen.Any())
					{
						foreach (var result in foundPollen)
							pollenResults.Add(result);
					}

					// Search places
					var placeResults = new SearchResultGroup("Places");
					var foundPlaces = await _GoogleMapsService.GeoCodeAsync(trimmedSearchTerm);
					if (foundPlaces != null && foundPlaces.Any())
					{
						foreach (var result in foundPlaces)
							placeResults.Add(result);
					}

					SearchResults.Add(pollenResults);
					SearchResults.Add(placeResults);

					IsBusy = false;
					SearchCommand.RaiseCanExecuteChanged();
				}, () => !IsBusy));
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

		private RelayCommand<Place> _NavigateToPlaceCommand;
		public RelayCommand<Place> NavigateToPlaceCommand
		{
			get
			{
				return _NavigateToPlaceCommand ?? (_NavigateToPlaceCommand = new RelayCommand<Place>((Place place) =>
				{
					var placeViewModel = SimpleIoc.Default.GetInstance<PlaceViewModel>();
					placeViewModel.CurrentPlace = place;
					_NavigationService.NavigateTo(ViewNames.Place);
				}));
			}
		}

		public SearchViewModel(INavigationService navigationService, PollenService pollenService, GoogleMapsService googleMapsService)
		{
			_NavigationService = navigationService;
			_PollenService = pollenService;
			_GoogleMapsService = googleMapsService;

			SearchResults = new ObservableCollection<SearchResultGroup>();
		}
	}
}
