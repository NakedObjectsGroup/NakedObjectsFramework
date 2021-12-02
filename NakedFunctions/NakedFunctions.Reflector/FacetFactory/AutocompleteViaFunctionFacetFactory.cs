// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFunctions.Reflector.Facet;
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class AutocompleteViaFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.AutoCompletePrefix
    };

    private readonly ILogger<AutocompleteViaFunctionFacetFactory> logger;

    public AutocompleteViaFunctionFacetFactory(IFacetFactoryOrder<AutocompleteViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) =>
        logger = loggerFactory.CreateLogger<AutocompleteViaFunctionFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    private void FindAutoCompleteMethodAndAddFacet(Type type, Type targetType, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters) {
        for (var i = 0; i < paramTypes.Length; i++) {
            // only support on strings and reference types
            var paramType = paramTypes[i];
            if (paramType.IsClass || paramType.IsInterface) {
                var name = $"{RecognisedMethodsAndPrefixes.AutoCompletePrefix}{i}{capitalizedName}";
                //returning an IQueryable ...
                //.. or returning a single object
                var method = FindAutoCompleteMethodWithReturnTypes(type, name, paramType, targetType);

                if (method is not null) {
                    var pageSizeAttr = method.GetCustomAttribute<PageSizeAttribute>();
                    var minLengthAttr = InjectUtils.FilterParms(method).FirstOrDefault()?.GetCustomAttribute<MinLengthAttribute>();

                    var pageSize = pageSizeAttr?.Value ?? 0; // default to 0 ie system default
                    var minLength = minLengthAttr?.Value ?? 0;

                    // add facets directly to parameters, not to actions
                    FacetUtils.AddFacet(new AutoCompleteViaFunctionFacet(method, pageSize, minLength, parameters[i], LoggerFactory.CreateLogger<AutoCompleteViaFunctionFacet>()));
                }
            }
        }
    }

    private MethodInfo FindAutoCompleteMethodWithReturnTypes(Type type, string name, Type paramType, Type targetType) {
        // only log by passing in logger the final attempt to find method
        var method = FindAutoCompleteMethod(type, name, typeof(IQueryable<>).MakeGenericType(paramType), targetType) ??
                     FindAutoCompleteMethod(type, name, paramType, targetType, TypeUtils.IsString(paramType) ? null : logger);

        //... or returning an enumerable of string
        if (method is null && TypeUtils.IsString(paramType)) {
            method = FindAutoCompleteMethod(type, name, typeof(IEnumerable<string>), targetType, logger);
        }

        return method;
    }

    private static bool MatchParams(MethodInfo methodInfo) {
        var actualParams = InjectUtils.FilterParms(methodInfo).Select(p => p.ParameterType).ToArray();
        return actualParams.Length == 1 && actualParams.First() == typeof(string);
    }

    private static bool Matches(MethodInfo methodInfo, string name, Type type, Type returnType, Type targetType) =>
        methodInfo.Matches(name, type, returnType, targetType) &&
        MatchParams(methodInfo);

    private static MethodInfo FindAutoCompleteMethod(Type declaringType, string name, Type returnType, Type targetType, ILogger logger = null) {
        bool Matcher(MethodInfo mi) => Matches(mi, name, declaringType, returnType, targetType);
        return FactoryUtils.FindComplementaryMethod(declaringType, name, Matcher, logger);
    }

    #region IMethodFilteringFacetFactory Members

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);
        var declaringType = actionMethod.DeclaringType;
        var targetType = actionMethod.ContributedToType();
        var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();

        if (action is IActionSpecImmutable actionSpecImmutable) {
            var actionParameters = actionSpecImmutable.Parameters;
            FindAutoCompleteMethodAndAddFacet(declaringType, targetType, capitalizedName, paramTypes, actionParameters);
        }

        return metamodel;
    }

    public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.AutoCompletePrefix);

    #endregion
}