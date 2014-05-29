// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Contributed {
    public class NotContributedActionFacetImpl : NotContributedActionFacetAbstract {
        public NotContributedActionFacetImpl(IFacetHolder holder, INakedObjectSpecification[] notContributedToTypes) : base(holder, notContributedToTypes) {}
    }
}