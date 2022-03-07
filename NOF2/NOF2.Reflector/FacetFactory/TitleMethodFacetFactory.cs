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
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Utils;
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;
using NOF2.Reflector.Facet;
using NOF2.Title;

namespace NOF2.Reflector.FacetFactory;

public sealed class TitleMethodFacetFactory : AbstractNOF2FacetFactoryProcessor, IMethodPrefixBasedFacetFactory, IAnnotationBasedFacetFactory {
    private static readonly string[] FixedPrefixes = {
        RecognisedMethodsAndPrefixes.TitleMethod
    };

    private readonly ILogger<TitleMethodFacetFactory> logger;

    public TitleMethodFacetFactory(IFacetFactoryOrder<TitleMethodFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
        logger = loggerFactory.CreateLogger<TitleMethodFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    /// <summary>
    ///     If no title or ToString can be used then will use Facets provided by
    ///     <see cref="FallbackFacetFactory" /> instead.
    /// </summary>
    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        try {
            var titleMethod = MethodHelpers.FindMethod(reflector, type, MethodType.Object, RecognisedMethodsAndPrefixes.TitleMethod, null, Type.EmptyTypes);
            if (titleMethod is not null && titleMethod.ReturnType.IsAssignableTo(typeof(ITitle))) {
                methodRemover.SafeRemoveMethod(titleMethod);
                var titleFacet = new TitleFacetViaTitleMethod(titleMethod, specification, Logger<TitleFacetViaTitleMethod>());
                FacetUtils.AddFacet(titleFacet, specification);
            }
        }
        catch (Exception e) {
            logger.LogError(e, "Unexpected Exception");
        }

        return metamodel;
    }
}