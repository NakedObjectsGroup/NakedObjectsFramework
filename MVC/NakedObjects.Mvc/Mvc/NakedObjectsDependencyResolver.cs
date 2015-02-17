// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NakedObjects.Architecture.Component;

namespace NakedObjects.Web.Mvc {
    public class NakedObjectsDependencyResolver : IDependencyResolver {
        private readonly IContainerInjector injector;

        public NakedObjectsDependencyResolver(IContainerInjector injector) {
            this.injector = injector;
        }

        private IContainerInjector Injector {
            get { return injector; }
        }

        public object GetService(Type serviceType) {
            // ViewPage = aspx,  WebViewPage = razor
            if (typeof (IController).IsAssignableFrom(serviceType)
                || typeof (ViewPage).IsAssignableFrom(serviceType)
                || typeof (WebViewPage).IsAssignableFrom(serviceType)
                || typeof (ViewMasterPage).IsAssignableFrom(serviceType)
                || typeof (ViewUserControl).IsAssignableFrom(serviceType)
                ) {
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