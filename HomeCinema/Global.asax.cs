﻿using HomeCinema.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HomeCinema
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            AreaRegistration.RegisterAllAreas();
           // WebApiConfig.Register(config);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Bootstrapper.Run();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            GlobalConfiguration.Configuration.EnsureInitialized();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
