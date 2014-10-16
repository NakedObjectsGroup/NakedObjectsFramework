// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Contributed;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    /// <summary>
    ///     Creates an <see cref="IExcludeFromFindMenuFacet" /> based on the presence of an
    ///     <see cref="ExcludeFromFindMenuAttribute" /> annotation
    /// </summary>
    public class ExcludeFromFindMenuAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ExcludeFromFindMenuAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, NakedObjectFeatureType.ActionsOnly) { }

        private static bool Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<ExcludeFromFindMenuAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, specification);
        }

        private static IExcludeFromFindMenuFacet Create(ExcludeFromFindMenuAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new ExcludeFromFindMenuFacetImpl(holder);
        }
    }
}