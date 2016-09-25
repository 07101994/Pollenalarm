using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Pollenalarm.Backend.Models
{
    [Table (Name = "Pollution")]
    public class PollutionEntity
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column]
        public string City_Zip { get; set; }
        [Column]
        public int Pollen_Id { get; set; }
        [Column]
        public int ValueToday { get; set; }
        [Column]
        public int ValueTomorrow { get; set; }
        [Column]
        public int ValueAfterTomorrow { get; set; }
        [Column]
        public DateTime TimeStamp { get; set; }

        internal void AddValue(int day, int value)
        {
            switch (day)
            {
                case 0:
                    ValueToday = value;
                    break;
                case 1:
                    ValueTomorrow = value;
                    break;
                case 2:
                    ValueAfterTomorrow = value;
                    break;
            }
        }
    }
}
