// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Facets.Hide {
    /// <summary>
    ///     Hide a property, collection or action based on the current session.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>HideXxx</c> support method for the member.
    /// </para>
    public abstract class HideForSessionFacetAbstract : FacetAbstract, IHideForSessionFacet {
        protected HideForSessionFacetAbstract(IFacetHolder holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IHideForSessionFacet); }
        }

        #region IHideForSessionFacet Members

        public virtual string Hides(InteractionContext ic) {
            return HiddenReason(ic.Session, ic.Target);
        }

        public virtual HiddenException CreateExceptionFor(InteractionContext ic) {
            return new HiddenException(ic, Hides(ic));
        }

        public abstract string HiddenReason(ISession session, INakedObject target);

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}