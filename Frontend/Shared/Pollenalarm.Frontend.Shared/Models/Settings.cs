using System.Collections.Generic;

namespace Pollenalarm.Frontend.Shared.Models
{
	public class Settings
	{
		public bool UseCurrentLocation { get; set; }
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