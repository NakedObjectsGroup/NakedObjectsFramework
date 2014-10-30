// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.EntityObjectStore {

    //TODO: Rename to something that describes the nature of this algorithm, which is not
    //specific to EF.  This also suggests it should be moved out of this assembly.
    public class EntityPersistAlgorithm : IPersistAlgorithm {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityPersistAlgorithm));
        private readonly INakedObjectManager manager;
        private readonly IObjectPersistor persistor;

        public EntityPersistAlgorithm(IObjectPersistor persistor, INakedObjectManager manager) {
            this.persistor = persistor;
            this.manager = manager;
        }

        #region IPersistAlgorithm Members

        public string Name {
            get { return "Entity Framework Persist Algorithm"; }
        }

        public void MakePersistent(INakedObject nakedObject, ISession session) {
            if (nakedObject.Spec.IsCollection) {
                MakeCollectionPersistent(nakedObject, session);
            }
            else {
                MakeObjectPersistent(nakedObject);
            }
        }

        #endregion

        public void MakeObjectPersistent(INakedObject nakedObject) {
            if (nakedObject.ResolveState.IsAggregated() ||
                nakedObject.ResolveState.IsPersistent() ||
                nakedObject.Spec.Persistable == PersistableType.Transient ||
                nakedObject.Spec.IsService) {
                return;
            }
            Log.Info("persist " + nakedObject);
            persistor.AddPersistedObject(nakedObject);
        }

        private void MakeCollectionPersistent(INakedObject collection, ISession session) {
            if (collection.ResolveState.IsPersistent() || collection.Spec.Persistable == PersistableType.Transient) {
                return;
            }
            Log.Info("persist " + collection);
            if (collection.ResolveState.IsTransient()) {
                collection.ResolveState.Handle(Events.StartResolvingEvent);
                collection.ResolveState.Handle(Events.EndResolvingEvent);
            }
            manager.MadePersistent(collection);
            collection.GetAsEnumerable(manager).ForEach(no => MakePersistent(no, session));
        }

        public override string ToString() {
            return Name;
        }
    }
}