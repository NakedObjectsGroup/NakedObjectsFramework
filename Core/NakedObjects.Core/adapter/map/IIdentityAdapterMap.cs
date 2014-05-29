// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Adapter.Map {
    /// <summary>
    ///     A map of the objects' identities and the adapters' of the objects
    /// </summary>
    public interface IIdentityAdapterMap : IEnumerable<IOid> {
        /// <summary>
        ///     Add an adapter for a given oid
        /// </summary>
        void Add(IOid oid, INakedObject adapter);

        /// <summary>
        ///     Get the adapter identified by the specified OID
        /// </summary>
        INakedObject GetAdapter(IOid oid);

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

        void Shutdown();
    }

    // Copyright (c) Naked Objects Group Ltd.
}