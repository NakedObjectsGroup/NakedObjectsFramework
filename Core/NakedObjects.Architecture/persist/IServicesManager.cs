using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.persist {
    public interface IServicesManager {

        INakedObject[] ServiceAdapters { get; }

        INakedObject GetService(string id);

        ServiceTypes GetServiceType(INakedObjectSpecification spec);

        INakedObject[] GetServices();

        INakedObject[] GetServicesWithVisibleActions(ServiceTypes serviceType);

        INakedObject[] GetServices(ServiceTypes serviceType);
    }
}
