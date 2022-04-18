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
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Serialization;
using NakedFramework.Metamodel.Utils;

namespace NakedObjects.Reflector.Facet;

[Serializable]
public sealed class ActionChoicesFacetViaMethod : ActionChoicesFacetAbstract, IImperativeFacet {
    private readonly MethodSerializationWrapper choicesMethod;
    private readonly string[] parameterNames;

    public ActionChoicesFacetViaMethod(MethodInfo choicesMethod, (string name, Type type)[] parameterNamesAndTypes, ILogger<ActionChoicesFacetViaMethod> logger, bool isMultiple) {
        this.choicesMethod = new MethodSerializationWrapper(choicesMethod, logger);
        IsMultiple = isMultiple;
        ParameterNamesAndTypes = parameterNamesAndTypes;
        parameterNames = parameterNamesAndTypes.Select(pnt => pnt.name).ToArray();
    }

    public override (string, Type)[] ParameterNamesAndTypes { get; }

    public override bool IsMultiple { get; }

    public override object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedFramework framework) {
        var parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);

        try {
            if (choicesMethod.Invoke<object>(nakedObjectAdapter, parms) is IEnumerable options) {
                return options.Cast<object>().ToArray();
            }

            throw new NakedObjectDomainException($"Must return IEnumerable from choices method: {GetMethod().Name}");
        }
        catch (ArgumentException ae) {
            throw new InvokeException($"Choices exception: {GetMethod().Name} has mismatched (ie type of choices parameter does not match type of action parameter) parameter types", ae);
        }
    }

    #region IImperativeFacet Members

    public MethodInfo GetMethod() => choicesMethod.GetMethod();

    public Func<object, object[], object> GetMethodDelegate() => choicesMethod.GetMethodDelegate();

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.