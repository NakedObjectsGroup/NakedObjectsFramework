// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.FacetFactory;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflector.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="IDataTypeFacet" /> based on the presence of an
    ///     <see cref="DataTypeAttribute" /> annotation
    /// </summary>
    public sealed class DataTypeAnnotationFacetFactory : ObjectFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        public DataTypeAnnotationFacetFactory(IFacetFactoryOrder<DataTypeAnnotationFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.PropertiesAndActionParameters) { }

        private static void Process(MemberInfo member, ISpecification holder) {
            var dataTypeAttribute = member.GetCustomAttribute<DataTypeAttribute>();
            FacetUtils.AddFacet(Create(dataTypeAttribute, holder));
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            Process(property, specification);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            var dataTypeAttribute = parameter.GetCustomAttribute<DataTypeAttribute>();
            FacetUtils.AddFacet(Create(dataTypeAttribute, holder));
            return metamodel;
        }

        private static IDataTypeFacet Create(DataTypeAttribute attribute, ISpecification holder) =>
            attribute is null
                ? null
                : attribute.DataType == DataType.Custom
                    ? new DataTypeFacetAnnotation(attribute.CustomDataType, holder)
                    : new DataTypeFacetAnnotation(attribute.DataType, holder);
    }
}