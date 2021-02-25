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
using NakedFunctions.Reflector.Facet;
using NakedFunctions.Reflector.Utils;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflector.FacetFactory;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class LifeCycleFunctionsFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.PersistingMethod,
            RecognisedMethodsAndPrefixes.PersistedMethod,
            RecognisedMethodsAndPrefixes.UpdatingMethod,
            RecognisedMethodsAndPrefixes.UpdatedMethod
        };

        private readonly ILogger<LifeCycleFunctionsFacetFactory> logger;

        public LifeCycleFunctionsFacetFactory(IFacetFactoryOrder<LifeCycleFunctionsFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Objects) =>
            logger = loggerFactory.CreateLogger<LifeCycleFunctionsFacetFactory>();

        public string[] Prefixes => FixedPrefixes;

        #region IMethodFilteringFacetFactory Members

        private static readonly IDictionary<string, Func<MethodInfo, ISpecification, IFacet>> matchingFacet = new Dictionary<string, Func<MethodInfo, ISpecification, IFacet>> {
            {RecognisedMethodsAndPrefixes.PersistingMethod, (m, s) => new PersistingCallbackFacetViaFunction(m, s)},
            {RecognisedMethodsAndPrefixes.PersistedMethod, (m, s) => new PersistedCallbackFacetViaFunction(m, s)},
            {RecognisedMethodsAndPrefixes.UpdatingMethod, (m, s) => new UpdatingCallbackFacetViaFunction(m, s)},
            {RecognisedMethodsAndPrefixes.UpdatedMethod, (m, s) => new UpdatedCallbackFacetViaFunction(m, s)}
        };

        private static Action<IMetamodelBuilder> GetAddAction(Type type, string methodName) {
            var recognizedMethod = type.GetMethods().SingleOrDefault(m => m.Name == methodName);
            Action<IMetamodelBuilder> action = m => { };

            if (recognizedMethod is not null) {
                var onType = recognizedMethod.ContributedToType();
                if (onType is not null) {
                    var facetCreator = matchingFacet[methodName];
                    action = m => {
                        var spec = m.GetSpecification(onType);
                        var facet = facetCreator(recognizedMethod, spec);
                        FacetUtils.AddFacet(facet);
                    };
                }
            }

            return action;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var actions = Prefixes.Select(methodName => GetAddAction(type, methodName));
            void Action(IMetamodelBuilder m) => actions.ForEach(a => a(m));
            var integrationFacet = specification.GetFacet<IIntegrationFacet>();

            if (integrationFacet is null) {
                integrationFacet = new IntegrationFacet(specification, Action);
                FacetUtils.AddFacet(integrationFacet);
            }
            else {
                integrationFacet.AddAction(Action);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) {
            return FixedPrefixes.Any(p => p == method.Name);
        }

        #endregion
    }
}