using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models.DataTables
{
	/// <summary>
	/// Request coming from a Datatable requiring server-side processing.
	/// </summary>
	public class DataTablesProcessingRequest
	{
		/// <summary>
		/// Number of drawn call for Datatables.<br/>
		/// DataTables recommands parsing this value to an integer to avoid potential XSS attacks.
		/// </summary>
		[FromForm(Name = "draw")]
		public int DrawCall { get; set; }

		/// <summary>
		/// Based-0 index of the first record to get.
		/// </summary>
		[FromForm(Name = "start")]
		public int StartAtRecord { get; set; }

		/// <summary>
		/// Number of records to get from the starting index.
		/// </summary>
		[FromForm(Name = "length")]
		public int RecordsCount { get; set; }

		/// <summary>
		/// Order by what column? (by index)
		/// </summary>
		[FromForm(Name = "order[0][column]")]
		public int OrderedWith { get; set; }

		/// <summary>
		/// Is the ordering Ascending or Descending?
		/// </summary>
		[FromForm(Name = "order[0][dir]")]
		public DataTableOrderDirection OrderDirection { get; set; }

		private string search = "";
		/// <summary>
		/// Search to do in columns.<br />
		/// Returns an empty string if null, to get every record.
		/// </summary>
		[FromForm(Name = "search[value]")]
		public string Search {
			get => search ?? "";
			set => search = value;
		}
	}

	public enum DataTableOrderDirection
	{
		ASC,
		DESC
	}
}
