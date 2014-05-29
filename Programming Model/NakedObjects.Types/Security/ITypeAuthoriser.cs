// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;

namespace NakedObjects.Security {
    /// <summary>
    ///     Implement this interface to manage authorization for a specific class of domain objects.
    ///     The implementation should be registered via NakedObjects.Security.TypeAuthorizerInstaller in the Run class
    /// </summary>
    /// <typeparam name="T">T should be a concrete domain type for a type-specific authorizer; 'Object' for a default authorizer</typeparam>
    public interface ITypeAuthorizer<T> {
        /// <summary>
        ///     A hook method for invoking (optional) application-specific logic upon initialization. The method need not do anything.
        /// </summary>
        void Init();

        /// <summary>
        ///     A hook method for invoking (optional) application-specific logic upon shut-down. The method need not do anything.
        /// </summary>
        void Shutdown();

        /// <summary>
        ///     Called only for properties on an object when user attempts to edit the object
        /// </summary>
        /// <param name="principal">Representation of the user</param>
        /// <param name="target">Domain object instance</param>
        /// <param name="memberName">String representation of property name</param>
        /// <returns></returns>
        bool IsEditable(IPrincipal principal, T target, string memberName);

        /// <summary>
        ///     Called on properties and actions on an object when user attempts to view the object
        /// </summary>
        /// <param name="principal">Representation of the user</param>
        /// <param name="target">Domain object instance</param>
        /// <param name="memberName">String representation of property or action name</param>
        /// <returns></returns>
        bool IsVisible(IPrincipal principal, T target, string memberName);
    }
}