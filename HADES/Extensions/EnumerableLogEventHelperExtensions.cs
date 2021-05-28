using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Extensions
{
	public static class EnumerableLogEventHelperExtensions
	{
		public static IEnumerable<string> FilterEvents(this IEnumerable<string> events, string searchTerm, out int filteredRecords)
		{
			string searchTermL = searchTerm.ToLower();
			IEnumerable<string> filteredEvents = events
				.Where(e => e.ToLower().Contains(searchTermL));

			filteredRecords = filteredEvents.Count();
			return filteredEvents;
		}

		public static IEnumerable<string> FilterEvents(this IEnumerable<string> events, string searchTerm)
		{
			string searchTermL = searchTerm.ToLower();
			IEnumerable<string> filteredEvents = events
				.Where(e => e.ToLower().Contains(searchTermL));

			return filteredEvents;
		}

		public static IEnumerable<string> PageEvents(this IEnumerable<string> events, int firstRecordIndex, int recordCount)
		{
			return events.Skip(firstRecordIndex).Take(recordCount);
		}

		// Unused for now
		public static IOrderedEnumerable<LogEvent> OrderEvents(this IEnumerable<LogEvent> events, int columnIndex, Models.DataTables.DataTableOrderDirection orderDirection)
		{
			LogEventColumns column = (LogEventColumns)columnIndex;
			IOrderedEnumerable<LogEvent> orderedEvents;
			// Could read column definitions sent by DataTables for more flexibility, having the columns hard-coded will do the job
			if (orderDirection == Models.DataTables.DataTableOrderDirection.ASC)
			{
				orderedEvents = column switch
				{
					LogEventColumns.Timestamp => orderedEvents = events.OrderBy(e => e.Timestamp),
					LogEventColumns.Level => orderedEvents = events.OrderBy(e => e.Level),
					LogEventColumns.User => orderedEvents = events.OrderBy(e => e.Properties.TryGetValue("User", out LogEventPropertyValue property) ? property.ToString() : "unknown"),
					LogEventColumns.Message => orderedEvents = events.OrderBy(e => e.RenderMessage()),
					LogEventColumns.Exception => orderedEvents = events.OrderBy(e => e.Exception.ToString()),
					_ => orderedEvents = events.OrderBy(e => e.Timestamp),
				};
			}
			else
			{
				orderedEvents = column switch
				{
					LogEventColumns.Timestamp => orderedEvents = events.OrderByDescending(e => e.Timestamp),
					LogEventColumns.Level => orderedEvents = events.OrderByDescending(e => e.Level),
					LogEventColumns.User => orderedEvents = events.OrderByDescending(e => e.Properties.TryGetValue("User", out LogEventPropertyValue property) ? property.ToString() : "unknown"),
					LogEventColumns.Message => orderedEvents = events.OrderByDescending(e => e.RenderMessage()),
					LogEventColumns.Exception => orderedEvents = events.OrderByDescending(e => e.Exception.ToString()),
					_ => orderedEvents = events.OrderByDescending(e => e.Timestamp),
				};
			}

			return orderedEvents;
		}

		private enum LogEventColumns
		{
			Timestamp = 0,
			Level = 1,
			User = 2,
			Message = 3,
			Exception = 4
		}

		/// <summary>
		/// This is a final operation and will read data from the data source.
		/// </summary>
		public static IEnumerable<LogEvent> ToLogEvents(this IEnumerable<string> events, out List<Exception> exceptions)
		{
			List<Exception> exps = new();
			IEnumerable<LogEvent> transformedEvents = events.Select(e => {
				try
				{
					return Serilog.Formatting.Compact.Reader.LogEventReader.ReadFromString(e);
				}
				catch (Exception ex)
				{
					exps.Add(ex);
					return null;
				}
			});
			exceptions = exps;
			return transformedEvents.Where(e => e != null);
		}

		public static IEnumerable<Models.EventLogCSV> ToEventLogCSVs(this IEnumerable<LogEvent> events)
		{
			return events.Select(e => new Models.EventLogCSV(
				e.Timestamp.ToString("o"), 
				e.Level.ToString(),
				e.Properties.TryGetValue("User", out LogEventPropertyValue property) ? property.ToString() : "unknown",
				e.RenderMessage(),
				e.Exception?.ToString() ?? "-"
			));
		}

		public static string RenderEvents(this IEnumerable<LogEvent> events)
		{
			using MemoryStream renderedLogs = new();
			using StreamWriter renderedLogsWriter = new(renderedLogs, encoding: System.Text.Encoding.UTF8, leaveOpen: true);
			using StreamReader renderedLogsReader = new(renderedLogs, encoding: System.Text.Encoding.UTF8, leaveOpen: true);

			Serilog.Formatting.ITextFormatter formatter = new Serilog.Formatting.Compact.RenderedCompactJsonFormatter();
			System.Text.StringBuilder stringBuilder = new();
			foreach (LogEvent logEvent in events)
			{
				long initPosition = renderedLogs.Position;
				formatter.Format(logEvent, renderedLogsWriter);
				renderedLogsWriter.Flush();
				renderedLogs.Position = initPosition;
				stringBuilder.Append(renderedLogsReader.ReadLine() + "@NEW");
			}

			return PrepareAndFixJson(stringBuilder.ToString());
		}

		private static string PrepareAndFixJson(string json)
		{
			// https://stackoverflow.com/questions/36759091/c-sharp-checking-if-expression-is-brackets-valid
			bool error = false;
			Stack<char> stack = new();
			char[] jsonChars = json.ToCharArray();
			foreach (char item in jsonChars)
			{
				if (item == '(' || item == '{' || item == '[')
				{
					stack.Push(item);
				}
				else if (item == ')' || item == '}' || item == ']')
				{
					if (stack.Pop() != GetComplementBracket(item))
					{
						error = true;
						break;
					}
				}
			}

			if (error || stack.Count > 0)
			{
				int index = json.LastIndexOf("}@NEW{");
				json = json.Remove(index + 1);
			}

			string fixedJson = json
				.Replace("}@NEW{", "},{")
				.Replace("}@NEW", "}")
				.Replace("@t", "t")
				.Replace("@m", "m")
				.Replace("@i", "id")
				.Replace("@x", "x")
				.Replace("@l", "l");
			return $"[{fixedJson}]";
		}

		private static char GetComplementBracket(char item)
		{
			return item switch
			{
				')' => '(',
				'}' => '{',
				']' => '[',
				_ => ' ',
			};
		}
	}
}
