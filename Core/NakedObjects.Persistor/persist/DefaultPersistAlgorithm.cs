// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Util;

namespace NakedObjects.Persistor {
    public class DefaultPersistAlgorithm : IPersistAlgorithm {
        private static readonly ILog Log;

        static DefaultPersistAlgorithm() {
            Log = LogManager.GetLogger(typeof (DefaultPersistAlgorithm));
        }

        #region IPersistAlgorithm Members

        public virtual void Init() {}

        public virtual void MakePersistent(INakedObject nakedObject, INakedObjectPersistor persistor) {
            if (nakedObject.Specification.IsCollection) {
                Log.Info("Persist " + nakedObject);

                nakedObject.GetAsEnumerable(persistor).ForEach(no => Persist(no, persistor));

                if (nakedObject.ResolveState.IsGhost()) {
                    nakedObject.ResolveState.Handle(Events.StartResolvingEvent);
                    nakedObject.ResolveState.Handle(Events.EndResolvingEvent);
                }
            }
            else {
                if (nakedObject.ResolveState.IsPersistent()) {
                    throw new NotPersistableException("can't make object persistent as it is already persistent: " + nakedObject);
                }
                if (nakedObject.Specification.Persistable == Persistable.TRANSIENT) {
                    throw new NotPersistableException("can't make object persistent as it is not persistable: " + nakedObject);
                }
                Persist(nakedObject, persistor);
            }
        }

        public virtual string Name {
            get { return "Simple Bottom Up Persistence Walker"; }
        }

        public virtual void Shutdown() {}

        #endregion

        protected void Persist(INakedObject nakedObject, INakedObjectPersistor persistor) {
            if (nakedObject.ResolveState.IsAggregated() ||
                (nakedObject.ResolveState.IsTransient() &&
                 nakedObject.Specification.Persistable != Persistable.TRANSIENT)) {
                INakedObjectAssociation[] fields = nakedObject.Specification.Properties;
                if (!nakedObject.Specification.IsEncodeable && fields.Length > 0) {
                    Log.Info("make persistent " + nakedObject);
                    nakedObject.Persisting();
                    if (!nakedObject.Specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                        persistor.MadePersistent(nakedObject);
                    }

                    foreach (INakedObjectAssociation field in fields) {
                        if (!field.IsPersisted) {
                            continue;
                        }
                        if (field is IOneToManyAssociation) {
                            INakedObject collection = field.GetNakedObject(nakedObject, persistor);
                            if (collection == null) {
                                throw new NotPersistableException("Collection " + field.Name + " does not exist in " + nakedObject.Specification.FullName);
                            }
                            MakePersistent(collection, persistor);
                        }
                        else {
                            INakedObject fieldValue = field.GetNakedObject(nakedObject, persistor);
                            if (fieldValue == null) {
                                continue;
                            }
                            Persist(fieldValue, persistor);
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