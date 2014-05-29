// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Value;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Value {
    public class ValueFacetFactory : AnnotationBasedFacetFactoryAbstract, INakedObjectConfigurationAware {
        public ValueFacetFactory()
            : base(NakedObjectFeatureType.ObjectsOnly) {}

        public override bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            return FacetUtils.AddFacet(Create(type, holder));
        }

        /// <summary>
        ///     Returns a <see cref="IValueFacet" /> implementation.
        /// </summary>
        private static IValueFacet Create(Type type, IFacetHolder holder) {
            // create from annotation, if present
            var annotation = type.GetCustomAttributeByReflection<ValueAttribute>();
            if (annotation != null) {
                if (annotation.SemanticsProviderClass != null || annotation.SemanticsProviderName.Length != 0) {
                    Type annotationType = annotation.SemanticsProviderClass;
                    if (annotationType == null && !string.IsNullOrEmpty(annotation.SemanticsProviderName)) {
                        annotationType = TypeUtils.GetType(annotation.SemanticsProviderName);
                    }
                    PropertyInfo method = annotationType.GetProperty("Parser");
                    Type propertyType = method.PropertyType.GetGenericArguments()[0];
                    if (!propertyType.IsAssignableFrom(type)) {
                        throw new ModelException(string.Format(Resources.NakedObjects.SemanticProviderMismatch, type, propertyType, holder.Identifier.ClassName));
                    }
                }

                var facet = TypeUtils.CreateGenericInstance<IValueFacet>(typeof (ValueFacetAnnotation<>),
                                                                         new[] {type},
                                                                         new object[] {type, holder});
                if (facet.IsValid) {
                    return facet;
                }
            }


            return null;
        }
    }
}