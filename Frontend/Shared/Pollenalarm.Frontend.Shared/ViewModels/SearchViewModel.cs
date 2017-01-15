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
					var results = await _PollenService.SearchAsnyc(searchTerm);
					if (results != null)
					{
						if (SearchResults == null)
							SearchResults = new ObservableCollection<ISearchResult>();

						foreach (var result in results)
							SearchResults.Add(result);
					}
				}));
			}
		}

		public SearchViewModel(PollenService pollenService)
		{
			_PollenService = pollenService;
		}
	}
}
