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
using NakedFunctions.Reflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class RecordFacetFactory : FunctionalFacetFactoryProcessor {
        public RecordFacetFactory(IFacetFactoryOrder<RecordFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) { }

        private static bool IsContributedFunction(IActionSpecImmutable sa, ITypeSpecImmutable ts) => sa.GetFacet<IContributedFunctionFacet>()?.IsContributedTo(ts) == true;

        private static bool IsContributedFunctionToCollectionOf(IActionSpecImmutable sa, IObjectSpecImmutable ts) => sa.GetFacet<IContributedFunctionFacet>()?.IsContributedToCollectionOf(ts) == true;

        private static bool IsStatic(ITypeSpecImmutable spec) => spec.GetFacet<ITypeIsStaticFacet>()?.Flag == true;

        private static void PopulateContributedFunctions(IObjectSpecBuilder spec, ITypeSpecImmutable[] functions)
        {
            var objectContribActions = functions.AsParallel().SelectMany(functionsSpec => {
                var serviceActions = functionsSpec.ObjectActions.Where(sa => sa != null).ToArray();

                var matchingActionsForObject = new List<IActionSpecImmutable>();

                foreach (var sa in serviceActions)
                {
                    if (IsContributedFunction(sa, spec))
                    {
                        matchingActionsForObject.Add(sa);
                    }
                }

                return matchingActionsForObject;
            }).ToList();

            if (objectContribActions.Any())
            {
                FactoryUtils.ErrorOnDuplicates(objectContribActions.Select(a => new FactoryUtils.ActionHolder(a)).ToList());
                spec.AddContributedFunctions(objectContribActions);
            }

            var collectionContribActions = functions.AsParallel().SelectMany(functionsSpec => {
                var serviceActions = functionsSpec.ObjectActions.Where(sa => sa != null).ToArray();

                var matchingActionsForCollection = new List<IActionSpecImmutable>();

                foreach (var sa in serviceActions)
                {
                    if (IsContributedFunctionToCollectionOf(sa, spec))
                    {
                        matchingActionsForCollection.Add(sa);
                    }
                }

                return matchingActionsForCollection;
            }).ToList();

            if (collectionContribActions.Any())
            {
                FactoryUtils.ErrorOnDuplicates(collectionContribActions.Select(a => new FactoryUtils.ActionHolder(a)).ToList());
                spec.AddCollectionContributedActions(collectionContribActions);
            }
        }

        private static Action<IMetamodelBuilder> GetAddAction(Type type) {
            void Action(IMetamodelBuilder m) {
                if (m.GetSpecification(type) is IObjectSpecBuilder spec) {
                    var functions = m.AllSpecifications.Where(IsStatic).ToArray();
                    PopulateContributedFunctions(spec, functions);
                }
            }

            return Action;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (!FactoryUtils.IsStatic(type)) {
                var action = GetAddAction(type);
                FactoryUtils.AddIntegrationFacet(specification, action);
            }

            return metamodel;
        }

        
    }
}