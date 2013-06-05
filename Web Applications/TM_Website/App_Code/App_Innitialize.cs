using System.Web.Mvc;
using System.Web.Routing;

namespace SecurityInnovation.TeamMentor.Website.App_Code
{
    public class MVC_Config
    {
        public static void AppInitialize()
        {            
            AreaRegistration.RegisterAllAreas();

            RouteTable.Routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new {controller = "Article", action = "Index", id = UrlParameter.Optional}); // Parameter defaults

        }
    }
}