using System.Web.Mvc;

namespace TeamMentor.Website
{
    public class MvcTestController : Controller
    {
        public ActionResult FirstAction()
        {
            ViewBag.HelloMessage = "Hello from the Controller";
            return View(@"~/MVC/Views/Index.cshtml");
        }

        public ActionResult SecondAction()
        {
            ViewBag.HelloMessage = "This is the SecondAction";
            return View(@"~/MVC/Views/Index.cshtml");
        }
    }
}