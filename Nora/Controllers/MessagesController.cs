using Microsoft.AspNetCore.Mvc;

namespace Nora.Controllers
{
	public class MessagesController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
