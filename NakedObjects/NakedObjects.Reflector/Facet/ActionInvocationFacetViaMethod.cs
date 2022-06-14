// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;

[assembly: InternalsVisibleTo("NakedFramework.Metamodel.Test")]

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class ActionInvocationFacetViaMethod : ActionInvocationFacetAbstract, IImperativeFacet {
    private readonly TypeSerializationWrapper elementType;
    private readonly MethodSerializationWrapper methodWrapper;
    private readonly TypeSerializationWrapper onType;

    private readonly int paramCount;
    private readonly TypeSerializationWrapper returnType;

    public ActionInvocationFacetViaMethod(MethodInfo method, Type onType, Type returnType, Type elementType, bool isQueryOnly, ILogger<ActionInvocationFacetViaMethod> logger) {
        methodWrapper = SerializationFactory.Wrap(method, logger);

        paramCount = method.GetParameters().Length;
        this.onType = onType is not null ? SerializationFactory.Wrap(onType) : null;
        this.returnType = returnType is not null ? SerializationFactory.Wrap(returnType) : null;
        this.elementType = elementType is not null ? SerializationFactory.Wrap(elementType) : null;
        IsQueryOnly = isQueryOnly;
    }

    public override Type ReturnType => returnType?.Type;

    public override Type OnType => onType?.Type;

    public override Type ElementType => elementType?.Type;

    public override bool IsQueryOnly { get; }

    public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter[] parameters, INakedFramework framework) {
        if (parameters.Length == paramCount) {
            var result = methodWrapper.Invoke<object>(inObjectAdapter, parameters);
            return framework.NakedObjectManager.CreateAdapter(result, null, null);
        }

        throw new NakedObjectSystemException($"{GetMethod()} requires {paramCount} parameters, not {parameters.Length}");
    }

    public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, INakedFramework framework) => Invoke(nakedObjectAdapter, parameters, framework);

    #region IImperativeFacet Members

    /// <summary>
    ///     See <see cref="IImperativeFacet" />
    /// </summary>
    public override MethodInfo GetMethod() => methodWrapper.GetMethod();

    public override Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.