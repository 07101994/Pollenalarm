using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Shared.ViewModels
{
    public class PollutionViewModel
    {
        public int Id { get; set; }
        public int ValueToday { get; set; }
        public int ValueTomorrow { get; set; }
        public int ValueAfterTomorrow { get; set; }
        public DateTime TimeStamp { get; set; }
        public CityViewModel City { get; set; }
        public PollenViewModel Pollen { get; set; }

        public PollutionViewModel(int id, int valueToday, int valueTomorrow, int valueAfterTomorrow, DateTime timeStamp, CityViewModel city, PollenViewModel pollen)
        {
            Id = id;
            ValueToday = valueToday;
            ValueTomorrow = valueTomorrow;
            ValueAfterTomorrow = valueAfterTomorrow;
            TimeStamp = timeStamp;
            City = city;
            Pollen = pollen;
        }
    }
}
