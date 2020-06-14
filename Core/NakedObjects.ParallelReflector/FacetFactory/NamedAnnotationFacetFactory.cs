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
using Microsoft.Extensions.Logging;
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
        private ILogger<NamedAnnotationFacetFactory> logger;

        public NamedAnnotationFacetFactory(int numericOrder, ILoggerFactory loggerFactory)
            : base(numericOrder, loggerFactory, FeatureType.Everything) =>
            logger = loggerFactory.CreateLogger<NamedAnnotationFacetFactory>();

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) type.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var attribute = method.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) method.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var attribute = property.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) property.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(CreateProperty(attribute, specification));
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttribute<DisplayNameAttribute>() ?? (Attribute) parameter.GetCustomAttribute<NamedAttribute>();
            FacetUtils.AddFacet(Create(attribute, holder));
            return metamodel;
        }

        private INamedFacet Create(Attribute attribute, ISpecification holder) =>
            attribute switch {
                null => null,
                NamedAttribute namedAttribute => new NamedFacetAnnotation(namedAttribute.Value, holder),
                DisplayNameAttribute nameAttribute => new NamedFacetAnnotation(nameAttribute.DisplayName, holder),
                _ => throw new ArgumentException(logger.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"))
            };

        private INamedFacet CreateProperty(Attribute attribute, ISpecification holder) =>
            attribute switch {
                null => null,
                NamedAttribute namedAttribute => Create(namedAttribute, holder),
                DisplayNameAttribute nameAttribute => Create(nameAttribute, holder),
                _ => throw new ArgumentException(logger.LogAndReturn($"Unexpected attribute type: {attribute.GetType()}"))
            };

        private static INamedFacet Create(NamedAttribute attribute, ISpecification holder) => CreateAnnotation(attribute.Value, holder);

        private static INamedFacet Create(DisplayNameAttribute attribute, ISpecification holder) => CreateAnnotation(attribute.DisplayName, holder);

        private static INamedFacet CreateAnnotation(string name, ISpecification holder) => new NamedFacetAnnotation(name, holder);
    }
}