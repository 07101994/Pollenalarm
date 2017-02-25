using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class Pollen : IPollen, INotifyPropertyChanged, ISearchResult
    {
        #region Implementation of IPollen

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime BloomStart { get; set; }
        public DateTime BloomEnd { get; set; }
        public int ClinicalPollution { get; set; }
        public string ImageCredits { get; set; }

        #endregion

        public string ImageName { get { return $"{Name.Replace("ß", "ss").Replace("ä", "ae")}.png"; } }
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
