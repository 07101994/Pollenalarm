using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pollenalarm.Shared.ViewModels
{
    public class PollenViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BloomStart { get; set; }
        public DateTime BloomEnd { get; set; }
        public int ClinicalPollution { get; set; }
        public string ImageUrl { get; set; }
        public string ImageCredits { get; set; }
        public string Description { get; set; }

        public PollenViewModel(int id, string name, DateTime bloomStart, DateTime bloomEnd, int clinicalPollution, string imageUrl, string imageCredits, string description)
        {
            Id = id;
            Name = name;
            BloomStart = bloomStart;
            BloomEnd = bloomEnd;
            ClinicalPollution = clinicalPollution;
            ImageUrl = imageUrl;
            ImageCredits = imageCredits;
            Description = description;
        }
    }
}
