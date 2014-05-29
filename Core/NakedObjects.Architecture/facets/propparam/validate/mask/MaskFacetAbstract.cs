// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Mask {
    public abstract class MaskFacetAbstract : SingleStringValueFacetAbstract, IMaskFacet {
        protected MaskFacetAbstract(string stringValue, IFacetHolder holder)
            : base(Type, holder, stringValue) {}

        public static Type Type {
            get { return typeof (IMaskFacet); }
        }

        #region IMaskFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            INakedObject proposedArgument = ic.ProposedArgument;
            if (DoesNotMatch(proposedArgument)) {
                return string.Format(Resources.NakedObjects.MaskMismatch, proposedArgument.TitleString(), Value);
            }
            return null;
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidMaskException(ic, Invalidates(ic));
        }

        public abstract bool DoesNotMatch(INakedObject nakedObject);

        #endregion
    }
}