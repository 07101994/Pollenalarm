using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class GeoLocation
    {
        public double Longitute { get; set; }
        public double Latitute { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }
    }
}
