// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.ParallelReflect.FacetFactory;

namespace NakedObjects.ParallelReflect.FunctionalFacetFactory {
    public sealed class TitleToStringMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes = {
            RecognisedMethodsAndPrefixes.ToStringMethod
        };

        private readonly ILogger<TitleMethodFacetFactory> logger;

        public TitleToStringMethodFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ObjectsAndInterfaces, ReflectionType.Functional) =>
            logger = loggerFactory.CreateLogger<TitleMethodFacetFactory>();

        public override string[] Prefixes => FixedPrefixes;

        /// <summary>
        ///     use ToString for title
        /// </summary>
        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            try {
                var toStringMethod = FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ToStringMethod, typeof(string), Type.EmptyTypes);

                methodRemover.RemoveMethod(toStringMethod);

                IFacet titleFacet = new TitleFacetViaToStringMethod(toStringMethod, specification, Logger<TitleFacetViaToStringMethod>());

                FacetUtils.AddFacet(titleFacet);
            }
            catch (Exception e) {
                logger.LogError(e, "Unexpected Exception");
            }

            return metamodel;
        }
    }
}