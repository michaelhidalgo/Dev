using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace TeamMentor.CoreLib.TM_AppCode.Utils
{
    public class MVC4
    {
        public MVC4()
        {
            SetUp_ASPNET_MVC();
        }

        public void SetUp_ASPNET_MVC()
        {
            //AreaRegistration.RegisterAllAreas();
            RouteTable.Routes.MapRoute("Default",
                                                   "{controller}/{action}/{id}",
                                                   new { action = "FirstAction", id = UrlParameter.Optional });
        }


    }
}
