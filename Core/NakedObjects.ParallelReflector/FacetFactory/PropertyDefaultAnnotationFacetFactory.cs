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
    public sealed class PropertyDefaultAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public PropertyDefaultAnnotationFacetFactory(IFacetFactoryOrder<PropertyDefaultAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.Properties, ReflectionType.Both) { }

        private static void Process(MemberInfo member, ISpecification holder) {
            var attribute = (Attribute) member.GetCustomAttribute<DefaultValueAttribute>() ?? member.GetCustomAttribute<NakedFunctions.DefaultValueAttribute>();
            FacetUtils.AddFacet(
                attribute switch {
                    DefaultValueAttribute a => Create(a, holder),
                    NakedFunctions.DefaultValueAttribute a => Create(a, holder),
                    _ => null
                }
            );
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(property, specification);
            return metamodel;
        }

        private static IPropertyDefaultFacet Create(DefaultValueAttribute attribute, ISpecification holder) => attribute == null ? null : new PropertyDefaultFacetAnnotation(attribute.Value, holder);
        private static IPropertyDefaultFacet Create(NakedFunctions.DefaultValueAttribute attribute, ISpecification holder) => attribute == null ? null : new PropertyDefaultFacetAnnotation(attribute.Value, holder);

    }
}