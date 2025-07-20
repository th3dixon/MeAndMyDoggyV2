using Microsoft.AspNetCore.Mvc;

namespace MeAndMyDog.WebApp.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthController : Controller
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            // Handle logout logic
            return RedirectToAction("Index", "Home");
        }
    }
}