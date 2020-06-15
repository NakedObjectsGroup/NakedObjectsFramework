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
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class MaxLengthAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private readonly ILogger<MaxLengthAnnotationFacetFactory> logger;

        public MaxLengthAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.ObjectsInterfacesPropertiesAndActionParameters) =>
            logger = loggerFactory.CreateLogger<MaxLengthAnnotationFacetFactory>();

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var attribute = type.GetCustomAttribute<StringLengthAttribute>() ?? (Attribute) type.GetCustomAttribute<MaxLengthAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
        }

        private void Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<StringLengthAttribute>() ?? (Attribute) member.GetCustomAttribute<MaxLengthAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) => Process(method, specification);

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) => Process(property, specification);

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            var parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttribute<StringLengthAttribute>() ?? (Attribute) parameter.GetCustomAttribute<MaxLengthAttribute>();

            FacetUtils.AddFacet(Create(attribute, holder));
        }

        private IMaxLengthFacet Create(Attribute attribute, ISpecification holder) =>
            attribute switch {
                null => null,
                StringLengthAttribute lengthAttribute => Create(lengthAttribute, holder),
                MaxLengthAttribute maxLengthAttribute => Create(maxLengthAttribute, holder),
                _ => throw new ArgumentException(logger.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"))
            };

        private static IMaxLengthFacet Create(MaxLengthAttribute attribute, ISpecification holder) => attribute == null ? null : new MaxLengthFacetAnnotation(attribute.Length, holder);

        private static IMaxLengthFacet Create(StringLengthAttribute attribute, ISpecification holder) => attribute == null ? null : new MaxLengthFacetAnnotation(attribute.MaximumLength, holder);
    }
}