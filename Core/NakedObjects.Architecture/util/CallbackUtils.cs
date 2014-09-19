// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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