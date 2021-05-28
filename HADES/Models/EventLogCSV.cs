
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models
{
	public class EventLogCSVMap : ClassMap<EventLogCSV>
	{
		public EventLogCSVMap()
		{
			Map(m => m.TimeStamp).Index(0).Name("TimeStamp");
			Map(m => m.Level).Index(1).Name("Level");
			Map(m => m.User).Index(2).Name("User");
			Map(m => m.Message).Index(3).Name("Message");
			Map(m => m.Exception).Index(4).Name("Exception");
		}
	}

	public class EventLogCSV
	{
		public EventLogCSV(string timeStamp, string level, string user, string message, string exception)
		{
			TimeStamp = timeStamp;
			Level = level;
			User = user;
			Message = message;
			Exception = exception;
		}

		public string TimeStamp { get; set; }

		public string Level { get; set; }

		public string User { get; set; }

		public string Message { get; set; }

		public string Exception { get; set; }
	}
}
