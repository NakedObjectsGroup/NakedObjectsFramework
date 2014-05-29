// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory {
    public abstract class MandatoryFacetAbstract : MarkerFacetAbstract, IMandatoryFacet {
        protected MandatoryFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IMandatoryFacet); }
        }

        #region IMandatoryFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            return IsRequiredButNull(ic.ProposedArgument) ? Resources.NakedObjects.Mandatory : null;
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidMandatoryException(ic, Invalidates(ic));
        }

        public virtual bool IsOptional {
            get { return !IsMandatory; }
        }

        public abstract bool IsMandatory { get; }

        public virtual bool IsRequiredButNull(INakedObject nakedObject) {
            return IsMandatory && nakedObject == null;
        }

        #endregion
    }
}