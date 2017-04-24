using System;

namespace Pollenalarm.Core.Models
{
    public interface IPollution
    {
        string Id { get; set; }
        string Zip { get; set; }
        string PollenId { get; set; }
        DateTime Date { get; set; }
        DateTime Updated { get; set; }
        int Intensity { get; set; }
    }
}
