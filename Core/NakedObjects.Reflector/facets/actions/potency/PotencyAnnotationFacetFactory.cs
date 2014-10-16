// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Potency;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Potency {
    /// <summary>
    ///     Creates an <see cref="IQueryOnlyFacet" /> or <see cref="IIdempotentFacet" />  based on the presence of a
    ///     <see cref="QueryOnlyAttribute" /> or <see cref="IdempotentAttribute" /> annotation
    /// </summary>
    public class PotencyAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public PotencyAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ActionsOnly) { }

        private static bool Process(MemberInfo member, ISpecification holder) {
            // give priority to Idempotent as more restrictive 
            if (member.GetCustomAttribute<IdempotentAttribute>() != null) {
                return FacetUtils.AddFacet(new IdempotentFacetAnnotation(holder));
            }
            if (member.GetCustomAttribute<QueryOnlyAttribute>() != null) {
                return FacetUtils.AddFacet(new QueryOnlyFacetAnnotation(holder));
            }
            return false;
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, specification);
        }
    }
}