using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Backend.Models
{
    [Table(Name = "Pollen")]
    public class PollenEntity
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column]
        public string Name { get; set; }
        [Column]
        public DateTime BloomStart { get; set; }
        [Column]
        public DateTime BloomEnd { get; set; }
        [Column]
        public int ClinicalPollution { get; set; }
        [Column]
        public string ImageUrl { get; set; }
        [Column]
        public string ImageCredits { get; set; }
        [Column]
        public string Description { get; set; }

        internal Pollen ToPollen()
        {
            return new Pollen
            {
                Name = Name,
                Description = Description
            };
        }
    }
}
