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
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public sealed class ServicesManager : IServicesManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServicesManager));
        private readonly IDomainObjectInjector injector;
        private readonly INakedObjectManager manager;

        private readonly List<object> services;

        // cacahe the adapters 
        private INakedObjectAdapter[] serviceAdapters;
        private bool servicesInit;

        public ServicesManager(IDomainObjectInjector injector, INakedObjectManager manager, IReflectorConfiguration config) {
            Assert.AssertNotNull(injector);
            Assert.AssertNotNull(manager);
            Assert.AssertNotNull(config);

            this.injector = injector;
            this.manager = manager;

            services = config.Services.Select(Activator.CreateInstance).ToList();
        }

        private IList<object> Services {
            get {
                if (!servicesInit) {
                    services.ForEach(s => injector.InjectInto(s));
                    servicesInit = true;
                }

                return services;
            }
        }

        #region IServicesManager Members

        public INakedObjectAdapter GetService(string id) {
            return GetServices().FirstOrDefault(no => id.Equals(ServiceUtils.GetId(no.Object)));
        }

        public INakedObjectAdapter GetService(IServiceSpec spec) {
            return GetServices().FirstOrDefault(s => Equals(s.Spec, spec));
        }

        public INakedObjectAdapter[] GetServices() {
            return serviceAdapters ?? (serviceAdapters = Services.Select(service => manager.GetServiceAdapter(service)).ToArray());
        }

        public INakedObjectAdapter[] GetServicesWithVisibleActions(ILifecycleManager lifecycleManager) {
            return GetServices().Where(no => no.Spec.GetActions().Any(a => a.IsVisible(no))).ToArray();
        }

        #endregion
    }
}