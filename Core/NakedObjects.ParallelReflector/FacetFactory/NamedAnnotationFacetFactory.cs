// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class NamedAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof(NamedAnnotationFacetFactory));

        public NamedAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Everything) { }

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Attribute attribute = type.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) type.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
            return metamodel;
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Attribute attribute = method.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) method.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
            return metamodel;
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Attribute attribute = property.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) property.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(CreateProperty(attribute, specification));
            return metamodel;
        }

        public override ImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            Attribute attribute = parameter.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) parameter.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
            return metamodel;
        }

        private INamedFacet Create(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }

            var namedAttribute = attribute as NamedAttribute;
            if (namedAttribute != null) {
                return new NamedFacetAnnotation(namedAttribute.Value, holder);
            }

            var nameAttribute = attribute as DisplayNameAttribute;
            if (nameAttribute != null) {
                return new NamedFacetAnnotation(nameAttribute.DisplayName, holder);
            }

            throw new ArgumentException(Log.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"));
        }

        private INamedFacet CreateProperty(Attribute attribute, ISpecification holder) {
            if (attribute == null) {
                return null;
            }

            var namedAttribute = attribute as NamedAttribute;
            if (namedAttribute != null) {
                return Create(namedAttribute, holder);
            }

            var nameAttribute = attribute as DisplayNameAttribute;
            if (nameAttribute != null) {
                return Create(nameAttribute, holder);
            }

            throw new ArgumentException(Log.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"));
        }

        private INamedFacet Create(NamedAttribute attribute, ISpecification holder) {
            return CreateAnnotation(attribute.Value, holder);
        }

        private INamedFacet Create(DisplayNameAttribute attribute, ISpecification holder) {
            return CreateAnnotation(attribute.DisplayName, holder);
        }

        private INamedFacet CreateAnnotation(string name, ISpecification holder) {
            return new NamedFacetAnnotation(name, holder);
        }
       
    }
}