using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Core.Models
{
    public class Pollen
    {
		public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
		public DateTime BloomStart { get; set; }
		public DateTime BloomEnd { get; set; }
        public int ClinicalPollution { get; set; }
        public string ImageName { get { return $"{Name.Replace("ß", "ss").Replace("ä", "ae")}.png"; } }
		public string ImageCredits { get; set; }
		public bool IsSelected { get; set; }
    }
}