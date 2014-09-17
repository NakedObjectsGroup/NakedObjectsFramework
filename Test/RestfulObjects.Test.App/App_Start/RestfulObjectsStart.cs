// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Web.Routing;
using MvcTestApp;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (RestfulObjectsStart), "PreStart")]
[assembly: PostApplicationStartMethod(typeof (RestfulObjectsStart), "PostStart")]

namespace MvcTestApp {
    public static class RestfulObjectsStart {
        public static void PreStart() {
            RestConfig.RestPreStart();
            RestConfig.RegisterRestfulObjectsRoutes(RouteTable.Routes);
        }

        public static void PostStart() {
            //RunWeb.Run();
            RestConfig.RestPostStart();
        }
    }
}