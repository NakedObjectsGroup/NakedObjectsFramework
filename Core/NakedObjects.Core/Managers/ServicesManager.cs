using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Service;
using NakedObjects.Persistor.Objectstore;

namespace NakedObjects.Managers {
    public class ServicesManager : IServicesManager {
        private static readonly ILog Log;
        private readonly IContainerInjector injector;
        private readonly INakedObjectManager manager;
        private readonly IServicesConfiguration servicesConfig;
        private readonly List<ServiceWrapper> services = new List<ServiceWrapper>();
        private readonly ISession session;
        private bool servicesInit;

        static ServicesManager() {
            Log = LogManager.GetLogger(typeof (ServicesManager));
        }

        public ServicesManager(IContainerInjector injector, INakedObjectManager manager, IServicesConfiguration servicesConfig, ISession session) {
            this.injector = injector;
            this.manager = manager;
            this.servicesConfig = servicesConfig;
            this.session = session;
            injector.ServiceTypes = servicesConfig.Services.Select(sw => sw.Service.GetType()).ToArray();
        }

        protected virtual List<ServiceWrapper> Services {
            get {
                if (!servicesInit) {
                    AddServices(servicesConfig.Services);
                    services.ForEach(sw => injector.InitDomainObject(sw.Service));
                    servicesInit = true;
                }

                return services;
            }
        }

        #region IServicesManager Members

        public virtual ServiceTypes GetServiceType(INakedObjectSpecification spec) {
            return Services.Where(sw => manager.GetServiceAdapter(sw.Service).Specification == spec).Select(sw => sw.ServiceType).FirstOrDefault();
        }

        public virtual INakedObject GetService(string id) {
            Log.DebugFormat("GetService: {0}", id);
            return (Services.Where(sw => id.Equals(ServiceUtils.GetId(sw.Service))).Select(sw => manager.GetServiceAdapter(sw.Service))).FirstOrDefault();
        }

        public virtual INakedObject[] GetServices() {
            Log.Debug("GetServices");
            return Services.Select(sw => manager.GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType, ILifecycleManager persistor) {
            Log.DebugFormat("GetServicesWithVisibleActions of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                Select(sw => manager.GetServiceAdapter(sw.Service)).
                Where(no => no.Specification.GetAllActions().Any(a => a.IsVisible(session, no, persistor))).ToArray();
        }

        public virtual INakedObject[] GetServices(ServiceTypes serviceType) {
            Log.DebugFormat("GetServices of: {0}", serviceType);
            return Services.Where(sw => (sw.ServiceType & serviceType) != 0).
                Select(sw => manager.GetServiceAdapter(sw.Service)).ToArray();
        }

        public virtual INakedObject[] ServiceAdapters {
            get { return Services.Select(x => manager.CreateAdapter(x.Service, null, null)).ToArray(); }
        }

        #endregion

        private void AddServices(IEnumerable<ServiceWrapper> ss) {
            Log.DebugFormat("AddServices count: {0}", ss.Count());
            services.AddRange(ss);
        }
    }
}