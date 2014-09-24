using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Persist {
    public interface IServicesManager {

        INakedObject[] ServiceAdapters { get; }

        INakedObject GetService(string id);

        ServiceTypes GetServiceType(INakedObjectSpecification spec);

        INakedObject[] GetServices();

        INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType, ILifecycleManager persistor);

        INakedObject[] GetServices(ServiceTypes serviceType);
    }
}
