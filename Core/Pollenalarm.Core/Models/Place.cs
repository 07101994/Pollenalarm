using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Core.Models
{
    public class Place
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }
        public int MaxPollutionToday { get; set; }
        public bool IsCurrentPosition { get; set; }

        public ObservableCollection<Pollution> PollutionToday { get; set; }
        public ObservableCollection<Pollution> PollutionTomorrow { get; set; }
        public ObservableCollection<Pollution> PollutionAfterTomorrow { get; set; }

        public Place()
        {
            Id = new Guid();

            PollutionToday = new ObservableCollection<Pollution>();
            PollutionTomorrow = new ObservableCollection<Pollution>();
            PollutionAfterTomorrow = new ObservableCollection<Pollution>();
        }

        public void RecalculateMaxPollution()
        {
            if (PollutionToday.Any())
                MaxPollutionToday = PollutionToday.Max(x => x.Intensity);
            else
                MaxPollutionToday = -1;
        }
    }
}