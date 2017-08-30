using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WildeRoverMgmtApp.Models;

namespace WildeRoverMgmtApp.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Home/Index", "WildeRoverItems");
            }

            LoginViewModel model = new LoginViewModel();
            model.UserName = string.Empty;
            model.Password = string.Empty;

            return View(model);
        }
    }
}
