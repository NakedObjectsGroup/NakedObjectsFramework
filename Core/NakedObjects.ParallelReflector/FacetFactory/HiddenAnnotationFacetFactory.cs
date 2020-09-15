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
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.ParallelReflect.FacetFactory {
    public sealed class HiddenAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public HiddenAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.PropertiesCollectionsAndActions, ReflectionType.Both) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(type.GetCustomAttribute<HiddenAttribute>,
                type.GetCustomAttribute<ScaffoldColumnAttribute>, specification);
            return metamodel;
        }

        private static void Process(MemberInfo member, ISpecification holder) => Process(member.GetCustomAttribute<HiddenAttribute>, member.GetCustomAttribute<ScaffoldColumnAttribute>, holder);

        private static void Process(Func<Attribute> getHidden, Func<Attribute> getScaffold, ISpecification specification) {
            var attribute = getHidden();
            FacetUtils.AddFacet(attribute != null ? Create((HiddenAttribute) attribute, specification) : Create((ScaffoldColumnAttribute) getScaffold(), specification));
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(method, specification);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(property, specification);
            return metamodel;
        }

        private static IHiddenFacet Create(HiddenAttribute attribute, ISpecification holder) => attribute == null ? null : new HiddenFacet(attribute.Value, holder);

        private static IHiddenFacet Create(ScaffoldColumnAttribute attribute, ISpecification holder) => attribute == null ? null : new HiddenFacet(attribute.Scaffold ? WhenTo.Never : WhenTo.Always, holder);
    }
}