// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NakedObjects.Mvc.App;
using NakedObjects.Web.Mvc;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (NakedObjectsStart), "PreStart")]
[assembly: PostApplicationStartMethod(typeof (NakedObjectsStart), "PostStart")]

namespace NakedObjects.Mvc.App {
    public static class NakedObjectsStart {
        public static void PreStart() {
            InitialiseLogging();
            RegisterRoutes(RouteTable.Routes);            
        }

        public static void PostStart() {
            RunWeb.Run();
            DependencyResolver.SetResolver(new NakedObjectsDependencyResolver());
            RestConfig.RestPostStart();

            // Without this any value type fields with a default value will be set to mandatory by the MS unobtrusive validation
            // - that overrides the required NOF behaviour based on the 'Optionally' attribute.
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*nakedobjects}", new { nakedobjects = @"(.*/)?nakedobjects.ico(/.*)?" });

            RestConfig.RestRegisterRoutes(routes); // must be rest first 
            RunMvc.RegisterGenericRoutes(routes);
        }

        public static void InitialiseLogging() {
            // uncomment and add appropriate Common.Logging package
            // http://netcommon.sourceforge.net/docs/2.1.0/reference/html/index.html

            //var properties = new NameValueCollection();
        
            //properties["configType"] = "INLINE";
            //properties["configFile"] = @"C:\Naked Objects\nologfile.txt";

            //LogManager.Adapter = new Common.Logging.Log4Net.Log4NetLoggerFactoryAdapter(properties);

        }
    }
}