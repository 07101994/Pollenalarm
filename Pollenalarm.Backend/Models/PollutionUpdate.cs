using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollenalarm.Backend.Models
{
    [Table]
    public class PollutionUpdate
    {
        [Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, IsDbGenerated = true)]
        public int Id { get; set; }
        [Column]
        public DateTime Date { get; set; }
        [Column]
        public int CrawlingDuration { get; set; }
        [Column]
        public int TransactionDuration { get; set; }
        [Column]
        public int OverallDuration { get; set; }
    }
}
