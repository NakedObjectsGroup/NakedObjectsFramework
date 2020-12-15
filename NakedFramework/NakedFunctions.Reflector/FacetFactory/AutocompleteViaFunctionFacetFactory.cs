// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFunctions.Meta.Facet;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;
using NakedObjects.Util;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class AutocompleteViaFunctionFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.AutoCompletePrefix
        };

        private readonly ILogger<AutocompleteViaFunctionFacetFactory> logger;

        public AutocompleteViaFunctionFacetFactory(IFacetFactoryOrder<AutocompleteViaFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            logger = loggerFactory.CreateLogger<AutocompleteViaFunctionFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        private static void FindAutoCompleteMethod(Type type, string capitalizedName, Type[] paramTypes, IActionParameterSpecImmutable[] parameters) {
            for (var i = 0; i < paramTypes.Length; i++) {
                // only support on strings and reference types
                var paramType = paramTypes[i];
                if (paramType.IsClass || paramType.IsInterface) {
                    //returning an IQueryable ...
                    //.. or returning a single object
                    var method = FindAutoCompleteMethod(type, capitalizedName, i, typeof(IQueryable<>).MakeGenericType(paramType)) ??
                                 FindAutoCompleteMethod(type, capitalizedName, i, paramType);

                    //... or returning an enumerable of string
                    if (method is null && TypeUtils.IsString(paramType)) {
                        method = FindAutoCompleteMethod(type, capitalizedName, i, typeof(IEnumerable<string>));
                    }

                    if (method is not null) {
                        var pageSizeAttr = method.GetCustomAttribute<NakedObjects.PageSizeAttribute>();
                        var minLengthAttr = (MinLengthAttribute) Attribute.GetCustomAttribute(method.GetParameters().First(), typeof(MinLengthAttribute));

                        var pageSize = pageSizeAttr?.Value ?? 0; // default to 0 ie system default
                        var minLength = minLengthAttr?.Length ?? 0;

                        // add facets directly to parameters, not to actions
                        FacetUtils.AddFacet(new AutoCompleteViaFunctionFacet(method, pageSize, minLength, parameters[i]));
                    }
                }
            }
        }

        private static bool Matches(MethodInfo methodInfo, string name, Type type, Type returnType) =>
            methodInfo.Name == name &&
            methodInfo.DeclaringType == type &&
            methodInfo.ReturnType == returnType;

        private static MethodInfo FindAutoCompleteMethod(Type declaringType, string capitalizedName, int i, Type returnType) {
            var name = RecognisedMethodsAndPrefixes.AutoCompletePrefix + i + capitalizedName;
            return declaringType.GetMethods().SingleOrDefault(methodInfo => Matches(methodInfo, name, declaringType, returnType));
        }

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod,  ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var capitalizedName = NameUtils.CapitalizeName(actionMethod.Name);
            var declaringType = actionMethod.DeclaringType;
            var paramTypes = actionMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            if (action is IActionSpecImmutable actionSpecImmutable) {
                var actionParameters = actionSpecImmutable.Parameters;
                FindAutoCompleteMethod(declaringType, capitalizedName, paramTypes, actionParameters);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name.StartsWith(RecognisedMethodsAndPrefixes.AutoCompletePrefix);

        #endregion
    }
}