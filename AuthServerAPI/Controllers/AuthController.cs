using Microsoft.AspNetCore.Mvc;

namespace AuthServerAPI.Controllers
{
	public class AuthController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
