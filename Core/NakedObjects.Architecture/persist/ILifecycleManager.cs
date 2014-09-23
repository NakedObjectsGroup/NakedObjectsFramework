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
        bool IsInitialized { get; set; }

       
        IOidGenerator OidGenerator { get; }

        /// <summary>
        ///     Forces a reload of this object from the persistent object store
        /// </summary>
        void Reload(INakedObject nakedObject);

        /// <summary>
        ///     Primarily for testing
        /// </summary>
        void Reset();

       

      

        /// <summary>
        ///     Hint that specified field within the specified object is likely to be needed soon. This allows the
        ///     object's data to be loaded, ready for use.
        /// </summary>
        /// <para>
        ///     This method need not do anything, but offers the object store the opportunity to load in objects before
        ///     their use. Contrast this with <see cref="ResolveImmediately" />, which requires an object to be loaded before
        ///     continuing.
        /// </para>
        /// <seealso cref="ResolveImmediately(INakedObject)" />
        void ResolveField(INakedObject nakedObject, INakedObjectAssociation field);


        // Investigate and remove this if no longer needed 
        void LoadField(INakedObject nakedObject, string fieldName);


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

        void DestroyObject(INakedObject nakedObject);

       

        PropertyInfo[] GetKeys(Type type);

        void Refresh(INakedObject nakedObject);

        int CountField(INakedObject nakedObject, string fieldName);

        INakedObject FindByKeys(Type type, object[] keys);
      

        List<INakedObject> GetCollectionOfAdaptedObjects(IEnumerable domainObjects);
        IOid RestoreGenericOid(string[] encodedData);
        void PopulateViewModelKeys(INakedObject nakedObject);

        object CreateObject(INakedObjectSpecification specification);

    }

    // Copyright (c) Naked Objects Group Ltd.
}