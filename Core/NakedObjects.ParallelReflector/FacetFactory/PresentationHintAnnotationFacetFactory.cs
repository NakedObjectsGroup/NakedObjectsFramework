// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Immutable;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class PresentationHintAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public PresentationHintAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Everything) {}

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            var attribute = type.GetCustomAttribute<PresentationHintAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            var attribute = type.GetCustomAttribute<PresentationHintAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
            return metamodel;
        }

        private static void Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<PresentationHintAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            Process(method, specification);
        }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            Process(property, specification);
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IMetamodelBuilder metamodel) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttribute<PresentationHintAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
        }


        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            Process(method, specification);
            return metamodel;
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            Process(property, specification);
            return metamodel;
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttribute<PresentationHintAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
            return metamodel;
        }

        private static IPresentationHintFacet Create(PresentationHintAttribute attribute, ISpecification holder) {
            return attribute != null ? new PresentationHintFacet(attribute.Value, holder) : null;
        }
    }
}