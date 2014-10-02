// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.PageSize;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.PageSize {
    /// <summary>
    ///     Creates an <see cref="IPageSizeFacet" /> based on the presence of an
    ///     <see cref="PageSizeAttribute" /> annotation
    /// </summary>
    public class PageSizeAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public PageSizeAnnotationFacetFactory(IMetadata metadata)
            : base(metadata, NakedObjectFeatureType.ActionsOnly) { }

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute = member.GetCustomAttribute<PageSizeAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(method, holder);
        }

        private static IPageSizeFacet Create(PageSizeAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new PageSizeFacetAnnotation(attribute.Value, holder);
        }
    }
}