using Microsoft.AspNetCore.Mvc;

namespace WEB_DAU_GIA.Controllers
{
    public class AdminController : Controller
    {
        //Quản trị(AJAX)

        public IActionResult Index()
        {
            return View();
        }
    }
}
