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
    ///     A map of the objects' identities and the adapters' of the objects
    /// </summary>
    public interface IIdentityAdapterMap : IEnumerable<IOid> {
        /// <summary>
        ///     Add an adapter for a given oid
        /// </summary>
        void Add(IOid oid, INakedObjectAdapter adapter);

        /// <summary>
        ///     Get the adapter identified by the specified OID
        /// </summary>
        INakedObjectAdapter GetAdapter(IOid oid);

        /// <summary>
        ///     Determine if an adapter exists for the the specified OID
        /// </summary>
        bool IsIdentityKnown(IOid oid);

        /// <summary>
        ///     Remove the adapter for the given oid
        /// </summary>
        void Remove(IOid oid);

        /// <summary>
        ///     Clear out all mappings
        /// </summary>
        void Reset();
    }

    // Copyright (c) Naked Objects Group Ltd.
}