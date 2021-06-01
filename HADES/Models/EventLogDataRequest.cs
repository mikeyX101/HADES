using Microsoft.AspNetCore.Mvc;

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
		public string Date { get; set; }
	}
}
