// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Web.Http;
using System.Web.Routing;
using RestfulObjects.Mvc;
using RestfulObjects.Mvc.Media;

namespace NakedObjects.Rest.App.Demo.App_Start {
    public class RestConfig {
        public static void RestPostStart() {

            if (RunWeb.RestRoot != null) {

                //var restDependencyResolver = new RestDependencyResolver();
                //GlobalConfiguration.Configuration.DependencyResolver = restDependencyResolver;

                GlobalConfiguration.Configuration.Formatters.Clear();
                GlobalConfiguration.Configuration.Formatters.Insert(0, new JsonNetFormatter(null));
                //GlobalConfiguration.Configuration.MessageHandlers.Add(new AccessControlExposeHeadersHandler());
                //GlobalConfiguration.Configuration.MessageHandlers.Add(new BasicAuthenticationHandler());
            }
        }

        public static void RestRegisterRoutes(RouteCollection routes) {

            if (RunWeb.RestRoot != null) {

                RestfulObjectsControllerBase.AddRestRoutes(routes, RunWeb.RestRoot);

                // to make whole application 'read only' 
                //RestfulObjectsControllerBase.IsReadOnly = true;

                // to configure domain model options 
                //RestfulObjectsControllerBase.DomainModel = RestControlFlags.DomainModelType.Selectable; //or Simple, Formal, None

                //to enforce concurrency checking
                //RestfulObjectsControllerBase.ConcurrencyChecking = true;

                // to change cache settings (transactional, user, non-expiring) where 0 = no-cache
                // 0, 3600, 86400 are the defaults 
                //RestfulObjectsControllerBase.CacheSettings = new Tuple<int, int, int>(0, 3600, 86400);

                // to set KeySeparator (for multi-part keys) defaults to "-"
                //RestfulObjectsControllerBase.KeySeparator = "-";

                // make Accept header handling non-strict (RO spec 2.4.4)
                //RestfulObjectsControllerBase.AcceptHeaderStrict = false;

                // to change the size limit on returned collections. The default value is 20.  Specifying 0 means 'unlimited'.
                //RestfulObjectsControllerBase.DefaultPageSize = 50; 
            }
        }
    }
}