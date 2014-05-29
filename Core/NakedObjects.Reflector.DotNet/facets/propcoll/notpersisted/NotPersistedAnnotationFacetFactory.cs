// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propcoll.NotPersisted;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propcoll.NotPersisted {
    public class NotPersistedAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public NotPersistedAnnotationFacetFactory()
            : base(NakedObjectFeatureType.ObjectsPropertiesAndCollections) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            var attribute = type.GetCustomAttributeByReflection<NotPersistedAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute = member.GetCustomAttribute<NotPersistedAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(property, holder);
        }

        private static INotPersistedFacet Create(NotPersistedAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new NotPersistedFacetAnnotation(holder);
        }
    }
}