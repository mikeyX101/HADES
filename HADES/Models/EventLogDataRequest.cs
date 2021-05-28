using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
	/// <summary>
	/// Request to get event logs for DataTables.
	/// </summary>
	public class EventLogDataRequest : DataTables.DataTablesProcessingRequest
	{
		/// <summary>
		/// Log Date.
		/// </summary>
		[FromForm(Name = "date")]
		public DateTime Date { get; set; }
	}
}
