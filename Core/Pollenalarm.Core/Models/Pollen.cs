using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Core.Models
{
    public class Pollen
    {
        public string Name { get; set; }
        public string Description { get; set; }
		public string ImageName { get { return $"{Name.Replace("ß", "ss").Replace("ä", "ae")}.png"; } }
    }
}