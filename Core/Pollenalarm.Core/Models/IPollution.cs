using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pollenalarm.Core.Models
{
    public interface IPollution
    {
        string Id { get; set; }
        string Zip { get; set; }
        string PollenId { get; set; }
        DateTime Date { get; set; }
        int Intensity { get; set; }
    }
}
