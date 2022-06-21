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
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Serialization;
using NakedFramework.Metamodel.Spec;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public sealed class ActionParameterSpecImmutable : Specification, IActionParameterSpecImmutable {
    private TypeSerializationWrapper typeWrapper;

    public ActionParameterSpecImmutable(Type type, IIdentifier identifier) : base(identifier) => typeWrapper = type is null ? null : SerializationFactory.Wrap(type);

    #region IActionParameterSpecImmutable Members

    public Type Type => typeWrapper?.Type;

    public bool IsChoicesEnabled(INakedObjectAdapter adapter, INakedFramework framework) => !GetIsMultipleChoicesEnabled(framework.MetamodelManager.Metamodel) && (GetSpecification(framework.MetamodelManager.Metamodel).IsBoundedSet() || GetFacet<IActionChoicesFacet>()?.IsEnabled(adapter, framework) == true || ContainsFacet<IEnumFacet>());
    public IObjectSpecImmutable GetSpecification(IMetamodel metamodel) => metamodel.GetSpecification(Type) as IObjectSpecImmutable;

    public bool GetIsChoicesDefined(IMetamodel metamodel) => !GetIsMultipleChoicesEnabled(metamodel) && (GetSpecification(metamodel).IsBoundedSet() || ContainsFacet<IActionChoicesFacet>() || ContainsFacet<IEnumFacet>());

    public bool GetIsMultipleChoicesEnabled(IMetamodel metamodel) => ContainsFacet<IActionChoicesFacet>() && GetFacet<IActionChoicesFacet>().IsMultiple;

    #endregion
}