// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Core.NakedObjectsSystem;

namespace NakedObjects.Boot {
    public class ServicesInstaller : IServicesInstaller {
        private readonly object[] services;

        public ServicesInstaller(params object[] services) {
            this.services = services;
        }

        #region IServicesInstaller Members

        public object[] GetServices() {
            return Expand(services).ToArray();
        }

        public virtual string Name {
            get { return "ServicesInstaller"; }
        }

        #endregion

        private static List<object> Expand(IEnumerable<object> services) {
            var list = new List<object>();
            foreach (object service in services) {
                if (service is IEnumerable<object>) {
                    list.AddRange(Expand((IEnumerable<object>) service));
                }
                else {
                    list.Add(service);
                }
            }
            return list;
        }
    }
}