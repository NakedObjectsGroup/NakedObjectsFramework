// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dependencies;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof2.Implementation;
using NakedObjects.Surface.Nof2.Utility;


namespace MvcTestApp.App_Start {
    public class RestDependencyResolver : IDependencyResolver {
        private FrameworkFacade surface;

        public FrameworkFacade Surface {
            get {
                if (surface == null) {
                    surface = new FrameworkFacade(new TestOidStrategy());
                }

                return surface;
            }
        }

        #region IDependencyResolver Members

        public object GetService(Type serviceType) {
            if (typeof(ApiController).IsAssignableFrom(serviceType)) {
                object obj = Activator.CreateInstance(serviceType);
                PropertyInfo surfaceProperty = obj.GetType().GetProperties().SingleOrDefault(p => p.PropertyType == typeof(IFrameworkFacade));
                if (surfaceProperty != null) {
                    surfaceProperty.SetValue(obj, Surface, null);
                }
                return obj;
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return new object[] { };
        }

        public IDependencyScope BeginScope() {
            return this;
        }

        public void Dispose() { }


        #endregion
    }
}