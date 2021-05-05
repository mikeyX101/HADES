using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HADES.Models.ViewModels
{
	public class HttpStatusErrorViewModel
	{
		public string PageTitle { get; set; }
		public string StatusNumber { get; set; }
		public string StatusName { get; set; }
		public string StatusDescription { get; set; }
	}
}
