using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Core;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class Place : IPlace, INotifyPropertyChanged, ISearchResult
    {
        #region Implementation of IPlace

        public string Id { get; set; }
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; RaisePropertyChanged(); }
        }
        public string Zip { get; set; }

        #endregion

        private int _MaxPollutionToday;
        public int MaxPollutionToday
        {
            get { return _MaxPollutionToday; }
            set { _MaxPollutionToday = value; RaisePropertyChanged(); }
        }

        public bool IsCurrentPosition { get; set; }

        public ObservableCollection<Pollution> PollutionToday { get; set; }
        public ObservableCollection<Pollution> PollutionTomorrow { get; set; }
        public ObservableCollection<Pollution> PollutionAfterTomorrow { get; set; }

        public Place()
        {
            Id = Guid.NewGuid().ToString();

            PollutionToday = new ObservableCollection<Pollution>();
            PollutionTomorrow = new ObservableCollection<Pollution>();
            PollutionAfterTomorrow = new ObservableCollection<Pollution>();
        }

        public void RecalculateMaxPollution()
        {
            if (PollutionToday.Any())
                MaxPollutionToday = PollutionToday.Max(x => x.Intensity);
            else
                MaxPollutionToday = 0;
        }

        // Implementation of INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
