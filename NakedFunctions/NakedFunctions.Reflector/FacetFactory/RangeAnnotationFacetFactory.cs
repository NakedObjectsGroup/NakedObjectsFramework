// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Metamodel.Facet;
using NakedFramework.Metamodel.Utils;

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class RangeAnnotationFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        private readonly ILogger<RangeAnnotationFacetFactory> logger;

        public RangeAnnotationFacetFactory(IFacetFactoryOrder<RangeAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.ActionParameters) =>
            logger = loggerFactory.CreateLogger<RangeAnnotationFacetFactory>();

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            var isDate = parameter.ParameterType.IsAssignableFrom(typeof(DateTime));
            var range = parameter.GetCustomAttribute<ValueRangeAttribute>();
            FacetUtils.AddFacet(Create(range, isDate, holder));
            return metamodel;
        }

        private IRangeFacet Create(ValueRangeAttribute attribute, bool isDate, ISpecification holder) =>
            attribute is null ? null : new RangeFacet(attribute.Minimum, attribute.Maximum, isDate, holder);
    }
}