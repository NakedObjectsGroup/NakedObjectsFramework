// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Adapter;
using NakedFramework.Core.Error;
using NakedFramework.Core.Interactions;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Spec;

public sealed class ActionSpec : MemberSpecAbstract, IActionSpec {
    private readonly IActionSpecImmutable actionSpecImmutable;
    private readonly ILogger<ActionSpec> logger;
    private readonly ILoggerFactory loggerFactory;
    private string asString;
    private IObjectSpec elementSpec;
    private bool? hasReturn;
    private bool? isFinderMethod;
    private ITypeSpec onSpec;
    private ITypeSpec ownerSpec;
    private IObjectSpec returnSpec;

    public ActionSpec(INakedFramework framework,
                      SpecFactory memberFactory,
                      IActionSpecImmutable actionSpecImmutable,
                      ILoggerFactory loggerFactory,
                      ILogger<ActionSpec> logger)
        : base(actionSpecImmutable?.Identifier?.MemberName, actionSpecImmutable, framework) {
        this.actionSpecImmutable = actionSpecImmutable ?? throw new InitialisationException($"{nameof(actionSpecImmutable)} is null");
        this.loggerFactory = loggerFactory ?? throw new InitialisationException($"{nameof(loggerFactory)} is null");
        this.logger = logger ?? throw new InitialisationException($"{nameof(logger)} is null");
        var index = 0;
        Parameters = this.actionSpecImmutable.Parameters.Select(pp => memberFactory.CreateParameter(pp, this, index++)).ToArray();
    }

    private IActionInvocationFacet ActionInvocationFacet => actionSpecImmutable.GetFacet<IActionInvocationFacet>();

    private bool FindServiceOnSpecOrSpecSuperclass(ITypeSpec spec) => spec != null && (spec.Equals(OnSpec) || FindServiceOnSpecOrSpecSuperclass(spec.Superclass));

    private INakedObjectAdapter FindService() {
        foreach (var serviceAdapter in Framework.ServicesManager.GetServices()) {
            if (FindServiceOnSpecOrSpecSuperclass(serviceAdapter.Spec)) {
                return serviceAdapter;
            }
        }

        throw new FindObjectException(logger.LogAndReturn($"failed to find service for action {Id}"));
    }

    private IActionParameterSpec GetParameter(int position) {
        if (position >= Parameters.Length) {
            throw new ArgumentException(logger.LogAndReturn($"GetParameter(int): only {Parameters.Length} parameters, position={position}"));
        }

        return Parameters[position];
    }

    public override string ToString() => asString ??= $"Action[{base.ToString()},returns={ReturnSpec},parameters={{{string.Join(",", Parameters.Select(p => p.Spec.ShortName))}}}]";

    #region IActionSpec Members

    public override IObjectSpec ReturnSpec => returnSpec ??= Framework.MetamodelManager.GetSpecification(actionSpecImmutable.GetReturnSpec(Framework.MetamodelManager.Metamodel));

    public override IObjectSpec ElementSpec => elementSpec ??= Framework.MetamodelManager.GetSpecification(actionSpecImmutable.GetElementSpec(Framework.MetamodelManager.Metamodel));

    private ITypeSpec OwnerSpec => ownerSpec ??= Framework.MetamodelManager.GetSpecification(actionSpecImmutable.GetOwnerSpec(Framework.MetamodelManager.Metamodel));

    public ITypeSpec OnSpec => onSpec ??= Framework.MetamodelManager.GetSpecification(ActionInvocationFacet.OnType);

    public override Type[] FacetTypes => actionSpecImmutable.FacetTypes;

    public override IIdentifier Identifier => actionSpecImmutable.Identifier;

    public int ParameterCount => actionSpecImmutable.Parameters.Length;

    public bool IsContributedMethod =>
        OwnerSpec is IServiceSpec &&
        Parameters.Any() &&
        ContainsFacet(typeof(IContributedActionFacet));

    public bool IsStaticFunction => actionSpecImmutable.IsStaticFunction;

    public bool GetIsFinderMethod(IMetamodel metamodel) {
        isFinderMethod ??= actionSpecImmutable.GetIsFinderMethod(metamodel);

        return isFinderMethod.Value;
    }

    public INakedObjectAdapter Execute(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameterSet) {
        var parms = RealParameters(nakedObjectAdapter, parameterSet);
        var target = RealTarget(nakedObjectAdapter);
        var result = ActionInvocationFacet.Invoke(target, parms, Framework);

        if (result is { Oid: null }) {
            result.SetATransientOid(new CollectionMemento(Framework, loggerFactory.CreateLogger<CollectionMemento>(), nakedObjectAdapter, this, parameterSet));
        }

        return result;
    }

    public INakedObjectAdapter RealTarget(INakedObjectAdapter target) =>
        target switch {
            null when IsStaticFunction => null,
            null => FindService(),
            _ when target.Spec is IServiceSpec => target,
            _ when IsContributedMethod => FindService(),
            _ => target
        };

    public override bool ContainsFacet(Type facetType) => actionSpecImmutable.ContainsFacet(facetType);

    public override IFacet GetFacet(Type type) => actionSpecImmutable.GetFacet(type);

    public override IEnumerable<IFacet> GetFacets() => actionSpecImmutable.GetFacets();

    public IActionParameterSpec[] Parameters { get; }

    /// <summary>
    ///     Returns true if the represented action returns something, else returns false
    /// </summary>
    public bool HasReturn {
        get {
            hasReturn ??= ReturnSpec is not null;

            return hasReturn.Value;
        }
    }

    /// <summary>
    ///     Checks declarative constraints, and then checks imperatively.
    /// </summary>
    public IConsent IsParameterSetValid(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameterSet) {
        IInteractionContext ic;
        var buf = new InteractionBuffer();
        if (parameterSet is not null) {
            var parms = RealParameters(nakedObjectAdapter, parameterSet);
            for (var i = 0; i < parms.Length; i++) {
                ic = InteractionContext.ModifyingPropParam(Framework, false, RealTarget(nakedObjectAdapter), Identifier, parameterSet[i]);
                InteractionUtils.IsValid(GetParameter(i), ic, buf);
            }
        }

        var target = RealTarget(nakedObjectAdapter);
        ic = InteractionContext.InvokingAction(Framework, false, target, Identifier, parameterSet);
        InteractionUtils.IsValid(this, ic, buf);
        return InteractionUtils.IsValid(buf);
    }

    public override IConsent IsUsable(INakedObjectAdapter target) {
        IInteractionContext ic = InteractionContext.InvokingAction(Framework, false, RealTarget(target), Identifier, new[] { target });
        return InteractionUtils.IsUsable(this, ic);
    }

    public override bool IsVisible(INakedObjectAdapter target) => base.IsVisible(RealTarget(target));

    public override bool IsVisibleWhenPersistent(INakedObjectAdapter target) => base.IsVisibleWhenPersistent(RealTarget(target));

    public INakedObjectAdapter[] RealParameters(INakedObjectAdapter target, INakedObjectAdapter[] parameterSet) =>
        parameterSet ?? (IsContributedMethod ? new[] { target } : Array.Empty<INakedObjectAdapter>());

    public string GetFinderMethodPrefix() => actionSpecImmutable.GetFacet<IFinderActionFacet>()?.Value;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.