using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models.API
{
	public class CSPReportRequest
	{
		[JsonProperty(PropertyName = "csp-report", Required = Required.Always)]
		public CSPReport CSPReport { get; set; }

		public override string ToString()
		{
			return CSPReport.ToString();
		}
	}

	public class CSPReport
	{
		[JsonProperty(PropertyName = "blocked-uri", Required = Required.Always)]
		public string BlockedUri { get; set; }

		[JsonProperty(PropertyName = "disposition")]
		public string Disposition { get; set; }

		[JsonProperty(PropertyName = "document-uri", Required = Required.Always)]
		public string DocumentUri { get; set; }

		[JsonProperty(PropertyName = "effective-directive")]
		public string EffectiveDirective { get; set; }

		[JsonProperty(PropertyName = "original-policy")]
		public string OriginalPolicy { get; set; }

		[JsonProperty(PropertyName = "referrer")]
		public string Referrer { get; set; }

		[JsonProperty(PropertyName = "script-sample", NullValueHandling = NullValueHandling.Include)]
		public string ScriptSample { get; set; }

		[JsonProperty(PropertyName = "status-code")]
		public string StatusCode { get; set; }

		[JsonProperty(PropertyName = "violated-directive", Required = Required.Always)]
		public string ViolatedDirective { get; set; }

		public override string ToString()
		{
			return $"Content Security Policy Violation: {{ Blocked Uri \"{BlockedUri}\" in page \"{DocumentUri}\" because of \"{ViolatedDirective}\"}}";
		}
	}
}
