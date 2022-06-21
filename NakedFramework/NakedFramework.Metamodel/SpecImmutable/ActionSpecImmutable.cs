// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public sealed class ActionSpecImmutable : MemberSpecImmutable, IActionSpecImmutable {
    private TypeSerializationWrapper typeWrapper;

    public ActionSpecImmutable(IIdentifier identifier,
                               Type ownerType,
                               IActionParameterSpecImmutable[] parameters)
        : base(identifier) {
        typeWrapper = ownerType is null ? null : SerializationFactory.Wrap(ownerType);
        Parameters = parameters;
    }

    private bool HasReturn() => GetFacet<IActionInvocationFacet>().ReturnType is not null;

    #region IActionSpecImmutable Members

    public override IObjectSpecImmutable GetReturnSpec(IMetamodel metamodel) {
        var type = GetFacet<IActionInvocationFacet>().ReturnType;
        return type is null ? null : metamodel.GetSpecification(type) as IObjectSpecImmutable;
    }

    public ITypeSpecImmutable GetOwnerSpec(IMetamodel metamodel) {
        var type = OwnerType;
        return type is null ? null : metamodel.GetSpecification(type);
    }

    public Type OwnerType => typeWrapper?.Type;

    public IActionParameterSpecImmutable[] Parameters { get; }

    public override IObjectSpecImmutable GetElementSpec(IMetamodel metamodel) {
        var type = GetFacet<IActionInvocationFacet>().ElementType;
        return type is null ? null : metamodel.GetSpecification(type) as IObjectSpecImmutable;
    }

    public bool GetIsFinderMethod(IMetamodel metamodel) =>
        HasReturn() &&
        ContainsFacet(typeof(IFinderActionFacet)) &&
        Parameters.All(p => p.GetSpecification(metamodel).IsParseable || p.GetIsChoicesDefined(metamodel) || p.GetIsMultipleChoicesEnabled(metamodel));

    public bool IsFinderMethodFor(IObjectSpecImmutable spec, IMetamodel metamodel) => GetIsFinderMethod(metamodel) && (GetReturnSpec(metamodel).IsOfType(spec) || (GetReturnSpec(metamodel).IsCollection && GetElementSpec(metamodel).IsOfType(spec)));

    public string StaticName => GetFacet<IMemberNamedFacet>().FriendlyName();

    public bool IsStaticFunction => ContainsFacet<IStaticFunctionFacet>();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.