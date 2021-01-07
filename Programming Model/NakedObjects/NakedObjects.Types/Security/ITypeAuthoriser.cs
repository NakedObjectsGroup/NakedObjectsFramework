// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Security.Principal;

namespace NakedObjects.Security {
    /// <summary>
    ///     Implement this interface to manage authorization for a specific class of domain objects.
    ///     The implementation should be registered via NakedObjects.Security.TypeAuthorizerInstaller in the Run class
    /// </summary>
    /// <typeparam name="T">
    ///     T should be a concrete domain type for a type-specific authorizer; 'Object' for a default
    ///     authorizer
    /// </typeparam>
    public interface ITypeAuthorizer<T> {
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