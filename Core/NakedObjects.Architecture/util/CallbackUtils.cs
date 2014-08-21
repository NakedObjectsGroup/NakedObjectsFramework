// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Callbacks;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Util {
    public static class CallbackUtils {
        private static void CallCallback<T>(INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) where T : ICallbackFacet {
            if (nakedObject != null && nakedObject.Specification != null) {
                // TODO this is for testing where the adapter or specification may be null 
                // remove when this is no longer true. 
                nakedObject.Specification.GetFacet<T>().Invoke(nakedObject, session, persistor);
            }
        }

        public static void Created(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<ICreatedCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Deleting(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<IDeletingCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Deleted(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<IDeletedCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Loading(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<ILoadingCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Loaded(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<ILoadedCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Persisting(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<IPersistingCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Persisted(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<IPersistedCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Updating(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<IUpdatingCallbackFacet>(nakedObject, session, persistor);
        }

        public static void Updated(this INakedObject nakedObject, ISession session, INakedObjectPersistor persistor) {
            CallCallback<IUpdatedCallbackFacet>(nakedObject, session, persistor);
        }
    }
}