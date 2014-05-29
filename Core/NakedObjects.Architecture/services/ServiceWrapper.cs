// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Services {
    [Flags]
    public enum ServiceTypes {
        NotAService = 0,
        Menu = 1,
        Contributor = 2,
        System = 4
    }

    public class ServiceWrapper {
        public ServiceWrapper(ServiceTypes serviceType, object service) {
            ServiceType = serviceType;
            Service = service;
        }

        public ServiceTypes ServiceType { get; private set; }
        public object Service { get; private set; }
    }
}