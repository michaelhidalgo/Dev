using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityInnovation.TeamMentor.Website.App_Code
{
    public class MVC_Config
    {
        public static void AppInitialize()
        {            
            AreaRegistration.RegisterAllAreas();            
            RouteTable.Routes.MapRoute("Default", 
                                       "{controller}/{action}", 
                                       new  {action = "FirstAction" });
        }
    }
}