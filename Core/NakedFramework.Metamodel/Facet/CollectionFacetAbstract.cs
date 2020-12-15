// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class CollectionFacetAbstract : FacetAbstract, ICollectionFacet {
        protected CollectionFacetAbstract(ISpecification holder)
            : base(typeof(ICollectionFacet), holder) =>
            IsASet = false;

        protected CollectionFacetAbstract(ISpecification holder, bool isASet)
            : this(holder) =>
            IsASet = isASet;

        #region ICollectionFacet Members

        public bool IsASet { get; private set; }

        public abstract bool IsQueryable { get; }
        public abstract bool Contains(INakedObjectAdapter collection, INakedObjectAdapter element);
        public abstract INakedObjectAdapter Page(int page, int size, INakedObjectAdapter collection, INakedObjectManager manager, bool forceEnumerable);

        public abstract IEnumerable<INakedObjectAdapter> AsEnumerable(INakedObjectAdapter collection, INakedObjectManager manager);
        public abstract IQueryable AsQueryable(INakedObjectAdapter collection);
        public abstract void Init(INakedObjectAdapter collection, INakedObjectAdapter[] initData);

        #endregion

        protected object Call(string name, INakedObjectAdapter collection, params object[] pp) {
            var m = GetType().GetMethod(name);
            var t = collection.Object.GetType().GenericTypeArguments.First();

            return m.MakeGenericMethod(t).Invoke(this, pp);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}