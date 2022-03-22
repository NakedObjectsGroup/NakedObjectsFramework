// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public sealed class ActionSpecImmutable : MemberSpecImmutable, IActionSpecImmutable {
    public ActionSpecImmutable(IIdentifier identifier, ITypeSpecImmutable ownerSpec,
                               IActionParameterSpecImmutable[] parameters)
        : base(identifier) {
        OwnerSpec = ownerSpec;
        Parameters = parameters;
    }

    private bool HasReturn() => ReturnSpec != null;

    #region IActionSpecImmutable Members

    public override IObjectSpecImmutable ReturnSpec => GetFacet<IActionInvocationFacet>().ReturnType;

    public ITypeSpecImmutable OwnerSpec { get; }

    public IActionParameterSpecImmutable[] Parameters { get; }

    public override IObjectSpecImmutable ElementSpec => GetFacet<IActionInvocationFacet>().ElementType;

    public bool IsFinderMethod =>
        HasReturn() &&
        ContainsFacet(typeof(IFinderActionFacet)) &&
        Parameters.All(p => p.Specification.IsParseable || p.IsChoicesDefined || p.IsMultipleChoicesEnabled);

    public bool IsFinderMethodFor(IObjectSpecImmutable spec) => IsFinderMethod && (ReturnSpec.IsOfType(spec) || ReturnSpec.IsCollection && ElementSpec.IsOfType(spec));
    public string StaticName => GetFacet<IMemberNamedFacet>().FriendlyName();

    public bool IsContributedMethod => OwnerSpec is IServiceSpecImmutable && Parameters.Any() &&
                                       ContainsFacet(typeof(IContributedActionFacet));

    public bool IsStaticFunction => ContainsFacet<IStaticFunctionFacet>();

    public bool IsContributedToLocalCollectionOf(IObjectSpecImmutable objectSpecImmutable, string id) {
        var memberOrderFacet = GetFacet<IMemberOrderFacet>();
        var directlyContributed = string.Equals(memberOrderFacet?.Name, id, StringComparison.CurrentCultureIgnoreCase);

        return directlyContributed ||
               GetFacet<IContributedToLocalCollectionFacet>()?.IsContributedToLocalCollectionOf(objectSpecImmutable, id) == true;
    }

    #endregion

    #region ISerializable

    // The special constructor is used to deserialize values. 
    public ActionSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
        OwnerSpec = info.GetValue<ITypeSpecImmutable>("specification");
        Parameters = info.GetValue<IActionParameterSpecImmutable[]>("parameters");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue<ISpecification>("specification", OwnerSpec);
        info.AddValue<IActionParameterSpecImmutable[]>("parameters", Parameters);

        base.GetObjectData(info, context);
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.