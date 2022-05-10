// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Configuration;
using NakedFramework.Core.Error;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class ValidateObjectFacet : FacetAbstract, IValidateObjectFacet, IMultipleImperativeFacet {
    public ValidateObjectFacet(IList<MethodInfo> validateMethods, ILogger logger) => ValidateMethods = validateMethods.Select(m => new MethodSerializationWrapper(m, logger, ReflectorDefaults.JitSerialization)).ToList();

    private List<MethodSerializationWrapper> ValidateMethods { get; set; }

    public int Count => ValidateMethods.Count;
    public MethodInfo GetMethod(int index) => ValidateMethods[index].MethodInfo;
    public Func<object, object[], object> GetMethodDelegate(int index) => ValidateMethods[index].MethodDelegate;

    public override Type FacetType => typeof(IValidateObjectFacet);

    private static void LogNoMatch(MethodInfo validator, string actual, ILogger logger) {
        var expects = validator.GetParameters().Select(p => p.Name.ToLower()).ToArray().Aggregate((s, t) => $"{s} {t}");
        logger.LogWarning($"No Matching parms Validator: {validator.Name} Expects {expects} Actual {actual} ");
    }

    private static string[] ParameterNames(MethodSerializationWrapper validator) => validator.MethodInfo.GetParameters().Select(p => p.Name.ToLower()).ToArray();

    #region IValidateObjectFacet Members

    public string Validate(INakedObjectAdapter nakedObjectAdapter, ILogger logger) {
        foreach (var validator in ValidateMethods) {
            var objectSpec = nakedObjectAdapter.Spec as IObjectSpec ?? throw new NakedObjectSystemException("nakedObjectAdapter.Spec must be IObjectSpec");

            var parameterNames = ParameterNames(validator);
            var matches = parameterNames.Select(name => objectSpec.Properties.SingleOrDefault(p => p.Id.ToLower() == name)).Where(s => s != null).ToArray();

            if (matches.Length == parameterNames.Length) {
                var parameters = matches.Select(s => s.GetNakedObject(nakedObjectAdapter)).ToArray();
                if (validator.Invoke<string>(nakedObjectAdapter, parameters) is { } result) {
                    return result;
                }
            }
            else {
                var actual = objectSpec.Properties.Select(s => s.Id).Aggregate((s, t) => $"{s} {t}");
                LogNoMatch(validator.MethodInfo, actual, logger);
            }
        }

        return null;
    }

    public string ValidateParms(INakedObjectAdapter nakedObjectAdapter, (string name, INakedObjectAdapter value)[] parms, ILogger logger) {
        foreach (var validator in ValidateMethods) {
            var parameterNames = ParameterNames(validator);
            var matches = parameterNames.Select(name => parms.SingleOrDefault(p => p.name.ToLower() == name)).Where(p => p != default).ToArray();

            if (matches.Length == parameterNames.Length) {
                var parameters = matches.Select(p => p.value).ToArray();
                if (validator.Invoke<string>(nakedObjectAdapter, parameters) is { } result) {
                    return result;
                }
            }
            else {
                var actual = parms.Select(s => s.name).Aggregate((s, t) => $"{s} {t}");
                LogNoMatch(validator.MethodInfo, actual, logger);
            }
        }

        return null;
    }

    #endregion
}