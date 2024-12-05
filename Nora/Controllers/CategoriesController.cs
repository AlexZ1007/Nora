using Microsoft.AspNetCore.Mvc;

namespace Nora.Controllers
{
	public class CategoriesController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
