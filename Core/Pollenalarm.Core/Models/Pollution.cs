using System;

namespace Pollenalarm.Core.Models
{
    public class Pollution
	{
		public Pollen Pollen { get; set; }
		public DateTime Date { get; set; }
		public int Intensity { get; set; }
	}
}
