// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Component {
    /// <summary>
    ///     Maintains a map between domain objects instances (POCOs) and their adaptors (INakedObjectAdapter).
    ///     It also ensures that the same object only ever has one adaptor.
    /// </summary>
    /// <para>
    ///     Each POCO is given an adapter so that the NOF can work with the POCOs even though it does not understand
    ///     their types. Each POCO maps to an adapter and these are reused
    /// </para>
    /// <para>
    ///     Loading of an object refers to the initializing of state within each object as it is restored for
    ///     persistent storage.
    /// </para>
    public interface IIdentityMap : IEnumerable<INakedObjectAdapter> {
        /// <summary>
        ///     Resets the loader to a known state
        /// </summary>
        void Reset();

        void AddAdapter(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     Marks the specified adapter as persistent (as opposed to to being transient) and sets the OID on the
        ///     adapter. The adapter is added to the identity-adapter map.
        /// </summary>
        void MadePersistent(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     Unloads the specified object from both the identity-adapter map, and the poco-adapter map. This
        ///     indicates that the object is no longer in use, and therefore that no objects exists within the system.
        /// </summary>
        void Unloaded(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     Retrieves an existing adapter, from the Poco-adapter map, for the specified object. If the object is
        ///     not in the map then null is returned.
        /// </summary>
        INakedObjectAdapter GetAdapterFor(object domainObject);

        /// <summary>
        ///     Retrieves an existing adapter, from the identity-adapter map, for the specified object. If the OID is
        ///     not in the map then null is returned.
        /// </summary>
        INakedObjectAdapter GetAdapterFor(IOid oid);

        /// <summary>
        ///     Returns true if the object for the specified OID exists, ie it is already loaded
        /// </summary>
        bool IsIdentityKnown(IOid oid);

        /// <summary>
        ///     Marks this object as having been replaced (presumably by a proxy) this allows us to catch
        ///     any incorrect uses of the original object.
        /// </summary>
        void Replaced(object domainObject);

        void UpdateViewModel(INakedObjectAdapter adapter, string[] keys);
    }

    // Copyright (c) Naked Objects Group Ltd.
}