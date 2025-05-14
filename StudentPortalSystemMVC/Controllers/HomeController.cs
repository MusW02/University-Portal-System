using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentPortalSystemMVC.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        // When selected from drop down menu
        [HttpPost]
        public ActionResult RedirectUser(string userType)
        {
            switch (userType)
            {
                case "Admin":
                    return RedirectToAction("Login", "Account");
                case "Student":
                case "Teacher":
                    return RedirectToAction("LoginOrRegister", "Account", new { role = userType });
                default:
                    return RedirectToAction("Index");
            }
        }
    }
}