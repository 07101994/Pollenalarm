using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Pollenalarm.Core;
using Pollenalarm.Frontend.Shared.Services;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class SearchViewModel : AsyncViewModelBase
    {
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
                    SearchResults.Clear();

                    // Trim
                    var trimmedSearchTerm = SearchTerm.Trim();
                    if (trimmedSearchTerm.Length == 0)
                        return;

                    // Search pollen
                    var pollenResults = new SearchResultGroup("Pollen");
                    var allPollen = await _PollenService.GetAllPollenAsync();
                    var foundPollen = allPollen.Where(p => p.Name.ToLower().Contains(trimmedSearchTerm.ToLower()) || p.Description.ToLower().Contains(trimmedSearchTerm.ToLower()));

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
				}));
			}
		}

		public SearchViewModel(PollenService pollenService, GoogleMapsService googleMapsService)
		{
			_PollenService = pollenService;
            _GoogleMapsService = googleMapsService;

            SearchResults = new ObservableCollection<SearchResultGroup>();
        }
	}
}
