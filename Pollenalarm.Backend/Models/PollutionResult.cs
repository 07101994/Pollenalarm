using Pollenalarm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pollenalarm.Backend.Models
{
    public class PollutionResult
    {
        public List<Pollution> Today { get; set; }
        public List<Pollution> Tomorrow { get; set; }
        public List<Pollution> AfterTomorrow { get; set; }
    }
}