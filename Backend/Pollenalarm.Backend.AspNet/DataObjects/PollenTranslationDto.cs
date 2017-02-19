using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server.Tables;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Backend.AspNet.DataObjects
{
    [Table(nameof(PollenTranslation))]
    public class PollenTranslationDto : PollenTranslation, ITableData
    {
        #region ITableData implementation for Azure Mobile App

        [Key]
        [TableColumn(TableColumnType.Id)]
        public new string Id { get; set; }

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