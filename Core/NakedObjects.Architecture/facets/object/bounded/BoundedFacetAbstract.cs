// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Objects.Bounded {
    public abstract class BoundedFacetAbstract : MarkerFacetAbstract, IBoundedFacet {
        protected BoundedFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IBoundedFacet); }
        }

        #region IBoundedFacet Members

        public virtual string Disables(InteractionContext ic) {
            if (!ic.TypeEquals(InteractionType.ObjectPersist)) {
                return null;
            }
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
        public abstract string DisabledReason(INakedObject nakedObject);
    }
}