// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    public sealed class RangeAnnotationFacetFactory : ObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        private readonly ILogger<RangeAnnotationFacetFactory> logger;

        public RangeAnnotationFacetFactory(IFacetFactoryOrder<RangeAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.PropertiesAndActionParameters) =>
            logger = loggerFactory.CreateLogger<RangeAnnotationFacetFactory>();

        private void Process(MemberInfo member, bool isDate, ISpecification specification) {
            var attribute = member.GetCustomAttribute<RangeAttribute>();
            FacetUtils.AddFacet(Create(attribute, isDate, specification));
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var isDate = property.PropertyType.IsAssignableFrom(typeof(DateTime));
            Process(property, isDate, specification);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            var isDate = parameter.ParameterType.IsAssignableFrom(typeof(DateTime));
            var range = parameter.GetCustomAttribute<RangeAttribute>();
            FacetUtils.AddFacet(Create(range, isDate, holder));
            return metamodel;
        }

        private IRangeFacet Create(RangeAttribute attribute, bool isDate, ISpecification holder) {
            if (attribute is null) {
                return null;
            }

            if (attribute.OperandType != typeof(int) && attribute.OperandType != typeof(double)) {
                logger.LogWarning($"Unsupported use of range attribute with explicit type on {holder}");
                return null;
            }

            if (attribute.Minimum is IConvertible min && attribute.Maximum is IConvertible max) {
                return new RangeFacet(min, max, isDate, holder);
            }

            logger.LogWarning($"Min Max values must be IConvertible for Range on {holder}");
            return null;
        }
    }
}