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
    public sealed class TypicalLengthDerivedFromTypeFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypicalLengthDerivedFromTypeFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.PropertiesAndActionParameters) {}

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            AddFacetDerivedFromTypeIfPresent(reflector, specification, property.PropertyType, metamodel);
        }

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IMetamodelBuilder metamodel) {
            Type type = method.ReturnType;
            AddFacetDerivedFromTypeIfPresent(reflector, specification, type, metamodel);
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IMetamodelBuilder metamodel) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            AddFacetDerivedFromTypeIfPresent(reflector, holder, parameter.ParameterType, metamodel);
        }

        private void AddFacetDerivedFromTypeIfPresent(IReflector reflector, ISpecification holder, Type type, IMetamodelBuilder metamodel) {
            ITypicalLengthFacet facet = GetTypicalLengthFacet(reflector, type, metamodel);
            if (facet != null) {
                FacetUtils.AddFacet(new TypicalLengthFacetDerivedFromType(facet, holder));
            }
        }

        private ITypicalLengthFacet GetTypicalLengthFacet(IReflector reflector, Type type, IMetamodelBuilder metamodel) {
            var paramTypeSpec = reflector.LoadSpecification<IObjectSpecImmutable>(type, metamodel);
            return paramTypeSpec.GetFacet<ITypicalLengthFacet>();
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            return AddFacetDerivedFromTypeIfPresent(reflector, specification, property.PropertyType, metamodel);
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            Type type = method.ReturnType;
            return AddFacetDerivedFromTypeIfPresent(reflector, specification, type, metamodel);
        }

        public override ImmutableDictionary<String, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            return AddFacetDerivedFromTypeIfPresent(reflector, holder, parameter.ParameterType, metamodel);
        }

        private ImmutableDictionary<String, ITypeSpecBuilder> AddFacetDerivedFromTypeIfPresent(IReflector reflector, ISpecification holder, Type type, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            var result = GetTypicalLengthFacet(reflector, type, metamodel);
            ITypicalLengthFacet facet = result.Item1;
            if (facet != null) {
                FacetUtils.AddFacet(new TypicalLengthFacetDerivedFromType(facet, holder));
            }

            return result.Item2;
        }

        private Tuple<ITypicalLengthFacet, ImmutableDictionary<String, ITypeSpecBuilder>> GetTypicalLengthFacet(IReflector reflector, Type type, ImmutableDictionary<String, ITypeSpecBuilder> metamodel) {
            var result = reflector.LoadSpecification(type, metamodel);
            metamodel = result.Item2;
            var paramTypeSpec = result.Item1 as IObjectSpecImmutable;
            return new Tuple<ITypicalLengthFacet, ImmutableDictionary<String, ITypeSpecBuilder>>(paramTypeSpec.GetFacet<ITypicalLengthFacet>(), metamodel);
        }

    }
}