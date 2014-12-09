// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public class NakedObjectFactory {
        private ILifecycleManager lifecycleManager;
        private IMetamodelManager metamodel;
        private INakedObjectManager nakedObjectManager;
        private IObjectPersistor persistor;
        private ISession session;
        private bool isInitialized;

        public void Initialize(IMetamodelManager metamodel, ISession session, ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectManager nakedObjectManager) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(session);
            Assert.AssertNotNull(lifecycleManager);
            Assert.AssertNotNull(persistor);
            Assert.AssertNotNull(nakedObjectManager);
         
            this.metamodel = metamodel;
            this.session = session;
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.nakedObjectManager = nakedObjectManager;
            isInitialized = true;
        }

        public INakedObject CreateAdapter(object obj, IOid oid) {
            Assert.AssertTrue(isInitialized);
          
            return new PocoAdapter(metamodel, session, persistor, lifecycleManager, nakedObjectManager, obj, oid);
        }
    }
}