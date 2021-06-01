using CsvHelper;
using HADES.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HADES.Controllers
{
	[Authorize]
    public class EventLogController : LocalizedController<EventLogController>
    {
        public EventLogController(IStringLocalizer<EventLogController> localizer) : base(localizer) { }


        public IActionResult EventLog()
        {
            if (!Util.ConnexionUtil.CurrentUser(User).GetRole().EventLogAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            string jQueryUILanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLower() switch
            {
                "fr-ca" => "fr-CA",
                "pt-br" => "pt-BR",
                "es-us" => "es",
                _ => "",
            };
            ViewData.Add("JQueryUILanguage", jQueryUILanguage);
            ViewData.Add("MinDate", DateTime.UtcNow.AddDays(new Services.AppConfigService().GetLogDeleteFrequency() / -1).AddMonths(-1)); // JS Dates start months at 0 (January is 0, February is 1, [...])

            string dataTablesLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToLower() switch
            {
                "fr-ca" => "/lib/datatables/i18n/French.json",
                "pt-br" => "/lib/datatables/i18n/Portuguese-Brasil.json",
                "es-us" => "/lib/datatables/i18n/Spanish.json",
                _ => "/lib/datatables/i18n/English.json",
            };
            ViewData.Add("DataTablesLanguage", dataTablesLanguage);

            return View();
        }

        [HttpPost]
        [Consumes("application/x-www-form-urlencoded; charset=UTF-8")]
        public IActionResult Data([FromForm] Models.EventLogDataRequest request)
        {
            if (!Util.ConnexionUtil.CurrentUser(User).GetRole().EventLogAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            Models.EventLogDataResponse response = new(request.DrawCall);
            if (request.RecordsCount != -1 && DateTime.TryParse(request.Date, out DateTime requestDate) && requestDate > DateTime.MinValue) // Valid data
            {
                string logPath = $"Logs/log{requestDate:yyyyMMdd}.json";
                if (System.IO.File.Exists(logPath))
                {
                    // Check with local date, Serilog uses local time to roll logs. :|
                    bool isTodaysLog = DateTime.Now.Date == requestDate.Date;
                    Exception ex = null;
                    List<Exception> readingExceptions = null;
                    try
                    {
                        if (isTodaysLog)
                        {
                            Serilog.Log.CloseAndFlush();
                        }

                        if (EventLogCountCache.CacheDate != requestDate.Date)
                        {
                            EventLogCountCache.Reset();
                        }

                        IEnumerable<string> file;
                        int totalRecords;
                        if (!EventLogCountCache.TotalRecordsCached) // Checks for TotalRecords != null
                        {
                            file = ReadEvents(logPath, out totalRecords);
                        }
                        else // Safe to use TotalRecords here
                        {
                            file = ReadEvents(logPath);
                            totalRecords = EventLogCountCache.TotalRecords.Value;
                        }

                        // Order by here, if made possible one day

                        int filteredRecords;
                        if (EventLogCountCache.TryGetFilteredRecordsCount(request.Search, out filteredRecords))
                        {
                            file = file.FilterEvents(request.Search);
                        }
                        else
                        {
                            file = file.FilterEvents(request.Search, out filteredRecords);
                        }

                        response.Data = file
                            .PageEvents(request.StartAtRecord, request.RecordsCount)
                            .ToLogEvents(out readingExceptions)
                            .RenderEvents();
                        response.RecordsTotal = totalRecords;
                        response.RecordsFiltered = filteredRecords;
                        response.NumberOfExceptions = readingExceptions.Count;

                        EventLogCountCache.CacheData(requestDate.Date, request.Search, filteredRecords, totalRecords);
                    }
                    catch (Exception e)
                    {
                        response.Error = Localizer["LogError"];
                        ex = e;
                    }
                    finally
                    {
                        if (isTodaysLog)
                        {
                            Util.LogManager.RefreshLogger(new Services.AppConfigService().GetAppConfig());
                        }

                        if (ex != null)
                        {
                            Serilog.Log.Error(ex, "An exception occured while {Action}", "processing the event log's data for DataTables");
                        }

                        if (readingExceptions != null)
						{
                            foreach (Exception e in readingExceptions)
							{
                                Serilog.Log.Error(e, "An exception occured while {Action}", "processing one JSON line from a log file for DataTables");
                            }
						}
                    }
                }
                else
                {
                    response.Error = Strings.NoLog;
                }
            }
            else
            {
                response.Error = Localizer["BadRequest"];
            }

            return Json(response);
        }

        [HttpPost]
        public IActionResult Refresh()
        {
            if (!Util.ConnexionUtil.CurrentUser(User).GetRole().EventLogAccess) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            EventLogCountCache.Reset();
            return new OkObjectResult("OK");
        }

        [HttpPost]
        public IActionResult CSV([FromForm] Models.EventLogDataRequest request)
        {
            Models.IUser user = Util.ConnexionUtil.CurrentUser(User);
            if (!user?.GetRole().EventLogAccess ?? true) // ACCESS CONTROL
            {
                return RedirectToAction("MainView", "Home");
            }

            IActionResult result = null;
            if (request != null && request.RecordsCount != -1 && DateTime.TryParse(request.Date, out DateTime requestDate) && requestDate > DateTime.MinValue) // Valid data
            {
                string logPath = $"Logs/log{requestDate:yyyyMMdd}.json";
                if (System.IO.File.Exists(logPath))
                {
                    Serilog.Log.Information("Data from log{Date}.json is being exported to a CSV file", requestDate.ToString("yyyyMMdd"));

                    // Check with local date, Serilog uses local time to roll logs. :|
                    bool isTodaysLog = DateTime.Now.Date == requestDate.Date;
                    Exception ex = null;
                    List<Exception> readingExceptions = null;
                    try
                    {
                        if (isTodaysLog)
                        {
                            Serilog.Log.CloseAndFlush();
                        }
                        if (!Directory.Exists("Temp"))
                        {
                            Directory.CreateDirectory("Temp");
                        }

                        IEnumerable<string> logFileEnumerable = ReadEvents(logPath, out int totalRecords)
                            .FilterEvents(request.Search, out int filteredRecords);

                        // Order by here, if made possible one day
#if DEBUG
                        System.Diagnostics.Stopwatch stopWatch = new();
                        stopWatch.Start();
#endif
                        string csvPath = $"Temp/{Guid.NewGuid()}.csv";
                        using (FileStream file = System.IO.File.Create(csvPath))
                        using (BufferedStream fileBuffer = new(file))
                        using (StreamWriter writer = new(fileBuffer, System.Text.Encoding.UTF8))
                        using (CsvWriter csv = new(writer, System.Globalization.CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(
                                logFileEnumerable
                                    .ToLogEvents(out readingExceptions)
                                    .ToEventLogCSVs()
                            );
                        }
#if DEBUG
                        stopWatch.Stop();
                        TimeSpan ts = stopWatch.Elapsed;
                        string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                            ts.Hours, ts.Minutes, ts.Seconds,
                            ts.Milliseconds / 10);
                        Console.WriteLine("CSV Creation took " + elapsedTime);
#endif
                        Guid fileId = Util.FileManager.Add(new Models.AvailableFile(csvPath, $"Log{requestDate:yyyy-MM-dd}.csv", "text/csv", user.GetId()));
                        // Encrypt -> Turn to bytes -> Encode into base64 string -> Escape for uri
                        string encryptedGuid = Uri.EscapeDataString(
                            Convert.ToBase64String(
								System.Text.Encoding.UTF8.GetBytes(
                                    Util.EncryptionUtil.Encrypt(fileId.ToString()).ToCharArray()
                                )
                            )
                        );
                        result = Json($"{{\"id\":\"{encryptedGuid}\", \"exceptionCount\":\"{readingExceptions.Count}\"}}");
                    }
                    catch (Exception e)
                    {
                        result = StatusCode(StatusCodes.Status500InternalServerError);
                        ex = e;
                    }
                    finally
                    {
                        if (isTodaysLog)
                        {
                            Util.LogManager.RefreshLogger(new Services.AppConfigService().GetAppConfig());
                        }

                        if (ex != null)
                        {
                            Serilog.Log.Error(ex, "An exception occured while {Action}", "converting the log to CSV");
                        }

                        if (readingExceptions != null)
                        {
                            foreach (Exception e in readingExceptions)
                            {
                                Serilog.Log.Error(e, "An exception occured while {Action}", "processing one JSON line from a log file for DataTables");
                            }
                        }
                    }
                }
                else
                {
                    result = NotFound();
                }
            }
            else
            {
                result = BadRequest();
            }

            return result;
        }

        private static IEnumerable<string> ReadEvents(string logPath, out int totalRecords)
        {
            IEnumerable<string> file = System.IO.File.ReadLines(logPath, System.Text.Encoding.UTF8);

            totalRecords = file.Count();
            return file;
        }

        private static IEnumerable<string> ReadEvents(string logPath)
        {
            return System.IO.File.ReadLines(logPath, System.Text.Encoding.UTF8);
        }

        // Should be object-oriented and keep instances as cache instead of having a singular cache at a time, oh well. To refactor if there's time.
        private static class EventLogCountCache
        {
            private static Dictionary<string, int> SearchFilteredRecords { get; } = new();

            public static DateTime? CacheDate { get; private set; }
            public static int? TotalRecords { get; private set; }
            public static bool TotalRecordsCached => TotalRecords.HasValue;
            public static bool TryGetFilteredRecordsCount(string search, out int filteredRecords)
            {
                if (SearchFilteredRecords.ContainsKey(search.ToLower()))
                {
                    filteredRecords = SearchFilteredRecords[search.ToLower()];
                    return true;
                }
                else
                {
                    filteredRecords = -1;
                    return false;
                }
            }

            public static void CacheData(DateTime date, string search, int filteredRecords, int totalRecords)
            {
                if (!CacheDate.HasValue)
                {
                    CacheDate = date;
                }
                else if (CacheDate.Value != date)
                {
                    CacheDate = date;
                    Reset();
                }

                if (!TotalRecords.HasValue)
                {
                    TotalRecords = totalRecords;
                }

                if (!SearchFilteredRecords.ContainsKey(search.ToLower()))
                {
                    SearchFilteredRecords.Add(search.ToLower(), filteredRecords);
                }
            }

            public static void Reset()
            {
                TotalRecords = null;
                SearchFilteredRecords.Clear();
            }
        }
    }
}
