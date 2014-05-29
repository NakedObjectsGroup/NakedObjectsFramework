// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Actions.Validate {
    public abstract class ActionParameterValidationFacetAbstract : FacetAbstract, IActionParameterValidationFacet {
        protected ActionParameterValidationFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IActionParameterValidationFacet); }
        }

        #region IActionValidationFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            return InvalidReason(ic.Target, ic.ProposedArgument);
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new ActionArgumentsInvalidException(ic, Invalidates(ic));
        }

        public abstract string InvalidReason(INakedObject nakedObject, INakedObject paramValue);

        #endregion
    }
}