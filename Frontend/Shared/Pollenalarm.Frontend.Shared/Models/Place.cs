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
    public class Place : IPlace, ISearchResult
    {
        #region Implementation of IPlace

        public string Id { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }

        #endregion

        public int MaxPollutionToday { get; set; }
        public bool IsCurrentPosition { get; set; }
        public List<Pollution> PollutionToday { get; set; }
        public List<Pollution> PollutionTomorrow { get; set; }
        public List<Pollution> PollutionAfterTomorrow { get; set; }

        public Place()
        {
            Id = Guid.NewGuid().ToString();

            PollutionToday = new List<Pollution>();
            PollutionTomorrow = new List<Pollution>();
            PollutionAfterTomorrow = new List<Pollution>();
        }

        public Place(string name, string zip, bool isCurrentPosition)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Zip = zip;
            IsCurrentPosition = isCurrentPosition;

            PollutionToday = new List<Pollution>();
            PollutionTomorrow = new List<Pollution>();
            PollutionAfterTomorrow = new List<Pollution>();
        }

        public void RecalculateMaxPollution()
        {
            if (PollutionToday.Any())
                MaxPollutionToday = PollutionToday.Max(x => x.Intensity);
            else
                MaxPollutionToday = 0;
        }
    }
}
