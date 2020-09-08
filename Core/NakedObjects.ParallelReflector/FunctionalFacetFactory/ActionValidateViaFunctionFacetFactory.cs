// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFunctions.Meta.Facet;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class ActionValidateViaFunctionFacetFactory : MethodPrefixBasedFacetFactoryAbstract, IMethodFilteringFacetFactory {
        private readonly ILogger<ActionValidateViaFunctionFacetFactory> logger;

        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ValidatePrefix
        };

        public ActionValidateViaFunctionFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.Actions, ReflectionType.Functional) {
            logger = loggerFactory.CreateLogger<ActionValidateViaFunctionFacetFactory>();

        }

        public override string[] Prefixes => FixedPrefixes;

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo actionMethod, IMethodRemover methodRemover, ISpecificationBuilder action, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Type type = actionMethod.GetParameters().FirstOrDefault()?.ParameterType;

            if (type != null) {
                // find matching disable function
                var match = FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods()).Where(m => NameMatches(m, actionMethod)).SingleOrDefault(m => IsSameType(m.GetParameters().FirstOrDefault(), type));

                if (match != null) {
                    var facet = new ActionValidationViaFunctionFacet(match, action);

                    FacetUtils.AddFacet(facet);
                }
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) {
            return method.Name.StartsWith(RecognisedMethodsAndPrefixes.ValidatePrefix);
        }

        #endregion

        private static bool IsSameType(ParameterInfo pi, Type toMatch) {
            return pi != null &&
                   pi.ParameterType == toMatch;
        }

        private static bool NameMatches(MethodInfo compFunction, MethodInfo actionFunction) {
            return compFunction.Name.StartsWith(RecognisedMethodsAndPrefixes.ValidatePrefix)
                   && compFunction.Name.Substring(RecognisedMethodsAndPrefixes.ValidatePrefix.Length) == actionFunction.Name;
        }
    }
}