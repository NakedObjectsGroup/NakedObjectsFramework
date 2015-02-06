// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public class ServicesManager : IServicesManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServicesManager));
        private readonly IContainerInjector injector;
        private readonly INakedObjectManager manager;
        private readonly List<object> services = new List<object>();
        private bool servicesInit;

        public ServicesManager(IContainerInjector injector, INakedObjectManager manager, IReflectorConfiguration config) {
            Assert.AssertNotNull(injector);
            Assert.AssertNotNull(manager);
            Assert.AssertNotNull(config);

            this.injector = injector;
            this.manager = manager;

            services = config.Services.Select(s => Activator.CreateInstance(s)).ToList();
        }

        private IList<object> Services {
            get {
                if (!servicesInit) {
                    services.ForEach(s => injector.InitDomainObject(s));
                    servicesInit = true;
                }

                return services;
            }
        }

        #region IServicesManager Members

        public virtual INakedObject GetService(string id) {
            Log.DebugFormat("GetService: {0}", id);
            return Services.Where(service => id.Equals(ServiceUtils.GetId(service))).Select(service => manager.GetServiceAdapter(service)).FirstOrDefault();
        }

        public INakedObject GetService(IServiceSpec spec) {
            return GetServices().FirstOrDefault(s => Equals(s.Spec, spec));
        }

        public virtual INakedObject[] GetServices() {
            Log.Debug("GetServices");
            return Services.Select(service => manager.GetServiceAdapter(service)).ToArray();
        }

        public virtual INakedObject[] GetServicesWithVisibleActions(ILifecycleManager lifecycleManager) {
            Log.DebugFormat("GetServicesWithVisibleActions");
            return Services.
                Select(service => manager.GetServiceAdapter(service)).
                Where(no => no.Spec.GetObjectActions().Any(a => a.IsVisible(no))).ToArray();
        }
        #endregion
    }
}