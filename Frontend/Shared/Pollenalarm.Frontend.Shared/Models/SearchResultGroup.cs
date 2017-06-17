using System.Collections.ObjectModel;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class SearchResultGroup : ObservableCollection<ISearchResult>
    {
        public string Title { get; set; }

        public SearchResultGroup(string title)
        {
            this.Title = title;
        }

        public static ObservableCollection<SearchResultGroup> All { get; private set; }
    }
}
