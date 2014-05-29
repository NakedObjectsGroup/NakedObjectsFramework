// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;

namespace NakedObjects.Web.Mvc {

    public class NakedObjectsDependencyResolver : IDependencyResolver {

        private IContainerInjector injector;

        private IContainerInjector Injector {
            get {
                if (injector == null) {
                    var servicesToInject = NakedObjectsContext.ObjectPersistor.GetServices().Select(no => no.Object).ToArray();
                    injector = NakedObjectsContext.Reflector.CreateContainerInjector(servicesToInject);
                }

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
