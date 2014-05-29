// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Objects.Callbacks;

namespace NakedObjects.Architecture.Util {
    public static class CallbackUtils {
        private static void CallCallback<T>(INakedObject nakedObject) where T : ICallbackFacet {
            if (nakedObject != null && nakedObject.Specification != null) {
                // TODO this is for testing where the adapter or specification may be null 
                // remove when this is no longer true. 
                nakedObject.Specification.GetFacet<T>().Invoke(nakedObject);
            }
        }

        public static void Created(this INakedObject nakedObject) {
            CallCallback<ICreatedCallbackFacet>(nakedObject);
        }

        public static void Deleting(this INakedObject nakedObject) {
            CallCallback<IDeletingCallbackFacet>(nakedObject);
        }

        public static void Deleted(this INakedObject nakedObject) {
            CallCallback<IDeletedCallbackFacet>(nakedObject);
        }

        public static void Loading(this INakedObject nakedObject) {
            CallCallback<ILoadingCallbackFacet>(nakedObject);
        }

        public static void Loaded(this INakedObject nakedObject) {
            CallCallback<ILoadedCallbackFacet>(nakedObject);
        }

        public static void Persisting(this INakedObject nakedObject) {
            CallCallback<IPersistingCallbackFacet>(nakedObject);
        }

        public static void Persisted(this INakedObject nakedObject) {
            CallCallback<IPersistedCallbackFacet>(nakedObject);
        }

        public static void Updating(this INakedObject nakedObject) {
            CallCallback<IUpdatingCallbackFacet>(nakedObject);
        }

        public static void Updated(this INakedObject nakedObject) {
            CallCallback<IUpdatedCallbackFacet>(nakedObject);
        }
    }
}