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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.SpecImmutable;
using NakedFramework.Metamodel.Utils;
using NakedFunctions.Reflector.Utils;
using NakedFramework.ParallelReflector.Utils;
using static NakedFramework.ParallelReflector.Utils.FactoryUtils;
using FactoryUtils = NakedFramework.ParallelReflector.Utils.FactoryUtils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class DisplayAsPropertyIntegrationFacetFactory : FunctionalFacetFactoryProcessor {
    public DisplayAsPropertyIntegrationFacetFactory(IFacetFactoryOrder<DisplayAsPropertyIntegrationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

    private static bool IsStatic(ITypeSpecImmutable spec) => spec.GetFacet<ITypeIsStaticFacet>()?.Flag == true;

    private static bool IsContributedProperty(IActionSpecImmutable sa, ITypeSpecImmutable ts) => sa.GetFacet<IDisplayAsPropertyFacet>()?.IsContributedTo(ts) == true;

    private static void PopulateDisplayAsPropertyFunctions(ITypeSpecBuilder spec, ITypeSpecBuilder[] functions, IMetamodel metamodel) {
        var result = functions.AsParallel().SelectMany(functionsSpec => {
            var serviceActions = functionsSpec.UnorderedObjectActions.Where(sa => sa is not null).ToArray();

            var matchingActionsForObject = new List<IActionSpecImmutable>();

            foreach (var sa in serviceActions) {
                if (IsContributedProperty(sa, spec)) {
                    matchingActionsForObject.Add(sa);
                }
            }

            return matchingActionsForObject;
        }).ToList();

        if (result.Any()) {
            var adaptedMembers = result.Select(ImmutableSpecFactory.CreateSpecAdapter).ToList();
            spec.AddContributedFields(adaptedMembers);
        }
    }

    private static Action<IMetamodelBuilder> GetAddAction(Type type) {
        void Action(IMetamodelBuilder m) {
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
            FacetUtils.AddIntegrationFacet(specification, action);
        }

        return metamodel;
    }
}