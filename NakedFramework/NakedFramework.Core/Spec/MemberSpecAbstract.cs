// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Interactions;
using NakedFramework.Core.Reflect;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Spec;

public abstract class MemberSpecAbstract : IMemberSpec {
    private readonly IMemberSpecImmutable memberSpecImmutable;

    protected internal MemberSpecAbstract(string id, IMemberSpecImmutable memberSpec, INakedFramework framework) {
        Id = id ?? throw new InitialisationException($"{nameof(id)} is null");
        Framework = framework;
        memberSpecImmutable = memberSpec ?? throw new InitialisationException($"{nameof(memberSpec)} is null");
    }

    public abstract IObjectSpec ElementSpec { get; }

    public override string ToString() => $"id={Id},name='{Name}'";

    protected virtual IConsent GetConsent(string message) => message is null ? Allow.Default : new Veto(message);

    #region IMemberSpec Members

    public virtual string Id { get; }
    protected INakedFramework Framework { get; }

    public virtual IIdentifier Identifier => memberSpecImmutable.Identifier;

    public virtual Type[] FacetTypes => memberSpecImmutable.FacetTypes;

    /// <summary>
    ///     Return the default label for this member. This is based on the name of this member.
    /// </summary>
    /// <seealso cref="Id()" />
    public virtual string Name => memberSpecImmutable.Name;

    public virtual string Description => memberSpecImmutable.Description;

    public abstract IObjectSpec ReturnSpec { get; }

    public virtual bool ContainsFacet(Type facetType) => memberSpecImmutable.ContainsFacet(facetType);

    public virtual bool ContainsFacet<T>() where T : IFacet => memberSpecImmutable.ContainsFacet<T>();

    public virtual IFacet GetFacet(Type type) => memberSpecImmutable.GetFacet(type);

    public virtual T GetFacet<T>() where T : IFacet => memberSpecImmutable.GetFacet<T>();

    public virtual IEnumerable<IFacet> GetFacets() => memberSpecImmutable.GetFacets();

    /// <summary>
    ///     Loops over all <see cref="IHidingInteractionAdvisor" /> <see cref="IFacet" />s and
    ///     returns <c>true</c> only if none hide the member.
    /// </summary>
    public virtual bool IsVisible(INakedObjectAdapter target) {
        IInteractionContext ic = InteractionContext.AccessMember(Framework, false, target, Identifier);
        return InteractionUtils.IsVisible(this, ic);
    }

    public virtual bool IsVisibleWhenPersistent(INakedObjectAdapter target) {
        IInteractionContext ic = InteractionContext.AccessMember(Framework, false, target, Identifier);
        return InteractionUtils.IsVisibleWhenPersistent(this, ic);
    }

    /// <summary>
    ///     Loops over all <see cref="IDisablingInteractionAdvisor" /> <see cref="IFacet" />s and
    ///     returns <c>true</c> only if none disables the member.
    /// </summary>
    public virtual IConsent IsUsable(INakedObjectAdapter target) {
        IInteractionContext ic = InteractionContext.AccessMember(Framework, false, target, Identifier);
        return InteractionUtils.IsUsable(this, ic);
    }

    public bool IsNullable => memberSpecImmutable.ContainsFacet(typeof(INullableFacet));

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.