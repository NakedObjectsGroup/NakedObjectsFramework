// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Properties.Validate {
    public abstract class PropertyValidateFacetAbstract : FacetAbstract, IPropertyValidateFacet {
        protected PropertyValidateFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IPropertyValidateFacet); }
        }

        #region IPropertyValidateFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            return InvalidReason(ic.Target, ic.ProposedArgument);
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidException(ic, Invalidates(ic));
        }

        public abstract string InvalidReason(INakedObject nakedObject, INakedObject nakedParm);

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}