// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Web.Http;
using System.Web.Routing;
using MvcTestApp.App_Start;
using RestfulObjects.Mvc;
using RestfulObjects.Mvc.Media;
using WebActivator;

[assembly: PreApplicationStartMethod(typeof (RestfulObjectsStart), "PreStart")]
[assembly: PostApplicationStartMethod(typeof (RestfulObjectsStart), "PostStart")]

namespace MvcTestApp.App_Start {
    public static class RestfulObjectsStart {
        public static void PreStart() {
            RegisterRoutes(RouteTable.Routes);
        }

        public static void PostStart() {
       
            var restDependencyResolver = new RestDependencyResolver();
            GlobalConfiguration.Configuration.DependencyResolver = restDependencyResolver;

            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonNetFormatter(null));
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new BasicAuthenticationHandler());
        }

        public static void RegisterRoutes(RouteCollection routes) {
            RestfulObjectsControllerBase.AddRestRoutes(routes);

            // to make whole application 'read only' 
            //RestfulObjectsControllerBase.IsReadOnly = true;

            // to configure domain model options 
            //RestfulObjectsControllerBase.DomainModel = RestControlFlags.DomainModelType.Selectable; //or Simple, Formal, None

            //to enforce concurrency checking
            //RestfulObjectsControllerBase.ConcurrencyChecking = true;

            // to change cache settings (transactional, user, non-expiring) where 0 = no-cache
            // 0, 3600, 86400 are the defaults 
            //RestfulObjectsControllerBase.CacheSettings = new Tuple<int, int, int>(0, 3600, 86400);
        }
    }
}