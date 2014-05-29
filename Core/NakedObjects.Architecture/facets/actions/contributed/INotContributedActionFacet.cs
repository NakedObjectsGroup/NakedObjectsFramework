// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Actions.Contributed {
    public interface INotContributedActionFacet : IFacet {
        bool NotContributedTo(INakedObjectSpecification spec);

        bool NeverContributed();
    }
}