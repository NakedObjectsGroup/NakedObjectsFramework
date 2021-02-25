// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedFramework.Value;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

#pragma warning disable 612

namespace NakedFunctions.Reflector.FacetFactory {
    public sealed class TypicalLengthDerivedFromTypeFacetFactory : FunctionalFacetFactoryProcessor, IAnnotationBasedFacetFactory {
        private static readonly IDictionary<Type, int> TypeMap = new Dictionary<Type, int> {
            {typeof(bool), 5},
            {typeof(byte), 3},
            {typeof(char), 2},
            {typeof(Color), 4},
            {typeof(DateTime), 18},
            {typeof(decimal), 18},
            {typeof(double), 22},
            {typeof(FileAttachment), 0},
            {typeof(float), 12},
            {typeof(Guid), 36},
            {typeof(Image), 0},
            {typeof(int), 11},
            {typeof(long), 20},
            {typeof(sbyte), 3},
            {typeof(short), 6},
            {typeof(string), 25},
            {typeof(TimeSpan), 6},
            {typeof(uint), 10},
            {typeof(ulong), 20},
            {typeof(ushort), 5}
        };

        public TypicalLengthDerivedFromTypeFacetFactory(IFacetFactoryOrder<TypicalLengthDerivedFromTypeFacetFactory> order, ILoggerFactory loggerFactory)
            : base(order.Order, loggerFactory, FeatureType.PropertiesAndActionParameters) { }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, PropertyInfo property, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            AddFacetDerivedFromTypeIfPresent(specification, property.PropertyType, reflector.ClassStrategy);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> Process(IReflector reflector, MethodInfo method, ISpecificationBuilder specification, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var type = method.ReturnType;
            AddFacetDerivedFromTypeIfPresent(specification, type, reflector.ClassStrategy);
            return metamodel;
        }

        public override IImmutableDictionary<string, ITypeSpecBuilder> ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            var parameter = method.GetParameters()[paramNum];
            AddFacetDerivedFromTypeIfPresent(holder, parameter.ParameterType, reflector.ClassStrategy);
            return metamodel;
        }

        private static void AddFacetDerivedFromTypeIfPresent(ISpecification holder, Type type, IClassStrategy classStrategy) => FacetUtils.AddFacet(GetTypicalLengthFacet(type, holder, classStrategy));

        private static ITypicalLengthFacet GetTypicalLengthFacet(Type type, ISpecification holder, IClassStrategy classStrategy) {
            var attribute = type.GetCustomAttribute<TypicalLengthAttribute>();

            if (attribute is not null) {
                return new TypicalLengthFacetDerivedFromType(attribute.Value, holder);
            }

            var length = GetValueTypeTypicalLength(type, classStrategy);
            return length is not null ? new TypicalLengthFacetDerivedFromType(length.Value, holder) : null;
        }

        private static int? GetValueTypeTypicalLength(Type type, IClassStrategy classStrategy) {
            var actualType = TypeKeyUtils.FilterNullableAndProxies(type);

            if (actualType is not null) {
                if (actualType.IsArray) {
                    var elementType = actualType.GetElementType();

                    if (elementType == typeof(string)) {
                        return null;
                    }

                    // byte[] has special facet factory

                    if (elementType is not null && elementType.IsValueType && elementType == typeof(byte)) {
                        return 20;
                    }

                    return null;
                }

                if (actualType.IsEnum) {
                    return 11;
                }

                if (TypeMap.ContainsKey(actualType)) {
                    return TypeMap[actualType];
                }
            }

            return null;
        }
    }
}