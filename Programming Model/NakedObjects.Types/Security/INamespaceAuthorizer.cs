// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

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