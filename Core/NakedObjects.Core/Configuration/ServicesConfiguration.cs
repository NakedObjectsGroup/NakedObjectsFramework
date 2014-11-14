using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Configuration;

namespace NakedObjects.Core.Configuration {
    public class ServicesConfiguration : IServicesConfiguration {
        public ServicesConfiguration() {
            Services = new List<IServiceWrapper>();
        }

        public List<IServiceWrapper> Services { get; set; }

        public void AddMenuServices(params object[] services) {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceType.Menu, s));
            Services.AddRange(ss);
        }

        public void AddContributedActions(params object[] services)
        {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceType.Contributor, s));
            Services.AddRange(ss);
        }

        public void AddSystemServices(params object[] services)
        {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceType.System, s));
            Services.AddRange(ss);
        }
    }
}