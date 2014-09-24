// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Persist {
    public interface ILifecycleManager : INakedObjectTransactionManager, INakedObjectManager, IObjectPersistor, IServicesManager {
        IOidGenerator OidGenerator { get; }

        INakedObject CreateInstance(INakedObjectSpecification specification);

        INakedObject CreateViewModel(INakedObjectSpecification specification);

        INakedObject RecreateInstance(IOid oid, INakedObjectSpecification specification);

        /// <summary>
        ///     Makes a naked object persistent. The specified object should be stored away via this object store's
        ///     persistence mechanism, and have an new and unique OID assigned to it (by calling the object's
        ///     <c>setOid</c> method). The object, should also be added to the cache as the object is
        ///     implicitly 'in use'.
        /// </summary>
        /// <para>
        ///     If the object has any associations then each of these, where they aren't already persistent, should
        ///     also be made persistent by recursively calling this method.
        /// </para>
        /// <para>
        ///     If the object to be persisted is a collection, then each element of that collection, that is not
        ///     already persistent, should be made persistent by recursively calling this method.
        /// </para>
        void MakePersistent(INakedObject nakedObject);

        List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects);

        IOid RestoreGenericOid(string[] encodedData);

        void PopulateViewModelKeys(INakedObject nakedObject);

        INakedObject GetViewModel(IOid oid);
    }

    // Copyright (c) Naked Objects Group Ltd.
}