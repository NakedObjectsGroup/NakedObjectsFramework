using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Persist {

    /// <summary>
    /// An injectable framework components that provides access to the domain services that 
    /// have been registered as part of the application. 
    /// </summary>
    public interface IServicesManager {

        //TODO: How does this differ from GetServices -  same return type?
        INakedObject[] ServiceAdapters { get; }

        INakedObject GetService(string id);

        ServiceTypes GetServiceType(INakedObjectSpecification spec);

        INakedObject[] GetServices();

        INakedObject[] GetServices(ServiceTypes serviceType);

        INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType, ILifecycleManager persistor);
    }
}
