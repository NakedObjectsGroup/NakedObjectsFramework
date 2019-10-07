// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class EnumFacetFactory : FacetFactoryAbstract {
        public EnumFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.PropertiesAndActionParameters) { }

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            var attribute = property.GetCustomAttribute<EnumDataTypeAttribute>();

            AddEnumFacet(attribute, specification, property.PropertyType);
        }

        private static void AddEnumFacet(EnumDataTypeAttribute attribute, ISpecification holder, Type typeOfEnum) {
            if (attribute != null) {
                FacetUtils.AddFacet(Create(attribute, holder));
                return;
            }

            Type typeOrNulledType = TypeUtils.GetNulledType(typeOfEnum);
            if (TypeUtils.IsEnum(typeOrNulledType)) {
                FacetUtils.AddFacet(new EnumFacet(holder, typeOrNulledType));
                return;
            }

            if (CollectionUtils.IsGenericOfEnum(typeOfEnum)) {
                Type enumInstanceType = typeOfEnum.GetGenericArguments().First();
                FacetUtils.AddFacet(new EnumFacet(holder, enumInstanceType));
            }
        }

        public override void ProcessParams(IReflector reflector, MethodInfo method, int paramNum, ISpecificationBuilder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttribute<EnumDataTypeAttribute>();
            AddEnumFacet(attribute, holder, parameter.ParameterType);
        }

        private static IEnumFacet Create(EnumDataTypeAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new EnumFacet(holder, attribute.EnumType);
        }
    }
}