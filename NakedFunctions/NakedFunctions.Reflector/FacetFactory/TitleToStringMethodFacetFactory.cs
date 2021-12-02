// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using NakedFramework;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class TitleToStringMethodFacetFactory : FunctionalFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.ToStringMethod
    };

    private readonly ILogger<TitleToStringMethodFacetFactory> logger;

    public TitleToStringMethodFacetFactory(IFacetFactoryOrder<TitleToStringMethodFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
        logger = loggerFactory.CreateLogger<TitleToStringMethodFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    /// <summary>
    ///     use ToString for title
    /// </summary>
    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        try {
            var toStringMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ToStringMethod, typeof(string), Type.EmptyTypes);
            var maskMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.ToStringMethod, typeof(string), new[] { typeof(string) });

            if (toStringMethod is not null) {
                // mask method can be null, facet defaults to ToString() which is always there and so no need to pass in 
                IFacet titleFacet = new TitleFacetViaToStringMethod(maskMethod, specification, Logger<TitleFacetViaToStringMethod>());
                FacetUtils.AddFacet(titleFacet);
            }
        }
        catch (Exception e) {
            logger.LogError(e, "Unexpected Exception");
        }

        return metamodel;
    }
}