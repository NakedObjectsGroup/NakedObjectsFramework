// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NakedFramework.Metamodel.Utils;

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class PropertyChoicesFacet : FacetAbstract, IPropertyChoicesFacet, IImperativeFacet {
    private readonly MethodSerializationWrapper methodWrapper;

    private readonly string[] parameterNames;
    private readonly (string name, TypeSerializationWrapper typeWrapper)[] parameterNamesAndTypes;

    public PropertyChoicesFacet(MethodInfo method, (string name, Type type)[] parameterNamesAndTypes, ILogger<PropertyChoicesFacet> logger) {
        methodWrapper = SerializationFactory.Wrap(method, logger);

        this.parameterNamesAndTypes = parameterNamesAndTypes.Select(t => (t.name, SerializationFactory.Wrap(t.type))).ToArray();
        parameterNames = parameterNamesAndTypes.Select(pnt => pnt.name).ToArray();
    }

    public override Type FacetType => typeof(IPropertyChoicesFacet);

    #region IImperativeFacet Members

    /// <summary>
    ///     See <see cref="IImperativeFacet" />
    /// </summary>
    public MethodInfo GetMethod() => methodWrapper.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => methodWrapper.GetMethodDelegate();

    #endregion

    #region IPropertyChoicesFacet Members

    public (string, Type)[] ParameterNamesAndTypes => parameterNamesAndTypes.Select(t => (t.name, t.typeWrapper.Type)).ToArray();

    public bool IsEnabled(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) => true;

    public object[] GetChoices(INakedObjectAdapter inObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedFramework framework) {
        var parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);
        try {
            var options = methodWrapper.Invoke<object>(inObjectAdapter, parms);
            if (options is IEnumerable enumerable) {
                return enumerable.Cast<object>().ToArray();
            }

            throw new NakedObjectDomainException($"Must return IEnumerable from choices method: {GetMethod().Name}");
        }
        catch (ArgumentException ae) {
            throw new InvokeException($"Choices exception: {GetMethod().Name} has mismatched (ie type of parameter does not match type of property) parameter types", ae);
        }
    }

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.