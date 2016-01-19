using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace NakedObjects.Rest.App.Demo {
    public class WebApiApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(CorsConfig.RegisterCors);

        }

        protected void Application_PostAuthorizeRequest() {
            HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
    }
}
