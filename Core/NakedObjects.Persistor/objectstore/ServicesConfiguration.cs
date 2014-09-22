using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Services;

namespace NakedObjects.Persistor.Objectstore {
    public class ServicesConfiguration {
        public ServicesConfiguration() {
            Services = new List<ServiceWrapper>();
        }

        public List<ServiceWrapper> Services { get; set; }

        public void AddMenuServices(object[] services) {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceTypes.Menu, s));
            Services.AddRange(ss);
        }

        public void AddContributedActions(object[] services) {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceTypes.Contributor, s));
            Services.AddRange(ss);
        }

        public void AddSystemServices(object[] services) {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceTypes.System, s));
            Services.AddRange(ss);
        }
    }
}