﻿using Newtonsoft.Json;

namespace HADES.Models
{
	/// <summary>
	/// Response to send event logs to DataTables.
	/// </summary>
	public class EventLogDataResponse : DataTables.DataTablesProcessingResponse
	{
		public EventLogDataResponse(int drawCall) : base(drawCall) { }

		/// <summary>
		/// Number of exceptions that occured while transforming JSON to objects for processing
		/// </summary>
		[JsonProperty(PropertyName = "exceptionCount")]
		public int NumberOfExceptions { get; set; } = 0;
	}
}
