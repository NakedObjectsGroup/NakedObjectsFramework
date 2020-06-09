// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class TypicalLengthDerivedFromTypeFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TypicalLengthDerivedFromTypeFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.PropertiesAndActionParameters) { }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) => AddFacetDerivedFromTypeIfPresent(reflector, specification, property.PropertyType);

        public override void Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var type = method.ReturnType;
            AddFacetDerivedFromTypeIfPresent(reflector, specification, type);
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            var parameter = method.GetParameters()[paramNum];
            AddFacetDerivedFromTypeIfPresent(reflector, holder, parameter.ParameterType);
        }

        private static void AddFacetDerivedFromTypeIfPresent(IReflector reflector, ISpecification holder, Type type) {
            var facet = GetTypicalLengthFacet(reflector, type);
            if (facet != null) {
                FacetUtils.AddFacet(new TypicalLengthFacetDerivedFromType(facet, holder));
            }
        }

        private static ITypicalLengthFacet GetTypicalLengthFacet(IReflector reflector, Type type) {
            var paramTypeSpec = reflector.LoadSpecification<IObjectSpecImmutable>(type);
            return paramTypeSpec.GetFacet<ITypicalLengthFacet>();
        }
    }
}