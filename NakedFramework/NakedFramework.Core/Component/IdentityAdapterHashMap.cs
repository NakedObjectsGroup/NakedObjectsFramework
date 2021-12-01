// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections;
using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Configuration;

namespace NakedFramework.Core.Component; 

public sealed class IdentityAdapterHashMap : IIdentityAdapterMap {
    private readonly IDictionary<IOid, INakedObjectAdapter> adapters;
    private readonly int capacity;

    private IdentityAdapterHashMap() => adapters = new Dictionary<IOid, INakedObjectAdapter>(capacity);

    // used by DI
    // ReSharper disable once UnusedMember.Global
    public IdentityAdapterHashMap(ICoreConfiguration config) : this() {
        capacity = config.HashMapCapacity;
        adapters = new Dictionary<IOid, INakedObjectAdapter>(capacity);
    }

    #region IIdentityAdapterMap Members

    public void Add(IOid oid, INakedObjectAdapter adapter) => adapters[oid] = adapter;

    public INakedObjectAdapter GetAdapter(IOid oid) => adapters.ContainsKey(oid) ? adapters[oid] : null;

    public bool IsIdentityKnown(IOid oid) => adapters.ContainsKey(oid);

    public IEnumerator<IOid> GetEnumerator() => adapters.Keys.GetEnumerator();

    public void Remove(IOid oid) => adapters.Remove(oid);

    public void Reset() => adapters.Clear();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.