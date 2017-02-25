using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Azure.Mobile.Server.Tables;
using Pollenalarm.Core.Models;

namespace Pollenalarm.Backend.AspNet.DataObjects
{
    [Table("Information")]
    public class InformationDto : IInformation, ITableData
    {
        #region Implementation of IInformation

        public DateTime Date { get; set; }
        public string Text { get; set; }
        public string Language { get; set; }

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