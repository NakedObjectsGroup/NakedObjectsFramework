// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using NakedObjects.App.Demo;
using NakedObjects.App.Demo.App_Start;
using NakedObjects.Mvc.App;
using NakedObjects.Web.Mvc;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (NakedObjectsStart), "PreStart")]
[assembly: PostApplicationStartMethod(typeof (NakedObjectsStart), "PostStart")]

namespace NakedObjects.App.Demo {
    public static class NakedObjectsStart {
        public static void PreStart() {
            InitialiseLogging();
            RegisterRoutes(RouteTable.Routes);            
        }

        public static void PostStart() {
            RegisterBundles(BundleTable.Bundles);

            RunWeb.Run();
            DependencyResolver.SetResolver(new NakedObjectsDependencyResolver());
            //RestConfig.RestPostStart();
        }

        public static void RegisterRoutes(RouteCollection routes) {
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*nakedobjects}", new { nakedobjects = @"(.*/)?nakedobjects.ico(/.*)?" });

            //RestConfig.RestRegisterRoutes(routes); // must be rest first 
            RunMvc.RegisterGenericRoutes(routes);
        }

        // this may be moved to BundleConfig as required - here just to simplify Nuget install  
        public static void RegisterBundles(BundleCollection bundles) {

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.js",
                "~/Scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.unobtrusive-ajax.js"));

            // register Naked Objects bundles  

            bundles.Add(new ScriptBundle("~/bundles/nakedobjectsajax").Include(
                "~/Scripts/jquery.address-{version}.js",
                "~/Scripts/jquery.json-{version}.js",
                "~/Scripts/jstorage*",
                "~/Scripts/NakedObjects-Ajax*"));

            bundles.Add(new ScriptBundle("~/bundles/jquerydatepicker").Include(
                "~/Scripts/ui/i18n/jquery.ui.datepicker-en-GB*"));

            bundles.Add(new StyleBundle("~/Content/nakedobjectscss").Include(
                "~/Content/NakedObjects.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/bootstrap.css",
                    "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

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