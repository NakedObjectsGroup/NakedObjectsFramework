// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public abstract class CollectionFacetAbstract : FacetAbstract, ICollectionFacet {
    protected CollectionFacetAbstract(bool isASet)
        => IsASet = isASet;

    public override Type FacetType => typeof(ICollectionFacet);

    protected object Call(string name, INakedObjectAdapter collection, params object[] pp) {
        var m = GetType().GetMethod(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static);
        var t = CollectionUtils.GetGenericType(collection.Object.GetType()).GenericTypeArguments.First();

        return m.MakeGenericMethod(t).Invoke(this, pp);
    }

    #region ICollectionFacet Members

    public bool IsASet { get; private set; }

    public abstract bool IsQueryable { get; }
    public abstract bool Contains(INakedObjectAdapter collection, INakedObjectAdapter element);
    public abstract INakedObjectAdapter Page(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable);

    public abstract IEnumerable<INakedObjectAdapter> AsEnumerable(INakedObjectAdapter collection, INakedObjectManager manager);
    public abstract IQueryable AsQueryable(INakedObjectAdapter collection);
    public abstract void Init(INakedObjectAdapter collection, INakedObjectAdapter[] initData);

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.