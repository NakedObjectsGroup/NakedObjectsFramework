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
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class EditAnnotationFacetFactory : ObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        private readonly ILogger<EditAnnotationFacetFactory> logger;

        public EditAnnotationFacetFactory(IFacetFactoryOrder<EditAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ActionsAndActionParameters) =>
            logger = loggerFactory.CreateLogger<EditAnnotationFacetFactory>();

        private static bool Matches(ParameterInfo pri, PropertyInfo ppi) =>
            string.Equals(pri.Name, ppi.Name, StringComparison.CurrentCultureIgnoreCase) &&
            pri.ParameterType == ppi.PropertyType;

        private static IDictionary<ParameterInfo, PropertyInfo> MatchParmsAndProperties(MethodInfo method) {
            var toMatchParms = method.GetParameters();

            if (toMatchParms.Any()) {
                var allProperties = method.ReturnType.GetProperties();

                var matchedProperties = allProperties.Where(p => toMatchParms.Any(tmp => Matches(tmp, p))).ToArray();
                var matchedParameters = toMatchParms.Where(tmp => allProperties.Any(p => Matches(tmp, p))).ToArray();

                return matchedParameters.ToDictionary(p => p, p => matchedProperties.Single(mp => Matches(p, mp)));
            }

            return new Dictionary<ParameterInfo, PropertyInfo>();
        }

        private static IImmutableDictionary<string, ITypeSpecBuilder> Process(MethodInfo method, Action<IDictionary<ParameterInfo, PropertyInfo>> addFacet, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (IsEditMethod(method)) {
                var matches = MatchParmsAndProperties(method);

                if (matches.Any()) {
                    addFacet(matches);
                }
            }

            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            void AddFacet(IDictionary<ParameterInfo, PropertyInfo> matches) => FacetUtils.AddFacet(new EditPropertiesFacet(specification, matches.Values.Select(p => p.Name).ToArray()));

            return Process(method, AddFacet, metamodel);
        }

        private static bool IsEditMethod(MethodInfo method) => method.IsDefined(typeof(EditAttribute), false) && method.DeclaringType == method.ReturnType;

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            void AddFacet(IDictionary<ParameterInfo, PropertyInfo> matches) {
                var thisParameter = method.GetParameters()[paramNum];

                if (matches.ContainsKey(thisParameter)) {
                    var property = matches[thisParameter];
                    // leave any existing default facet
                    if (specification.ContainsFacet<IActionDefaultsFacet>()) {
                        logger.LogWarning($"Edit default not added to {thisParameter} on {method} as has explicit default method");
                    }
                    else {
                        FacetUtils.AddFacet(new ActionDefaultsFacetViaProperty(property, specification, LoggerFactory.CreateLogger<ActionDefaultsFacetViaProperty>()));
                    }
                }
            }

            return Process(method, AddFacet, metamodel);
        }
    }
}