using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models.DataTables
{
	/// <summary>
	/// Response to send back to DataTables when it requires server-side processing
	/// </summary>
	public class DataTablesProcessingResponse
	{
		public DataTablesProcessingResponse(int drawCall) {
			DrawCall = drawCall;
		}

		/// <summary>
		/// Number of drawn call for Datatables.<br/>
		/// DataTables recommands parsing this value to an integer to avoid potential XSS attacks.
		/// </summary>
		[JsonProperty(PropertyName = "draw")]
		public int DrawCall { get; private set; }

		/// <summary>
		/// Total number of records in the data source.
		/// </summary>
		[JsonProperty(PropertyName = "recordsTotal")]
		public int RecordsTotal { get; set; }

		/// <summary>
		/// Number of records that got filtered.
		/// </summary>
		[JsonProperty(PropertyName = "recordsFiltered")]
		public int RecordsFiltered { get; set; }

		/// <summary>
		/// Record Data.
		/// </summary>
		[JsonProperty(PropertyName = "data")]
		public string Data { get; set; } = "[]";

		/// <summary>
		/// Error message if any errors occured.<br />
		/// Omit if no errors occured.
		/// </summary>
		[JsonProperty(PropertyName = "error", NullValueHandling = NullValueHandling.Ignore)]
		public string Error { get; set; }
	}
}
