using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pollenalarm.Core.Models
{
    public class Pollen : INotifyPropertyChanged, ISearchResult
    {
		public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public DateTime BloomStart { get; set; }
		public DateTime BloomEnd { get; set; }
        public int ClinicalPollution { get; set; }
        public string ImageName { get { return $"{Name.Replace("ß", "ss").Replace("ä", "ae")}.png"; } }
		public string ImageCredits { get; set; }
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; RaisePropertyChanged(); }
        }

        // Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}