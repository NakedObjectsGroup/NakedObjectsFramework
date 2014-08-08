// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Util;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Enums {
    public class EnumFacetFactory : FacetFactoryAbstract {
        public EnumFacetFactory(INakedObjectReflector reflector)
            : base(reflector, NakedObjectFeatureType.PropertiesAndParameters) {}


        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            var attribute = property.GetCustomAttribute<EnumDataTypeAttribute>();

            return AddEnumFacet(attribute, holder, property.PropertyType);
        }

        private static bool AddEnumFacet(EnumDataTypeAttribute attribute, IFacetHolder holder, Type typeOfEnum) {
            if (attribute != null) {
                return FacetUtils.AddFacet(Create(attribute, holder));
            }

            Type typeOrNulledType = TypeUtils.GetNulledType(typeOfEnum);
            if (TypeUtils.IsEnum(typeOrNulledType)) {
                return FacetUtils.AddFacet(new EnumFacet(holder, typeOrNulledType));
            }
            if (CollectionUtils.IsGenericOfEnum(typeOfEnum)) {
                Type enumInstanceType = typeOfEnum.GetGenericArguments().First();
                return FacetUtils.AddFacet(new EnumFacet(holder, enumInstanceType));
            }

            return false;
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            var attribute = parameter.GetCustomAttributeByReflection<EnumDataTypeAttribute>();

            return AddEnumFacet(attribute, holder, parameter.ParameterType);
        }

        private static IEnumFacet Create(EnumDataTypeAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new EnumFacet(holder, attribute.EnumType);
        }
    }
}