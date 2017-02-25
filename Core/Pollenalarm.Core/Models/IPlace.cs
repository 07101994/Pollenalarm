using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Pollenalarm.Core.Models
{
    public interface IPlace
    {
        string Id { get; set; }
        string Name { get; set; }
        string Zip { get; set; }
    }    
}