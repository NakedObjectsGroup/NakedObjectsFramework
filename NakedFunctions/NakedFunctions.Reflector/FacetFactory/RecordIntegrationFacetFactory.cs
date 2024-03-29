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
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class RecordIntegrationFacetFactory : FunctionalFacetFactoryProcessor {
    public RecordIntegrationFacetFactory(IFacetFactoryOrder<RecordIntegrationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

    private static bool IsContributedFunction(IActionSpecImmutable sa, ITypeSpecImmutable ts, IModelIntegrator mi) {
        var facet = sa.GetFacet<IContributedFunctionFacet>();

        if (facet is not null) {
            mi.AddToRemove(facet, sa);

            return facet.IsContributedTo(ts);
        }

        return false;
    }

    private static bool IsContributedFunctionToCollectionOf(IActionSpecImmutable sa, IObjectSpecImmutable ts, IModelIntegrator mi) {
        var facet = sa.GetFacet<IContributedFunctionFacet>();
        if (facet is not null) {
            mi.AddToRemove(facet, sa);
            return facet.IsContributedToCollectionOf(ts);
        }

        return false;
    }

    private static bool IsStatic(ITypeSpecImmutable spec) => spec.ContainsFacet<ITypeIsStaticFacet>();

    private static void PopulateContributedFunctions(IObjectSpecBuilder spec, ITypeSpecBuilder[] functions, IModelIntegrator modelIntegrator) {
        var objectContribActions = functions.AsCustomParallel().SelectMany(functionsSpec => {
            var serviceActions = functionsSpec.UnorderedObjectActions.Where(sa => sa != null).ToArray();

            var matchingActionsForObject = new List<IActionSpecImmutable>();

            foreach (var sa in serviceActions) {
                if (IsContributedFunction(sa, spec, modelIntegrator)) {
                    matchingActionsForObject.Add(sa);
                }
            }

            return matchingActionsForObject;
        }).ToList();

        if (objectContribActions.Any()) {
            objectContribActions.Select(a => new FacetUtils.ActionHolder(a)).ToList().ErrorOnDuplicates();
            spec.AddContributedFunctions(objectContribActions);
        }

        var collectionContribActions = functions.AsCustomParallel().SelectMany(functionsSpec => {
            var serviceActions = functionsSpec.UnorderedObjectActions.Where(sa => sa != null).ToArray();

            var matchingActionsForCollection = new List<IActionSpecImmutable>();

            foreach (var sa in serviceActions) {
                if (IsContributedFunctionToCollectionOf(sa, spec, modelIntegrator)) {
                    matchingActionsForCollection.Add(sa);
                }
            }

            return matchingActionsForCollection;
        }).ToList();

        if (collectionContribActions.Any()) {
            collectionContribActions.Select(a => new FacetUtils.ActionHolder(a)).ToList().ErrorOnDuplicates();
            spec.AddCollectionContributedActions(collectionContribActions);
        }
    }

    private static Action<IMetamodelBuilder, IModelIntegrator> GetAddAction(Type type) {
        void Action(IMetamodelBuilder m, IModelIntegrator mi) {
            if (m.GetSpecification(type) is IObjectSpecBuilder spec) {
                var functions = m.AllSpecifications.Where(IsStatic).OfType<ITypeSpecBuilder>().ToArray();
                PopulateContributedFunctions(spec, functions, mi);
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