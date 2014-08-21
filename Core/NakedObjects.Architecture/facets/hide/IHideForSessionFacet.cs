// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Facets.Hide {
    /// <summary>
    ///     Hide a property, collection or action based on the current session.
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>HideXxx</c> support method for the member
    /// </para>
    public interface IHideForSessionFacet : IFacet, IHidingInteractionAdvisor {
        string HiddenReason(ISession session, INakedObject target, INakedObjectPersistor persistor);
    }


    // Copyright (c) Naked Objects Group Ltd.
}