// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Resolve;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Component {
    public class DefaultPersistAlgorithm : IPersistAlgorithm {
        private static readonly ILog Log = LogManager.GetLogger(typeof (DefaultPersistAlgorithm));
        private readonly INakedObjectManager manager;
        private readonly IObjectPersistor persistor;

        public DefaultPersistAlgorithm(IObjectPersistor persistor, INakedObjectManager manager) {
            Assert.AssertNotNull(persistor);
            Assert.AssertNotNull(manager);

            this.persistor = persistor;
            this.manager = manager;
        }

        #region IPersistAlgorithm Members

        public virtual void MakePersistent(INakedObject nakedObject) {
            if (nakedObject.Spec.IsCollection) {
                Log.Info("Persist " + nakedObject);

                nakedObject.GetAsEnumerable(manager).ForEach(Persist);

                if (nakedObject.ResolveState.IsGhost()) {
                    nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
                    nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
                }
            }
            else {
                if (nakedObject.ResolveState.IsPersistent()) {
                    throw new NotPersistableException("can't make object persistent as it is already persistent: " + nakedObject);
                }
                if (nakedObject.Spec.Persistable == PersistableType.Transient) {
                    throw new NotPersistableException("can't make object persistent as it is not persistable: " + nakedObject);
                }
                Persist(nakedObject);
            }
        }

        public virtual string Name {
            get { return "Simple Bottom Up Persistence Walker"; }
        }

        #endregion

        private void Persist(INakedObject nakedObject) {
            if (nakedObject.ResolveState.IsAggregated() || (nakedObject.ResolveState.IsTransient() && nakedObject.Spec.Persistable != PersistableType.Transient)) {
                IAssociationSpec[] fields = ((IObjectSpec) nakedObject.Spec).Properties;
                if (!nakedObject.Spec.IsEncodeable && fields.Length > 0) {
                    Log.Info("make persistent " + nakedObject);
                    nakedObject.Persisting();
                    if (!nakedObject.Spec.ContainsFacet(typeof (IComplexTypeFacet))) {
                        manager.MadePersistent(nakedObject);
                    }

                    foreach (IAssociationSpec field in fields) {
                        if (!field.IsPersisted) {
                            continue;
                        }
                        if (field is IOneToManyAssociationSpec) {
                            INakedObject collection = field.GetNakedObject(nakedObject);
                            if (collection == null) {
                                throw new NotPersistableException("Collection " + field.Name + " does not exist in " + nakedObject.Spec.FullName);
                            }
                            MakePersistent(collection);
                        }
                        else {
                            INakedObject fieldValue = field.GetNakedObject(nakedObject);
                            if (fieldValue == null) {
                                continue;
                            }
                            Persist(fieldValue);
                        }
                    }
                    persistor.AddPersistedObject(nakedObject);
                }
            }
        }

        public override string ToString() {
            return new AsString(this).ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}