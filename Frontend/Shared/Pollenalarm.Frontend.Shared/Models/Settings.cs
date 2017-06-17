using System.Collections.Generic;

namespace Pollenalarm.Frontend.Shared.Models
{
	public class Settings
	{
		public bool UseCurrentLocation { get; set; }
		public Dictionary<string, bool> SelectedPollen { get; set; }

		public Settings()
		{
			SelectedPollen = new Dictionary<string, bool>();
		}

		public Settings Clone()
		{
			return (Settings)MemberwiseClone();
		}
	}
}