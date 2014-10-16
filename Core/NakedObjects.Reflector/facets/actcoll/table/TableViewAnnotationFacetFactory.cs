// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Table;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Util;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Actcoll.Table {
    public class TableViewAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public TableViewAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.CollectionsAndActions) { }

        private bool Process(MemberInfo member, Type methodReturnType, ISpecification specification) {
            if (CollectionUtils.IsGenericEnumerable(methodReturnType) || CollectionUtils.IsCollection(methodReturnType)) {
                var attribute = member.GetCustomAttribute<TableViewAttribute>();
                return FacetUtils.AddFacet(Create(attribute, specification));
            }

            return false;
        }

        private static ITableViewFacet Create(TableViewAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new TableViewFacetFromAnnotation(attribute.Title, attribute.Columns, holder);
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, method.ReturnType, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            if (property.GetGetMethod() != null) {
                return Process(property, property.PropertyType, specification);
            }
            return false;
        }
    }
}