using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Template.MVC.Controllers
{
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
