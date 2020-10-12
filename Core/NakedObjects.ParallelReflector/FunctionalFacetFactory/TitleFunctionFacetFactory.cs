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
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.FunctionalFacetFactory {
    public sealed class TitleFunctionFacetFactory : MethodPrefixBasedFacetFactoryAbstract, IMethodFilteringFacetFactory {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.TitleMethod
        };

        private readonly ILogger<TitleFunctionFacetFactory> logger;

        public TitleFunctionFacetFactory(IFacetFactoryOrder<TitleFunctionFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Objects, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<TitleFunctionFacetFactory>();

        public override string[] Prefixes => FixedPrefixes;

        private static bool IsSameType(ParameterInfo pi, Type toMatch) =>
            pi != null &&
            pi.ParameterType == toMatch;

        #region IMethodFilteringFacetFactory Members

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, IClassStrategy classStrategy, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            // find matching TitleFunction 
            var match = FunctionalIntrospector.Functions.SelectMany(t => t.GetMethods()).Where(m => m.Name == RecognisedMethodsAndPrefixes.TitleMethod).SingleOrDefault(m => IsSameType(m.GetParameters().FirstOrDefault(), type));

            if (match != null) {
                var titleFacet = new TitleFacetViaTitleFunction(match, specification);

                FacetUtils.AddFacet(titleFacet);
            }

            return metamodel;
        }

        public bool Filters(MethodInfo method, IClassStrategy classStrategy) => method.Name == RecognisedMethodsAndPrefixes.TitleMethod;

        #endregion
    }
}