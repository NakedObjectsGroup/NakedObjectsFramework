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
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;
using NakedFunctions.Reflector.Facet;

namespace NakedFunctions.Reflector.FacetFactory;

public sealed class DisplayAsPropertyAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
    private readonly ILogger<DisplayAsPropertyAnnotationFacetFactory> logger;

    public DisplayAsPropertyAnnotationFacetFactory(IFacetFactoryOrder<DisplayAsPropertyAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
        : base(order.Order, loggerFactory, FeatureType.Actions) =>
        logger = loggerFactory.CreateLogger<DisplayAsPropertyAnnotationFacetFactory>();

    private static bool IsContributedToObject(MethodInfo member) => member.IsDefined(typeof(ExtensionAttribute), false);

    private static Type GetContributeeType(MethodInfo member) => IsContributedToObject(member) ? member.GetParameters().First().ParameterType : member.DeclaringType;

    public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        // all functions are contributed to first parameter or if menu, itself

        if (method.GetCustomAttribute<DisplayAsPropertyAttribute>() is not null) {
            var displayAsPropertyFacet = new DisplayAsPropertyFacet(GetContributeeType(method));

            FacetUtils.AddFacets(new IFacet[] {
                displayAsPropertyFacet,
                new PropertyAccessorFacetViaFunction(method, Logger<PropertyAccessorFacetViaFunction>()),
                MandatoryFacetDefault.Instance,
                DisabledFacetAlways.Instance
            }, specification);
        }

        return metamodel;
    }
}