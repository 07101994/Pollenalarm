using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Core.Models
{
    public class Pollution
    {
        public Pollen Pollen { get; set; }
        public DateTime Date { get; set; }
        public int Intensity { get; set; }
    }
}
