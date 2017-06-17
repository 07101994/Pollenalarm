using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server.Tables;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Backend.AspNet.DataObjects
{
    [Table("Pollen")]
    public class PollenDto : IPollen, ITableData
    {
        #region Implementation of IPollen

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime BloomStart { get; set; }
        public DateTime BloomEnd { get; set; }
        public int ClinicalPollution { get; set; }
        public string ImageCredits { get; set; }

        #endregion

        #region Implementation of ITableData  for Azure Mobile App

        [Key]
        [TableColumn(TableColumnType.Id)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Index(IsClustered = true)]
        [TableColumn(TableColumnType.CreatedAt)]
        public DateTimeOffset? CreatedAt { get; set; }

        [TableColumn(TableColumnType.Deleted)]
        public bool Deleted { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [TableColumn(TableColumnType.UpdatedAt)]
        public DateTimeOffset? UpdatedAt { get; set; }

        [TableColumn(TableColumnType.Version)]
        [Timestamp]
        public byte[] Version { get; set; }

        #endregion
    }
}