// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Services;
using NakedObjects.Core.NakedObjectsSystem;

namespace NakedObjects.Core.Service {
    public static class ServiceUtils {
        public static string GetId(object obj) {
            PropertyInfo m = obj.GetType().GetProperty("Id", typeof (string));
            if (m != null) {
                return (string) m.GetValue(obj, null);
            }
            return obj.GetType().Name;
        }

        public static void AddServices(this INakedObjectPersistor objectPersistor, IServicesInstaller menuServicesInstaller, IServicesInstaller contributedActionsInstaller, IServicesInstaller systemServicesInstaller) {
            objectPersistor.AddServices(menuServicesInstaller != null ? menuServicesInstaller.GetServices().Select(x => new ServiceWrapper(ServiceTypes.Menu, x)) : new ServiceWrapper[] {});
            objectPersistor.AddServices(contributedActionsInstaller != null ? contributedActionsInstaller.GetServices().Select(x => new ServiceWrapper(ServiceTypes.Contributor, x)) : new ServiceWrapper[] {});
            objectPersistor.AddServices(systemServicesInstaller != null ? systemServicesInstaller.GetServices().Select(x => new ServiceWrapper(ServiceTypes.System, x)) : new ServiceWrapper[] {});
        }
    }


    // Copyright (c) Naked Objects Group Ltd.
}