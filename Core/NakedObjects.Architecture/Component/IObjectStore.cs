// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Component {
    /// <summary>
    /// Provides a generic interface to a specific persistence mechanism
    /// </summary>
    public interface IObjectStore {
        /// <summary>
        ///     Determine if the object store has been initialized with its set of start up objects. If this flag returns
        ///     <c>false</c> the framework will run the fixtures to initialise the object store.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        ///     The name of this object store (for logging/debugging purposes)
        /// </summary>
        string Name { get; }

        void AbortTransaction();

        /// <summary>
        ///     Makes a naked object persistent. The specified object should be stored away via this object store's
        ///     persistence mechanism, and have an new and unique OID assigned to it (by setting the object's
        ///     <see cref="INakedObjectAdapter.Oid" />. The object, should also be added to the cache as the object is
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
        void ExecuteCreateObjectCommand(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     Removes the specified object from the object store. The specified object's data should be removed from
        ///     the persistence mechanism and, if it is cached (which it probably is), removed from the cache also.
        /// </summary>
        void ExecuteDestroyObjectCommand(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     Persists the specified object's state. Essentially the data held by the persistence mechanism should be
        ///     updated to reflect the state of the specified objects. 
        /// </summary>
        void ExecuteSaveObjectCommand(INakedObjectAdapter nakedObjectAdapter);

        void EndTransaction();
        IQueryable<T> GetInstances<T>() where T : class;
        IQueryable GetInstances(Type type);
        IQueryable GetInstances(IObjectSpec spec);
        T CreateInstance<T>(ILifecycleManager lifecycleManager) where T : class;
        object CreateInstance(Type type);

        /// <summary>
        ///     Retrieves the object identified by the specified OID from the object store. The cache should be checked
        ///     first and, if the object is cached, the cached version should be returned. It is important that if this
        ///     method is called again, while the originally returned object is in working memory, then this method
        ///     must return that same object.
        /// </summary>
        /// <para>
        ///     Assuming that the object is not cached then the data for the object should be retrieved from the
        ///     persistence mechanism and the object recreated (as describe previously). The specified OID should then
        ///     be assigned to the recreated object by settings its OID.
        /// </para>
        INakedObjectAdapter GetObject(IOid oid, IObjectSpec hint);

        void Reload(INakedObjectAdapter nakedObjectAdapter);
        void ResolveField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec field);
        void ResolveImmediately(INakedObjectAdapter nakedObjectAdapter);
        void Execute(IPersistenceCommand[] commands);
        void StartTransaction();
        PropertyInfo[] GetKeys(Type type);
        void Refresh(INakedObjectAdapter nakedObjectAdapter);
        int CountField(INakedObjectAdapter nakedObjectAdapter, IAssociationSpec associationSpec);
        INakedObjectAdapter FindByKeys(Type type, object[] keys);
        void LoadComplexTypesIntoNakedObjectFramework(INakedObjectAdapter adapter, bool isGhost);
       
    }

    // Copyright (c) Naked Objects Group Ltd.
}