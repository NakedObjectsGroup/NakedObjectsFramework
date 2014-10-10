// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Managers {
    public class NakedObjectFactory {
        private IMetamodelManager metamodel;
        private ILifecycleManager persistor;
        private ISession session;

        public void Initialize(IMetamodelManager metamodel, ISession session, ILifecycleManager persistor) {
            this.metamodel = metamodel;
            this.session = session;
            this.persistor = persistor;
        }

        public INakedObject CreateAdapter(object obj, IOid oid) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(session);
            Assert.AssertNotNull(persistor);

            return new PocoAdapter(metamodel, session, persistor, persistor, obj, oid);
        }
    }
}