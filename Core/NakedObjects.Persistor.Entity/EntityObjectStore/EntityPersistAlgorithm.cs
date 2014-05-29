// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Util;
using NakedObjects.Persistor;

namespace NakedObjects.EntityObjectStore {
    public class EntityPersistAlgorithm : IPersistAlgorithm {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityPersistAlgorithm));

        #region IPersistAlgorithm Members

        public void Init() {}

        public void MakePersistent(INakedObject nakedObject, IPersistedObjectAdder persistor) {
            if (nakedObject.Specification.IsCollection) {
                MakeCollectionPersistent(nakedObject, persistor);
            }
            else {
                MakeObjectPersistent(nakedObject, persistor);
            }
        }

        public string Name {
            get { return "Entity Framework Persist Algorithm"; }
        }

        public void Shutdown() {}

        #endregion

        public static void MakeObjectPersistent(INakedObject nakedObject, IPersistedObjectAdder persistor) {
            if (nakedObject.ResolveState.IsAggregated() ||
                nakedObject.ResolveState.IsPersistent() ||
                nakedObject.Specification.Persistable == Persistable.TRANSIENT ||
                nakedObject.Specification.IsService) {
                return;
            }
            Log.Info("persist " + nakedObject);
            persistor.AddPersistedObject(nakedObject);
        }

        private void MakeCollectionPersistent(INakedObject collection, IPersistedObjectAdder persistor) {
            if (collection.ResolveState.IsPersistent() || collection.Specification.Persistable == Persistable.TRANSIENT) {
                return;
            }
            Log.Info("persist " + collection);
            if (collection.ResolveState.IsTransient()) {
                collection.ResolveState.Handle(Events.StartResolvingEvent);
                collection.ResolveState.Handle(Events.EndResolvingEvent);
            }
            persistor.MadePersistent(collection);
            collection.GetAsEnumerable().ForEach(no => MakePersistent(no, persistor));
        }

        public override string ToString() {
            return Name;
        }
    }
}