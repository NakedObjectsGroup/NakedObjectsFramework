// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;

namespace NakedObjects.Security {
    /// <summary>
    ///     An implementation of this interface provides authorization for a single fully-qualified type, or for any types within
    ///     a namespace.
    /// </summary>
    public interface INamespaceAuthorizer {
        /// <summary>
        ///     May be a partial namespace, a complete namespace, or a fully-qualified type name
        /// </summary>
        string NamespaceToAuthorize { get; }

        /// <summary>
        ///     Determine whether the member (applies to properties only) may be edited by the current user.
        /// </summary>
        bool IsEditable(IPrincipal principal, object target, string memberName);

        /// <summary>
        ///     Determine whether the member (property or action method) may be seen by the current user.
        /// </summary>
        bool IsVisible(IPrincipal principal, object target, string memberName);
    }
}