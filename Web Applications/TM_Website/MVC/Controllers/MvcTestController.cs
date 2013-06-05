using System.Web.Mvc;

namespace TeamMentor.Website
{
    public class MvcTestController : Controller
    {        
        public ActionResult Transform()
        {
            ViewBag.MarkDownText = 
@"## A title

created in Markdown

* item 1
* item 2
* item 3";
            return View(@"~/MVC/Views/Index.cshtml");
        }
    }
}