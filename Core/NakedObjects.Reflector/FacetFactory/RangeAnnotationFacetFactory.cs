// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class RangeAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private readonly ILogger<RangeAnnotationFacetFactory> logger;

        public RangeAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.PropertiesAndActionParameters) =>
            logger = loggerFactory.CreateLogger<RangeAnnotationFacetFactory>();

        private void Process(MemberInfo member, bool isDate, ISpecification specification) {
            var attribute = member.GetCustomAttribute<RangeAttribute>();
            FacetUtils.AddFacet(Create(attribute, isDate, specification));
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var isDate = property.PropertyType.IsAssignableFrom(typeof(DateTime));
            Process(property, isDate, specification);
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            var parameter = method.GetParameters()[paramNum];
            var isDate = parameter.ParameterType.IsAssignableFrom(typeof(DateTime));
            var range = parameter.GetCustomAttribute<RangeAttribute>();
            FacetUtils.AddFacet(Create(range, isDate, holder));
        }

        private IRangeFacet Create(RangeAttribute attribute, bool isDate, ISpecification holder) {
            if (attribute == null) {
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