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

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class EditAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        public EditAnnotationFacetFactory(IFacetFactoryOrder<HiddenAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) { }

        private static bool IsContext(Type t) => t.IsAssignableTo(typeof(IContext));

        private static bool ReturnsContext(MethodInfo method) => IsContext(method.ReturnType) || FacetUtils.IsTuple(method.ReturnType);

        private PropertyInfo[] MatchingProperties(MethodInfo method) {
            var allParms = method.GetParameters();
            var toMatchParms = allParms.Where(p => !p.IsInjectedParameter() && !p.IsTargetParameter()).ToArray();

            if (toMatchParms.Any()) {
                var firstParm = allParms.First();

                if (firstParm.IsTargetParameter()) {
                    var allProperties = firstParm.ParameterType.GetProperties();

                    return allProperties.Where(p => toMatchParms.Any(tmp => string.Equals(p.Name, tmp.Name, StringComparison.CurrentCultureIgnoreCase))).ToArray();
                }
            }

            return new PropertyInfo[] { };
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (method.IsDefined(typeof(EditAttribute), false) && ReturnsContext(method)) {
                var matchingProperties = MatchingProperties(method);

                if (matchingProperties.Any()) {
                    FacetUtils.AddFacet(new EditPropertiesFacet(specification, matchingProperties.Select(p => p.Name).ToArray()));
                }
            }

            return metamodel;
        }
    }
}