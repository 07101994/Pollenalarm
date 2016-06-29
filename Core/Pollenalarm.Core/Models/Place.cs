using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Core.Models
{
    public class Place
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Zip { get; set; }

        public Place()
        {
            Id = new Guid();
        }
    }
}