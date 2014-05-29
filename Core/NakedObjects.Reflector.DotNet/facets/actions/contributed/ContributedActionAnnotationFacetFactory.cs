// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Linq;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Contributed;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Core.Context;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Executed {
    /// <summary>
    ///     Creates an <see cref="INotContributedActionFacet" /> based on the presence of an
    ///     <see cref="NotContributedActionAttribute" /> annotation
    /// </summary>
    public class ContributedActionAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ContributedActionAnnotationFacetFactory()
            : base(NakedObjectFeatureType.ActionsOnly) {}

        private static bool Process(MemberInfo member, IFacetHolder holder) {
            var attribute = member.GetCustomAttribute<NotContributedActionAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return Process(method, holder);
        }

        private static INotContributedActionFacet Create(NotContributedActionAttribute attribute, IFacetHolder holder) {
            return attribute == null ? null : new NotContributedActionFacetImpl(holder, attribute.NotContributedToTypes.Select(t => NakedObjectsContext.Reflector.LoadSpecification(t)).ToArray());
        }
    }
}