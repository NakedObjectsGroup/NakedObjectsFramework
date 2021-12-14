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
using NakedFramework.ParallelReflector.FacetFactory;
using NakedFramework.ParallelReflector.Utils;

namespace NakedObjects.Reflector.FacetFactory;

/// <summary>
///     Note - this factory simply removes the class level attribute from the list of methods.  The action and properties
///     look up this attribute directly
/// </summary>
public sealed class DisableDefaultMethodFacetFactory : DomainObjectFacetFactoryProcessor, IMethodPrefixBasedFacetFactory {
    private static readonly string[] FixedPrefixes;
    private readonly ILogger<DisableDefaultMethodFacetFactory> logger;

    static DisableDefaultMethodFacetFactory() =>
        FixedPrefixes = new[] {
            RecognisedMethodsAndPrefixes.DisablePrefix + "Action" + RecognisedMethodsAndPrefixes.DefaultPrefix,
            RecognisedMethodsAndPrefixes.DisablePrefix + "Property" + RecognisedMethodsAndPrefixes.DefaultPrefix
        };

    public DisableDefaultMethodFacetFactory(IFacetFactoryOrder<DisableDefaultMethodFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.ObjectsAndInterfaces) =>
        logger = loggerFactory.CreateLogger<DisableDefaultMethodFacetFactory>();

    public string[] Prefixes => FixedPrefixes;

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        try {
            foreach (var methodName in FixedPrefixes) {
                var methodInfo = MethodHelpers.FindMethod(reflector, type, MethodType.Object, methodName, typeof(string), Type.EmptyTypes);
                methodRemover.SafeRemoveMethod(methodInfo);
            }
        }
        catch (Exception e) {
            logger.LogError(e, "Unexpected exception");
        }

        return metamodel;
    }
}