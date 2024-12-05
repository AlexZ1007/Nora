using Microsoft.AspNetCore.Mvc;

namespace Nora.Controllers
{
	public class ChannelsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
