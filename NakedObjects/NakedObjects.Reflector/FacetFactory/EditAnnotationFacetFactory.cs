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
using NakedFramework.ParallelReflector.Utils;
using NakedObjects.Reflector.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class EditAnnotationFacetFactory : ObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        private readonly ILogger<EditAnnotationFacetFactory> logger;

        public EditAnnotationFacetFactory(IFacetFactoryOrder<EditAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ActionsAndActionParameters) =>
            logger = loggerFactory.CreateLogger<EditAnnotationFacetFactory>();

        private IImmutableDictionary<string, ITypeSpecBuilder> Process(MethodInfo method, Action<IDictionary<ParameterInfo, PropertyInfo>> addFacet, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var (isEdit, toEdit) = IsEditMethod(method);
            if (isEdit) {
                var matches = ObjectMethodHelpers.MatchParmsAndProperties(method, toEdit, logger);

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

        private static bool IsContributedAction(MethodInfo member) {
            var allParams = member.GetParameters();
            var paramsWithAttribute = allParams.Where(p => p.GetCustomAttribute<ContributedActionAttribute>() is not null).Where(p => !p.ParameterType.IsValueType).ToArray();
            return paramsWithAttribute.Any();
        }

        private static (bool, Type) IsInstanceEdit(MethodInfo method) {
            if (method.ReturnType == typeof(void)) {
                return (true, method.DeclaringType);
            }

            return (method.DeclaringType?.IsAssignableTo(method.ReturnType) == true, method.ReturnType);
        }


        private static (bool, Type) IsEditMethod(MethodInfo method) {
            if (method.IsDefined(typeof(EditAttribute), false)) {
                return IsContributedAction(method) ? (false, null) : IsInstanceEdit(method);
            }

            return (false, null);
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            void AddFacet(IDictionary<ParameterInfo, PropertyInfo> matches) {
                var thisParameter = method.GetParameters()[paramNum];

                if (matches.ContainsKey(thisParameter)) {
                    var property = matches[thisParameter];
                    // wrap any existing default facet
                    var defaultFacet = specification.GetFacet<IActionDefaultsFacet>();

                    FacetUtils.AddFacet(new ActionDefaultsFacetViaProperty(property, specification, defaultFacet, LoggerFactory.CreateLogger<ActionDefaultsFacetViaProperty>()));
                }
            }

            return Process(method, AddFacet, metamodel);
        }
    }
}