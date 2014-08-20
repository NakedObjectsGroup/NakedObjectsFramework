// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Web.Mvc {

    public class NakedObjectsDependencyResolver : IDependencyResolver {

        private IContainerInjector injector;

        public NakedObjectsDependencyResolver(IContainerInjector injector) {
            this.injector = injector;
        }

        private IContainerInjector Injector {
            get {
                return injector;
            }
        }

        public object GetService(Type serviceType) {
            // ViewPage = aspx,  WebViewPage = razor
            if (typeof (IController).IsAssignableFrom(serviceType)
                || typeof (ViewPage).IsAssignableFrom(serviceType)
                || typeof (WebViewPage).IsAssignableFrom(serviceType)) {
                object obj = Activator.CreateInstance(serviceType);
                Injector.InitDomainObject(obj);
                return obj;
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return new object[] {};
        }
    }
}
