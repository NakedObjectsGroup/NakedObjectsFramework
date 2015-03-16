// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Component {
    /// <summary>
    /// Most of this type's responsibilities will be delegated to an injected INakedObjectManager and/or IObjectPersistor.
    /// The primary purpose in having a separate interface ILifecycleManager is so that the caller need not
    /// be concerned with whether the object already exists in memory, persistently, or both. 
    /// </summary>
    public interface ILifecycleManager {
        INakedObjectAdapter CreateInstance(IObjectSpec spec);
        INakedObjectAdapter CreateViewModel(IObjectSpec spec);
        INakedObjectAdapter RecreateInstance(IOid oid, ITypeSpec spec);

        object CreateNonAdaptedInjectedObject(Type type);

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
        void MakePersistent(INakedObjectAdapter nakedObjectAdapter);

        void PopulateViewModelKeys(INakedObjectAdapter nakedObjectAdapter);
        INakedObjectAdapter GetViewModel(IOid oid);
        IOid RestoreOid(string[] encodedData);
        INakedObjectAdapter LoadObject(IOid oid, ITypeSpec spec);
    }

    // Copyright (c) Naked Objects Group Ltd.
}