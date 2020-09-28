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
using System.Security.Principal;
using Microsoft.Extensions.Logging;
using NakedFunctions;
using NakedFunctions.Meta.Facet;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.FunctionalFacetFactory {
    /// <summary>
    ///     Sets up all the <see cref="IFacet" />s for an action in a single shot
    /// </summary>
    public sealed class InjectedParameterFacetFactory : FacetFactoryAbstract {
        private readonly ILogger<InjectedParameterFacetFactory> logger;

        public InjectedParameterFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ActionParameters, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<InjectedParameterFacetFactory>();

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parm = method.GetParameters()[paramNum];

            if (FunctionalFacetFactoryHelpers.IsInjectedParameter(method, paramNum)) {
                IFacet facet = null;

                if (CollectionUtils.IsQueryable(parm.ParameterType)) {
                    var elementType = parm.ParameterType.GetGenericArguments().First();
                    facet = new InjectedQueryableParameterFacet(holder, elementType);
                }

                if (parm.ParameterType == typeof(DateTime)) {
                    facet = new InjectedDateTimeParameterFacet(holder);
                }

                if (parm.ParameterType == typeof(Guid)) {
                    facet = new InjectedGuidParameterFacet(holder);
                }

                if (parm.ParameterType == typeof(int)) {
                    facet = new InjectedRandomParameterFacet(holder);
                }

                if (parm.ParameterType == typeof(IPrincipal)) {
                    facet = new InjectedIPrincipalParameterFacet(holder);
                }

                FacetUtils.AddFacet(facet);
            }

            return metamodel;
        }
    }
}