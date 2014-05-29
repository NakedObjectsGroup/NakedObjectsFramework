// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Architecture.Security {
    /// <summary>
    ///     Authorizes the user in the current session view and use members of an object
    /// </summary>
    public interface IAuthorizationManager : IRequiresSetup {
        /// <summary>
        ///     Returns true when the user represented by the specified session is authorized to view the member of the
        ///     class/object represented by the member identifier. Normally the view of the specified field, or the
        ///     display of the action will be suppress if this returns false.
        /// </summary>
        bool IsVisible(ISession session, INakedObject target, IIdentifier identifier);

        /// <summary>
        ///     Returns true when the use represented by the specified session is authorized to change the field
        ///     represented by the member identifier. Normally the specified field will be not appear editable if this
        ///     returns false.
        /// </summary>
        bool IsEditable(ISession session, INakedObject target, IIdentifier identifier);

        /// <summary>
        ///     This is an optimisation to update the cache for usability and visibility in one hit.
        ///     Calling or not calling it should have no functional difference. Depending on the authorization manager
        ///     implementation it may do nothing.
        /// </summary>
        void UpdateAuthorizationCache(INakedObject nakedObject);
    }

    // Copyright (c) Naked Objects Group Ltd.
}