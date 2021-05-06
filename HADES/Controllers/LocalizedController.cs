using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HADES.Controllers
{
	public class LocalizedController<T> : Controller
	{
		private readonly IStringLocalizer<T> _localizer;
		internal IStringLocalizer Localizer => _localizer;

		public LocalizedController(IStringLocalizer<T> localizer)
		{
			_localizer = localizer;
		}
	}
}
