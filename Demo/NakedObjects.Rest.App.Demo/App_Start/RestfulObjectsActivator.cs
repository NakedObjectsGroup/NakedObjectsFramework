// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Web.Routing;
using RestfulObjects.Mvc.App;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (RestfulObjectsActivator), "PreStart")]
[assembly: PostApplicationStartMethod(typeof (RestfulObjectsActivator), "PostStart")]

namespace RestfulObjects.Mvc.App {
    public static class RestfulObjectsActivator {
        public static void PreStart() {
            RestfulObjectsConfig.RestPreStart();
            RestfulObjectsConfig.RegisterRestfulObjectsRoutes(RouteTable.Routes);
        }

        public static void PostStart() {
            RestfulObjectsConfig.RestPostStart();
        }
    }
}