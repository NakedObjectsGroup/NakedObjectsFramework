using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Util;

namespace NakedObjects.Managers {
    public class NakedObjectFactory {
        private ILifecycleManager persistor;
        private IMetamodelManager metamodel;
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