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
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.Metamodel.Utils;
using NakedObjects.Reflector.Facet;

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class DisplayAsPropertyAnnotationFacetFactory : ObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        public DisplayAsPropertyAnnotationFacetFactory(IFacetFactoryOrder<DisplayAsPropertyAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Actions) =>
            loggerFactory.CreateLogger<DisplayAsPropertyAnnotationFacetFactory>();

        private static IImmutableDictionary<string, ITypeSpecBuilder> AddIntegrationFacet(IReflector reflector, ISpecificationBuilder specification, Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            ITypeSpecBuilder typeSpecBuilder;
            (typeSpecBuilder, metamodel) = reflector.LoadSpecification(type, metamodel);

            if (specification is IActionSpecImmutable actionSpec) {
                void Action(IMetamodelBuilder b) {
                    var adaptedMember = ImmutableSpecFactory.CreateSpecAdapter(actionSpec);
                    var orderedFields = typeSpecBuilder.Fields.Append(adaptedMember).OrderBy(f => f, new MemberOrderComparator<IAssociationSpecImmutable>()).ToList();
                    FacetUtils.ErrorOnDuplicates(orderedFields.Select(a => new FacetUtils.ActionHolder(a)).ToList());
                    typeSpecBuilder.AddContributedFields(orderedFields);
                }

                FacetUtils.AddIntegrationFacet(typeSpecBuilder, Action);
            }

            return metamodel;
        }


        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            
            if (method.GetCustomAttribute<DisplayAsPropertyAttribute>() is not null && !method.GetParameters().Any()) {
                FacetUtils.AddFacets(new IFacet[] {
                    new DisplayAsPropertyFacet(specification),
                    new PropertyAccessorFacetViaMethod(method, specification, LoggerFactory.CreateLogger<PropertyAccessorFacetViaMethod>()),
                    new MandatoryFacetDefault(specification)
                });

                metamodel = AddIntegrationFacet(reflector, specification, method.DeclaringType, metamodel);

                methodRemover.RemoveMethod(method);
            }

            return metamodel;
        }
    }
}