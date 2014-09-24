// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Util;
using NakedObjects.Persistor;

namespace NakedObjects.EntityObjectStore {
    public class EntityPersistAlgorithm : IPersistAlgorithm {
        private static readonly ILog Log = LogManager.GetLogger(typeof (EntityPersistAlgorithm));

        #region IPersistAlgorithm Members

       

        public void MakePersistent(INakedObject nakedObject, ILifecycleManager persistor, ISession session) {
            if (nakedObject.Specification.IsCollection) {
                MakeCollectionPersistent(nakedObject, persistor, session);
            }
            else {
                MakeObjectPersistent(nakedObject, persistor);
            }
        }

        public string Name {
            get { return "Entity Framework Persist Algorithm"; }
        }

      

        #endregion

        public static void MakeObjectPersistent(INakedObject nakedObject, ILifecycleManager persistor) {
            if (nakedObject.ResolveState.IsAggregated() ||
                nakedObject.ResolveState.IsPersistent() ||
                nakedObject.Specification.Persistable == Persistable.TRANSIENT ||
                nakedObject.Specification.IsService) {
                return;
            }
            Log.Info("persist " + nakedObject);
            persistor.AddPersistedObject(nakedObject);
        }

        private void MakeCollectionPersistent(INakedObject collection, ILifecycleManager persistor, ISession session) {
            if (collection.ResolveState.IsPersistent() || collection.Specification.Persistable == Persistable.TRANSIENT) {
                return;
            }
            Log.Info("persist " + collection);
            if (collection.ResolveState.IsTransient()) {
                collection.ResolveState.Handle(Events.StartResolvingEvent);
                collection.ResolveState.Handle(Events.EndResolvingEvent);
            }
            persistor.MadePersistent(collection);
            collection.GetAsEnumerable(persistor).ForEach(no => MakePersistent(no, persistor, session));
        }

        public override string ToString() {
            return Name;
        }
    }
}