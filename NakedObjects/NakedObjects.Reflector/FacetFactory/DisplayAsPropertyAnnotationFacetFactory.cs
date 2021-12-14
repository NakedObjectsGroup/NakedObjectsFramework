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

namespace NakedObjects.Reflector.FacetFactory;

public sealed class DisplayAsPropertyAnnotationFacetFactory : DomainObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    public DisplayAsPropertyAnnotationFacetFactory(IFacetFactoryOrder<DisplayAsPropertyAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) =>
        loggerFactory.CreateLogger<DisplayAsPropertyAnnotationFacetFactory>();

    private static void RemoveAction(IActionSpecImmutable actionSpec) {
        if (actionSpec.OwnerSpec is ITypeSpecBuilder tsb) {
            tsb.RemoveAction(actionSpec);
        }
    }

    private static IImmutableDictionary<string, ITypeSpecBuilder> AddIntegrationFacet(IReflector reflector, ISpecificationBuilder specification, Type type, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        ITypeSpecBuilder displayOnTypeSpec;
        (displayOnTypeSpec, metamodel) = reflector.LoadSpecification(type, metamodel);

        if (specification is IActionSpecImmutable actionSpec) {
            void Action(IMetamodelBuilder b) {
                var adaptedMember = ImmutableSpecFactory.CreateSpecAdapter(actionSpec);
                displayOnTypeSpec.AddContributedFields(new[] { adaptedMember });
                RemoveAction(actionSpec);
            }

            FacetUtils.AddIntegrationFacet(displayOnTypeSpec, Action);
        }

        return metamodel;
    }

    private static bool IsObjectMethod(MethodInfo method) => method.GetCustomAttribute<DisplayAsPropertyAttribute>() is not null && !method.GetParameters().Any();

    private static bool IsContributedMethod(MethodInfo method) => method.GetCustomAttribute<DisplayAsPropertyAttribute>() is not null
                                                                  && method.GetParameters().Length == 1
                                                                  && method.GetParameters().First().GetCustomAttribute<ContributedActionAttribute>() is not null
                                                                  && !FasterTypeUtils.IsGenericCollection(method.GetParameters().First().ParameterType);

    private static bool MatchesPropertySignature(MethodInfo method) => method.ReturnType != typeof(void);

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if ((IsObjectMethod(method) || IsContributedMethod(method)) && MatchesPropertySignature(method)) {
            Type displayOnType;
            IPropertyAccessorFacet accessorFacet;

            if (IsContributedMethod(method)) {
                displayOnType = method.GetParameters().First().ParameterType;
                accessorFacet = new PropertyAccessorFacetViaContributedAction(method, specification, LoggerFactory.CreateLogger<PropertyAccessorFacetViaContributedAction>());
            }
            else {
                displayOnType = method.DeclaringType;
                accessorFacet = new PropertyAccessorFacetViaMethod(method, specification, LoggerFactory.CreateLogger<PropertyAccessorFacetViaMethod>());
            }

            FacetUtils.AddFacets(new IFacet[] {
                new DisplayAsPropertyFacet(specification),
                accessorFacet,
                new MandatoryFacetDefault(specification),
                new DisabledFacetAlways(specification)
            });

            metamodel = AddIntegrationFacet(reflector, specification, displayOnType, metamodel);

            methodRemover.RemoveMethod(method);
        }

        return metamodel;
    }
}