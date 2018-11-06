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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class RegExAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public RegExAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.ObjectsPropertiesAndActionParameters) {}

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Attribute attribute = type.GetCustomAttribute<RegularExpressionAttribute>() ?? (Attribute)type.GetCustomAttribute<RegExAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
            return metamodel;
        }

        private static void Process(MemberInfo member, ISpecification holder) {
            Attribute attribute = member.GetCustomAttribute<RegularExpressionAttribute>() ?? (Attribute) member.GetCustomAttribute<RegExAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (TypeUtils.IsString(method.ReturnType)) {
                Process(method, specification);
            }

            return metamodel;
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            if (property.GetGetMethod() != null && TypeUtils.IsString(property.PropertyType)) {
                Process(property, specification);
            }

            return metamodel;
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            if (TypeUtils.IsString(parameter.ParameterType)) {
                Attribute attribute = parameter.GetCustomAttribute<RegularExpressionAttribute>() ?? (Attribute)parameter.GetCustomAttribute<RegExAttribute>();
                FacetUtils.AddFacet(Create(attribute, holder));
            }

            return metamodel;
        }

        private static IRegExFacet Create(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }
            var expressionAttribute = attribute as RegularExpressionAttribute;
            if (expressionAttribute != null) {
                return Create(expressionAttribute, holder);
            }
            var exAttribute = attribute as RegExAttribute;
            if (exAttribute != null) {
                return Create(exAttribute, holder);
            }
            throw new ArgumentException("Unexpected attribute type: " + attribute.GetType());
        }

        private static IRegExFacet Create(RegExAttribute attribute, ISpecification holder) {
            return new RegExFacet(attribute.Validation, attribute.Format, attribute.CaseSensitive, attribute.Message, holder);
        }

        private static IRegExFacet Create(RegularExpressionAttribute attribute, ISpecification holder) {
            return new RegExFacet(attribute.Pattern, string.Empty, true, attribute.ErrorMessage, holder);
        }
    }
}