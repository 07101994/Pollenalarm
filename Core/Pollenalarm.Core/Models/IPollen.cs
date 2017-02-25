using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Pollenalarm.Core.Models
{
    public interface IPollen
    {
        string Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime BloomStart { get; set; }
        DateTime BloomEnd { get; set; }
        int ClinicalPollution { get; set; }
        string ImageCredits { get; set; }
    }
}