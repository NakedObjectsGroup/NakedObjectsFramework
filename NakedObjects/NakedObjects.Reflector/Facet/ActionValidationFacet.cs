// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Error;
using NakedFramework.Metamodel.Facet;

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class ActionValidationFacet : FacetAbstract, IActionValidationFacet, IImperativeFacet {
    private readonly ILogger<ActionValidationFacet> logger;
    private readonly MethodInfo method;

    [field: NonSerialized] private Func<object, object[], object> methodDelegate;

    public ActionValidationFacet(MethodInfo method, ISpecification holder, ILogger<ActionValidationFacet> logger)
        : base(typeof(IActionValidationFacet), holder) {
        this.method = method;
        this.logger = logger;
        methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    protected override string ToStringValues() => $"method={method}";

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);

    #region IActionValidationFacet Members

    public string Invalidates(IInteractionContext ic) => InvalidReason(ic.Target, ic.Framework, ic.ProposedArguments);

    public Exception CreateExceptionFor(IInteractionContext ic) => new ActionArgumentsInvalidException(ic, Invalidates(ic));

    public string InvalidReason(INakedObjectAdapter target, INakedFramework framework, INakedObjectAdapter[] proposedArguments) {
        if (methodDelegate != null) {
            return (string)methodDelegate(target.GetDomainObject(), proposedArguments.Select(no => no.GetDomainObject()).ToArray());
        }

        //Fall back (e.g. if method has > 6 params) on reflection...
        logger.LogWarning($"Invoking validate method via reflection as no delegate {target}.{method}");
        return (string)InvokeUtils.Invoke(method, target, proposedArguments);
    }

    #endregion

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => method;

    public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.