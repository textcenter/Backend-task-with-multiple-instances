using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TextCenter.BackendTask.MyBackendTask;
namespace TextCenter.NetAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            int BackendEmailInstanceCount = int.Parse(System.Configuration.ConfigurationManager.AppSettings["BackendEmailInstanceCount"]);
            EmailCustomerTask.Start(BackendEmailInstanceCount);
        }
        protected void Application_End()
        {
            EmailCustomerTask.Stop();
        }
    }
}
