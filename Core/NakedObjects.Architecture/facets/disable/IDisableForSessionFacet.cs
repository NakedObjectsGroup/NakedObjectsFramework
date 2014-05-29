// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Facets.Disable {
    /// <summary>
    ///     Disable a property, collection or action based on the current session
    /// </summary>
    /// <para>
    ///     In the standard Naked Objects Programming Model, corresponds to
    ///     invoking the <c>DisableXxx</c> support method for the member
    /// </para>
    public interface IDisableForSessionFacet : IFacet {
        /// <summary>
        ///     The reason this is disabled, or <c>null</c> if not
        /// </summary>
        string DisabledReason(ISession session, INakedObject target);
    }


    // Copyright (c) Naked Objects Group Ltd.
}