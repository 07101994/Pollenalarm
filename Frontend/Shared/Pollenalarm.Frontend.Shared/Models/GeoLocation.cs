using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Frontend.Shared.Models
{
    public class GeoLocation
    {
        public long Longitute { get; set; }
        public long Latitute { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }
    }
}
