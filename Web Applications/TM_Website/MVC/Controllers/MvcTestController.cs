using System;
using System.Web.Mvc;

namespace TeamMentor.Website
{
    public class MvcTestController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.HelloMessage = "Hello from the Controller";
            return View(@"~/MVC/Views/Index.cshtml");
        }        
    }
}