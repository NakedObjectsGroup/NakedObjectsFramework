// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class MaxLengthAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public MaxLengthAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsPropertiesAndActionParameters) {}

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Attribute attribute = type.GetCustomAttribute<StringLengthAttribute>() ?? (Attribute) type.GetCustomAttribute<MaxLengthAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static void Process(MemberInfo member, ISpecification holder) {
            Attribute attribute = member.GetCustomAttribute<StringLengthAttribute>() ?? (Attribute) member.GetCustomAttribute<MaxLengthAttribute>();

            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(method, specification);
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Process(property, specification);
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            Attribute attribute = parameter.GetCustomAttribute<StringLengthAttribute>() ?? (Attribute) parameter.GetCustomAttribute<MaxLengthAttribute>();

            FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IMaxLengthFacet Create(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }
            var lengthAttribute = attribute as StringLengthAttribute;
            if (lengthAttribute != null) {
                return Create(lengthAttribute, holder);
            }
            var maxLengthAttribute = attribute as MaxLengthAttribute;
            if (maxLengthAttribute != null) {
                return Create(maxLengthAttribute, holder);
            }

            throw new ArgumentException("Unexpected attribute type: " + attribute.GetType());
        }

        private static IMaxLengthFacet Create(MaxLengthAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new MaxLengthFacetAnnotation(attribute.Length, holder);
        }

        private static IMaxLengthFacet Create(StringLengthAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new MaxLengthFacetAnnotation(attribute.MaximumLength, holder);
        }
    }
}