// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Web.Mvc;
using NakedObjects.Mvc.App;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(NakedObjectsActivator), "PreStart")]
[assembly: PostApplicationStartMethod(typeof(NakedObjectsActivator), "PostStart")]

namespace NakedObjects.Mvc.App {
    public static class NakedObjectsActivator {
        public static void PreStart() {
            InitialiseLogging();
            //RestConfig.RestPreStart();           
        }

        public static void PostStart() {
           
            //var injector = new DotNetDomainObjectContainerInjector(NakedObjectsContext.Reflector, NakedObjectsContext.ObjectPersistor.GetServices().Select(no => no.Object).ToArray());
            //DependencyResolver.SetResolver(new NakedObjectsDependencyResolver(injector));
            //RestConfig.RestPostStart();

            // Without this any value type fields with a default value will be set to mandatory by the MS unobtrusive validation
            // - that overrides the required NOF behaviour based on the 'Optionally' attribute.
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
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