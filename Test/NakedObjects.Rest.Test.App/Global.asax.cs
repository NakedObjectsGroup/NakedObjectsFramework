using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NakedObjects.Rest.Test.App {
    public class WebApiApplication : System.Web.HttpApplication {
        protected void Application_Start() {

            try {
                GlobalConfiguration.Configure(WebApiConfig.Register);
            }
            catch {
                // temp hack
            }

        }

        protected void Application_PostAuthorizeRequest() {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
    }
}
