using System.Collections.Generic;
using NakedObjects.Architecture.Services;

namespace NakedObjects.Persistor.Objectstore {
    public interface IServicesConfiguration {
        void AddMenuServices(params object[] services);
        void AddContributedActions(params object[] services);
        void AddSystemServices(params object[] services);
        List<ServiceWrapper> Services { get; set; }
    }
}