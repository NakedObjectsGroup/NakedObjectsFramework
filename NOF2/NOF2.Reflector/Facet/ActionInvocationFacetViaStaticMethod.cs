// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NOF2.Container;
using NOF2.Reflector.Helpers;

namespace NOF2.Reflector.Facet;

[Serializable]
public sealed class ActionInvocationFacetViaStaticMethod : ActionInvocationFacetAbstract, IImperativeFacet {
    private readonly TypeSerializationWrapper elementType;
    private readonly bool injected;
    private readonly MethodSerializationWrapper methodWrapper;
    private readonly TypeSerializationWrapper onType;
    private readonly int paramCount;
    private readonly TypeSerializationWrapper returnType;

    public ActionInvocationFacetViaStaticMethod(MethodInfo method,
                                                Type onType,
                                                Type returnType,
                                                Type elementType,
                                                bool isQueryOnly,
                                                ILogger<ActionInvocationFacetViaStaticMethod> logger) {
        methodWrapper = SerializationFactory.Wrap(method, logger);
        (injected, paramCount) = ParameterCount(method);
        this.onType = onType is not null ? SerializationFactory.Wrap(onType) : null;
        this.returnType = returnType is not null ? SerializationFactory.Wrap(returnType) : null;
        this.elementType = elementType is not null ? SerializationFactory.Wrap(elementType) : null;
        IsQueryOnly = isQueryOnly;
    }

    public override Type ReturnType => returnType?.Type;

    public override Type OnType => onType?.Type;

    public override Type ElementType => elementType?.Type;

    public override bool IsQueryOnly { get; }

    private static (bool, int) ParameterCount(MethodInfo method) {
        var parameters = method.GetParameters();
        var count = parameters.Length;
        var injected = parameters.Count(p => p.ParameterType.IsAssignableTo(typeof(IContainer)));
        if (injected > 1) {
            throw new ReflectionException($"Cannot inject more than one container into {method}");
        }

        return (injected == 1, count - injected);
    }

    private INakedObjectAdapter HandleInvokeResult(INakedFramework framework, object result) =>
        // if any changes made by invocation fail 
        framework.NakedObjectManager.CreateAdapter(result, null, null);

    public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter,
                                               INakedObjectAdapter[] parameters,
                                               INakedFramework framework) {
        if (parameters.Length != paramCount) {
            throw new NakedObjectSystemException($"{GetMethod()} requires {paramCount} parameters, not {parameters.Length}");
        }

        var rawParms = parameters.Select(p => p?.Object).ToArray();
        if (injected) {
            rawParms = rawParms.Append(null).ToArray();
        }

        var substituteParms = NOF2Helpers.SubstituteNullsAndContainer(rawParms, GetMethod(), framework);

        return HandleInvokeResult(framework, GetMethodDelegate().Invoke<object>(GetMethod(), null, substituteParms));
    }

    public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter,
                                               INakedObjectAdapter[] parameters,
                                               int resultPage,
                                               INakedFramework framework) =>
        Invoke(nakedObjectAdapter, parameters, framework);

    #region IImperativeFacet Members

    /// <summary>
    ///     See <see cref="IImperativeFacet" />
    /// </summary>
    public override MethodInfo GetMethod() => methodWrapper.GetMethod();

    public override Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.