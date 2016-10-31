using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class Settings
    {
        public bool UseCurrentLocation { get; set; }
        public bool ShowSelectedPollenOnly { get; set; }
        public Dictionary<int, bool> SelectedPollen { get; set; }

		public Settings()
		{
			SelectedPollen = new Dictionary<int, bool>();
		}

        public Settings Clone()
        {
            return (Settings)MemberwiseClone();
        }
    }
}