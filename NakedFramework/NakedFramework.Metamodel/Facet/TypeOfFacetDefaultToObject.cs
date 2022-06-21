// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class TypeOfFacetDefaultToObject : TypeOfFacetInferredAbstract, ITypeOfFacet {
    private TypeOfFacetDefaultToObject() { }

    public static TypeOfFacetDefaultToObject Instance { get; } = new();

    private static Type DefaultType => typeof(object);

    #region ITypeOfFacet Members

    public override Type GetValue(INakedObjectAdapter collection) => DefaultType;

    public override IObjectSpecImmutable GetValueSpec(INakedObjectAdapter collection, IMetamodel metamodel) => metamodel.GetSpecification(DefaultType) as IObjectSpecImmutable;

    #endregion
}