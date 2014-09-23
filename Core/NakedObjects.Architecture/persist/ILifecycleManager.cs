// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Persist {
    public interface ILifecycleManager : INakedObjectTransactionManager, INakedObjectManager, IObjectPersistor, IServicesManager {
        /// <summary>
        ///     Determine if the object store has been initialized with its set of start up objects.
        /// </summary>
        /// <para>
        ///     This method is called only once after the <see cref="IRequiresSetup.Init" /> has been called.
        ///     If this flag returns <c>false</c> the framework will run the fixtures to initialise the persistor.
        /// </para>
        //bool IsInitialized { get; set; }

       
        IOidGenerator OidGenerator { get; }

        /// <summary>
        ///     Primarily for testing
        /// </summary>
        void Reset();

      
      

        /// <summary>
        ///     Re-initialises the fields of an object. If the object is unresolved then the object's missing data
        ///     should be retrieved from the persistence mechanism and be used to set up the value objects and
        ///     associations.
        /// </summary>
        void ResolveImmediately(INakedObject nakedObject);

        void ObjectChanged(INakedObject nakedObject);


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

        void MadePersistent(INakedObject nakedObject);

        void DestroyObject(INakedObject nakedObject);
      

        List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects);
        IOid RestoreGenericOid(string[] encodedData);
        void PopulateViewModelKeys(INakedObject nakedObject);

        object CreateObject(INakedObjectSpecification specification);

    }

    // Copyright (c) Naked Objects Group Ltd.
}