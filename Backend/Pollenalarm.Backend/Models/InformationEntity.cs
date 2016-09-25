using System;
using System.Data.Linq.Mapping;

namespace Pollenalarm.Backend
{	
	[Table (Name = "Information")]
	public class InformationEntity
	{
		[Column(IsPrimaryKey = true, AutoSync = AutoSync.OnInsert, IsDbGenerated = true)]
		public int Id { get; set; }
		[Column]
		public DateTime Date { get; set; }
		[Column]
		public string Text { get; set; }
	}
}

