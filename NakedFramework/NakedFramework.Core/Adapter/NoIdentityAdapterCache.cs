// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;

namespace NakedFramework.Core.Adapter;

public sealed class NoIdentityAdapterCache : INoIdentityAdapterCache {
    private readonly Dictionary<object, INakedObjectAdapter> adapters = new();

    #region INoIdentityAdapterCache Members

    public void AddAdapter(INakedObjectAdapter adapter) => adapters[adapter.Object] = adapter;

    public INakedObjectAdapter GetAdapter(object domainObject) => adapters.ContainsKey(domainObject) ? adapters[domainObject] : null;

    public void Reset() => adapters.Clear();

    #endregion
}