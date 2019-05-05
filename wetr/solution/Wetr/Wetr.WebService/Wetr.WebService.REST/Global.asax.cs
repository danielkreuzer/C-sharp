using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using NSwag.AspNet.Owin;

namespace Wetr.WebService.REST
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            RouteTable.Routes.MapOwinPath("swagger", app =>
            {
                app.UseSwaggerUi3(typeof(WebApiApplication).Assembly, settings => {
                    settings.GeneratorSettings.Title = "Wetr.WebService API";
                    settings.MiddlewareBasePath = "/swagger";
                });
            });
            GlobalConfiguration.Configure(WebApiConfig.Register);

        }
    }
}
