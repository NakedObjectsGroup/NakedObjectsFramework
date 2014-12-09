// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public class ServicesManager : IServicesManager {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ServicesManager));
        private readonly IContainerInjector injector;
        private readonly INakedObjectManager manager;
        private readonly List<IServiceWrapper> services = new List<IServiceWrapper>();
        private readonly IServicesConfiguration servicesConfig;
        private bool servicesInit;

        public ServicesManager(IContainerInjector injector, INakedObjectManager manager, IServicesConfiguration servicesConfig) {
            Assert.AssertNotNull(injector);
            Assert.AssertNotNull(manager);
            Assert.AssertNotNull(servicesConfig);

            this.injector = injector;
            this.manager = manager;
            this.servicesConfig = servicesConfig;
            injector.ServiceTypes = servicesConfig.Services.Select(sw => sw.Service.GetType()).ToArray();
        }

        #region IServicesManager Members

        public virtual ServiceType GetServiceType(IObjectSpec spec) {
            return Services.Where(sw => manager.GetServiceAdapter(sw.Service).Spec == spec).Select(sw => sw.ServiceType).FirstOrDefault();
        }

        public virtual INakedObject GetService(string id) {
            Log.DebugFormat("GetService: {0}", id);
            return (Services.Where(sw => id.Equals(ServiceUtils.GetId(sw.Service))).Select(sw => manager.GetServiceAdapter(sw.Service))).FirstOrDefault();
        }

        public INakedObject GetService(IObjectSpec spec) {
            return GetServices().FirstOrDefault(s => s.Spec == spec);
        }

        public virtual INakedObject[] GetServices() {
            Log.Debug("GetServices");
            return Services.Select(sw => manager.GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] GetServicesWithVisibleActions(ServiceType serviceType, ILifecycleManager persistor) {
            Log.DebugFormat("GetServicesWithVisibleActions of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                Select(sw => manager.GetServiceAdapter(sw.Service)).
                Where(no => no.Spec.GetAllActions().Any(a => a.IsVisible(no))).ToArray();
        }

        public virtual INakedObject[] GetServices(ServiceType serviceType) {
            Log.DebugFormat("GetServices of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                Select(sw => manager.GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] ServiceAdapters {
            get { return Services.Select(x => manager.CreateAdapter(x.Service, null, null)).ToArray(); }
        }

        #endregion

        private List<IServiceWrapper> Services {
            get {
                if (!servicesInit) {
                    AddServices(servicesConfig.Services);
                    services.ForEach(sw => injector.InitDomainObject(sw.Service));
                    servicesInit = true;
                }

                return services;
            }
        }

        private void AddServices(IEnumerable<IServiceWrapper> ss) {
            Log.DebugFormat("AddServices count: {0}", ss.Count());
            services.AddRange(ss);
        }
    }
}