// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Configuration;

namespace NakedObjects.Core.Configuration {
    public class ServicesConfiguration : IServicesConfiguration {
        public ServicesConfiguration() {
            Services = new List<IServiceWrapper>();
        }

        #region IServicesConfiguration Members

        public List<IServiceWrapper> Services { get; set; }

        public void AddMenuServices(params object[] services) {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceType.Menu, s));
            Services.AddRange(ss);
        }

        public void AddContributedActions(params object[] services) {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceType.Contributor, s));
            Services.AddRange(ss);
        }

        public void AddSystemServices(params object[] services) {
            IEnumerable<ServiceWrapper> ss = services.Select(s => new ServiceWrapper(ServiceType.System, s));
            Services.AddRange(ss);
        }

        #endregion
    }
}