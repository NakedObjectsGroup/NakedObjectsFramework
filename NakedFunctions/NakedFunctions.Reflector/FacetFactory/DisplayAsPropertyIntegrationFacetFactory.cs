// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class DisplayAsPropertyIntegrationFacetFactory : FunctionalFacetFactoryProcessor {
    public DisplayAsPropertyIntegrationFacetFactory(IFacetFactoryOrder<DisplayAsPropertyIntegrationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

    private static bool IsStatic(ITypeSpecImmutable spec) => spec.ContainsFacet<ITypeIsStaticFacet>();

    private static bool IsContributedProperty(IActionSpecImmutable sa, ITypeSpecImmutable ts, IMetamodel metamodel) {
        if (sa.GetFacet<IDisplayAsPropertyFacet>()?.ContributedTo is { } type) {
            var contributedToSpec = metamodel.GetSpecification(type);
            return contributedToSpec is not null && ts.IsOfType(contributedToSpec);
        }

        return false;
    }

    private static void PopulateDisplayAsPropertyFunctions(ITypeSpecBuilder spec, ITypeSpecBuilder[] functions, IMetamodel metamodel) {
        var result = functions.AsCustomParallel().SelectMany(functionsSpec =>
                                                                 functionsSpec.UnorderedObjectActions.Where(sa => sa is not null && IsContributedProperty(sa, spec, metamodel))).ToList();

        if (result.Any()) {
            var adaptedMembers = result.Select(r => ImmutableSpecFactory.CreateSpecAdapter(r, metamodel)).ToList();
            spec.AddContributedFields(adaptedMembers);
        }
    }

    private static Action<IMetamodelBuilder, IModelIntegrator> GetAddAction(Type type) {
        void Action(IMetamodelBuilder m, IModelIntegrator mi) {
            if (m.GetSpecification(type) is ITypeSpecBuilder spec) {
                var functions = m.AllSpecifications.Where(IsStatic).OfType<ITypeSpecBuilder>().ToArray();
                PopulateDisplayAsPropertyFunctions(spec, functions, m);
            }
        }

        return Action;
    }

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        if (!FactoryUtils.IsStatic(type)) {
            var action = GetAddAction(type);
            metamodel = FacetUtils.AddIntegrationFacet(reflector, type, action, metamodel);
        }

        return metamodel;
    }
}