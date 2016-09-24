using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Backend.Models
{
    [Table]
    public class City
    {
        [Column(IsPrimaryKey = true)]
        public string Zip { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public DateTime? LastUpdate { get; set; }
        [Column]
        public int IsFavorite { get; set; }
        [Column]
        public int UserCount { get; set; }

        public City()
        {

        }

        public City(string zip, string name, DateTime? lastUpdate, int isFavorite, int userCount)
        {
            Zip = zip;
            Name = name;
            LastUpdate = lastUpdate;
            IsFavorite = isFavorite;
            UserCount = userCount;
        }
    }
}
