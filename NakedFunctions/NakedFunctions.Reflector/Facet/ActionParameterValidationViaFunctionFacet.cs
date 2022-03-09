// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.ParallelReflector.Utils;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.Facet;

[Serializable]
public sealed class ActionParameterValidationViaFunctionFacet : FacetAbstract, IActionParameterValidationFacet, IImperativeFacet {
    private readonly MethodInfo method;
    private readonly Func<object, object[], object> methodDelegate;

    public ActionParameterValidationViaFunctionFacet(MethodInfo method, ILogger<ActionParameterValidationViaFunctionFacet> logger) {
        this.method = method;
        methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    public override Type FacetType => typeof(IActionParameterValidationFacet);

    protected override string ToStringValues() => $"method={method}";

    #region IActionParameterValidationFacet Members

    public string Invalidates(IInteractionContext ic) => InvalidReason(ic.Target, ic.Framework, ic.ProposedArgument);

    public Exception CreateExceptionFor(IInteractionContext ic) => new ActionArgumentsInvalidException(ic, Invalidates(ic));

    public string InvalidReason(INakedObjectAdapter target, INakedFramework framework, INakedObjectAdapter proposedArgument) =>
        methodDelegate.Invoke<string>(method, method.GetParameterValues(target, proposedArgument, framework));

    #endregion

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => method;

    public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.