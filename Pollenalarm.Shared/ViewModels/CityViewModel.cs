using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pollenalarm.Shared.ViewModels
{
    public class CityViewModel
    {
        public string Zip { get; set; }
        public string Name { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int IsFavorite { get; set; }
        public int UserCount { get; set; }

        public CityViewModel(string zip, string name, DateTime? lastUpdate, int isFavorite, int userCount)
        {
            Zip = zip;
            Name = name;
            LastUpdate = lastUpdate;
            IsFavorite = isFavorite;
            UserCount = userCount;
        }
    }
}
