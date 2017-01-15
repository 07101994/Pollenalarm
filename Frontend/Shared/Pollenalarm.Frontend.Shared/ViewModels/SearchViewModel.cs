using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Pollenalarm.Core;
using Pollenalarm.Frontend.Shared.Services;

namespace Pollenalarm.Frontend.Shared.ViewModels
{
    public class SearchViewModel : AsyncViewModelBase
    {
		private PollenService _PollenService;
        private GoogleMapsService _GoogleMapsService;

		private ObservableCollection<ISearchResult> _SearchResults;
		public ObservableCollection<ISearchResult> SearchResults
		{
			get { return _SearchResults; }
			set { _SearchResults = value; RaisePropertyChanged(); }
		}

		private RelayCommand<string> _SearchCommand;
		public RelayCommand<string> SearchCommand
		{
			get
			{
				return _SearchCommand ?? (_SearchCommand = new RelayCommand<string>(async (string searchTerm) =>
				{
                    // Trim
                    var trimmedSearchTerm = searchTerm.Trim();

                    SearchResults.Clear();

                    // Search pollen
                    var allPollen = await _PollenService.GetAllPollenAsync();
                    var pollenResults = allPollen.Where(p => p.Name.ToLower().Contains(trimmedSearchTerm.ToLower()) || p.Description.ToLower().Contains(trimmedSearchTerm.ToLower()));

                    if (pollenResults.Any())
                    {
                        foreach (var result in pollenResults)
                            SearchResults.Add(result);
                    }

                    // Search places
                    var placeResults = await _GoogleMapsService.GeoCodeAsync(trimmedSearchTerm);
                    if (placeResults != null && placeResults.Any())
                    {
                        foreach (var result in placeResults)
                            SearchResults.Add(result);
                    }
				}));
			}
		}

		public SearchViewModel(PollenService pollenService, GoogleMapsService googleMapsService)
		{
			_PollenService = pollenService;
            _GoogleMapsService = googleMapsService;

            SearchResults = new ObservableCollection<ISearchResult>();
        }
	}
}
