// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class TypeOfFacetInferredFromArray : FacetAbstract, ITypeOfFacet {
        public TypeOfFacetInferredFromArray(ISpecification holder)
            : base(Type, holder) { }

        public static Type Type => typeof(ITypeOfFacet);

        #region ITypeOfFacet Members

        public Type GetValue(INakedObjectAdapter collection) => collection.Object.GetType().GetElementType();

        public IObjectSpecImmutable GetValueSpec(INakedObjectAdapter collection, IMetamodel metamodel) => (IObjectSpecImmutable) metamodel.GetSpecification(GetValue(collection));

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}