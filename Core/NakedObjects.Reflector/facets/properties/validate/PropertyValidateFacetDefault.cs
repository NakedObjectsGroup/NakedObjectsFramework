// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Properties.Validate;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Reflector.DotNet.Facets.Properties.Validate {
    public class PropertyValidateFacetDefault : FacetAbstract, IPropertyValidateFacet {
        public PropertyValidateFacetDefault(ISpecification holder)
            : base(typeof (IPropertyValidateFacet), holder) {}

        #region IPropertyValidateFacet Members

        public InvalidException CreateExceptionFor(InteractionContext ic) {
            return null;
        }

        public string Invalidates(InteractionContext ic) {
            return null;
        }

        public string InvalidReason(INakedObject target, INakedObject proposedValue) {
            return null;
        }

        #endregion
    }
}