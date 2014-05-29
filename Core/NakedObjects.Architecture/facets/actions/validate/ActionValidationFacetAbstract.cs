// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Actions.Validate {
    public abstract class ActionValidationFacetAbstract : FacetAbstract, IActionValidationFacet {
        protected ActionValidationFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IActionValidationFacet); }
        }

        #region IActionValidationFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            return InvalidReason(ic.Target, ic.ProposedArguments);
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new ActionArgumentsInvalidException(ic, Invalidates(ic));
        }

        public abstract string InvalidReason(INakedObject nakedObject, INakedObject[] param2);

        #endregion
    }
}