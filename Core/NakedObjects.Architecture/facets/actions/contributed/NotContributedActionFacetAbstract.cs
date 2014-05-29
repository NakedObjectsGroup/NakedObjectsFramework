// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Contributed {
    public abstract class NotContributedActionFacetAbstract : FacetAbstract, INotContributedActionFacet {
        private readonly List<INakedObjectSpecification> notContributedToTypes = new List<INakedObjectSpecification>();

        protected NotContributedActionFacetAbstract(IFacetHolder holder, INakedObjectSpecification[] notContributedToTypes) : base(Type, holder) {
            this.notContributedToTypes.AddRange(notContributedToTypes);
        }

        public static Type Type {
            get { return typeof (INotContributedActionFacet); }
        }

        public bool NotContributedTo(INakedObjectSpecification spec) {
            return NeverContributed() || notContributedToTypes.Any(spec.IsOfType);
        }

        public bool NeverContributed() {
            return !notContributedToTypes.Any();
        }
    }
}