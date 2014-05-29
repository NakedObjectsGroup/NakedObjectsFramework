// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Objects.Immutable {
    public abstract class ImmutableFacetAbstract : SingleWhenValueFacetAbstract, IImmutableFacet {
        protected ImmutableFacetAbstract(When when, IFacetHolder holder)
            : base(Type, holder, when) {}

        public static Type Type {
            get { return typeof (IImmutableFacet); }
        }

        #region IImmutableFacet Members

        public virtual string Disables(InteractionContext ic) {
            INakedObject target = ic.Target;
            return DisabledReason(target);
        }

        public virtual DisabledException CreateExceptionFor(InteractionContext ic) {
            return new DisabledException(ic, Disables(ic));
        }

        #endregion

        /// <summary>
        ///     Hook method for subclasses to override
        /// </summary>
        public abstract string DisabledReason(INakedObject no);
    }
}