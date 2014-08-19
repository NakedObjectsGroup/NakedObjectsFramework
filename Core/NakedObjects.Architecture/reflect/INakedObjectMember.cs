// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Architecture.Reflect {
    /// <summary>
    ///     Provides reflective access to an action or a field on a domain object
    /// </summary>
    public interface INakedObjectMember : INakedObjectFeature {
        /// <summary>
        ///     Returns the identifier of the member, which must not change. This should be all Pascal-case with no
        ///     spaces: so if the member is called 'Return Date' then the a suitable id would be 'ReturnDate'.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Determines if this member is visible imperatively (ie <c>HideXxx(...)</c>).
        /// </summary>
        /// <param name="session" />
        /// <param name="target">
        ///     may be <c>null</c> if just checking for authorization
        /// </param>
        /// <param name="persistor"></param>
        bool IsVisible(ISession session, INakedObject target, INakedObjectPersistor persistor);
    }
}