// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    /// <summary>
    ///     Algorithm to use when the object store will automatically persist all associated objects. Simply adds the single
    ///     object or each
    ///     object in the collection to the store.
    /// </summary>
    public sealed class FlatPersistAlgorithm : IPersistAlgorithm {
        private readonly ILogger<FlatPersistAlgorithm> logger;
        private readonly INakedObjectManager manager;
        private readonly IObjectPersistor persistor;

        public FlatPersistAlgorithm(IObjectPersistor persistor,
                                    INakedObjectManager manager,
                                    ILogger<FlatPersistAlgorithm> logger) {
            this.persistor = persistor;
            this.manager = manager;
            this.logger = logger;
        }

        #region IPersistAlgorithm Members

        public string Name => "Entity Framework Persist Algorithm";

        public void MakePersistent(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Spec.IsCollection) {
                MakeCollectionPersistent(nakedObjectAdapter);
            }
            else {
                MakeObjectPersistent(nakedObjectAdapter);
            }
        }

        #endregion

        private void MakeObjectPersistent(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.ResolveState.IsAggregated() ||
                nakedObjectAdapter.ResolveState.IsPersistent() ||
                nakedObjectAdapter.Spec.Persistable == PersistableType.Transient ||
                nakedObjectAdapter.Spec is IServiceSpec) {
                return;
            }

            persistor.AddPersistedObject(nakedObjectAdapter);
        }

        private void MakeCollectionPersistent(INakedObjectAdapter collection) {
            if (collection.ResolveState.IsPersistent() || collection.Spec.Persistable == PersistableType.Transient) {
                return;
            }

            if (collection.ResolveState.IsTransient()) {
                collection.ResolveState.Handle(Events.StartResolvingEvent);
                collection.ResolveState.Handle(Events.EndResolvingEvent);
            }

            manager.MadePersistent(collection);
            collection.GetAsEnumerable(manager).ForEach(MakePersistent);
        }

        public override string ToString() => Name;
    }
}