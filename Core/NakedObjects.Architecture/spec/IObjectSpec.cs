// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    /// Gives access to the full specification (metadata) for a specific domain object type 
    /// (defined by the FullName property). This is the 'runtime specification', which can provide
    /// services in relation to a specific object; where possible its responsibilities are delegated 
    /// to the static version of the specification: IObjectSpecImmutable.
    /// </summary>
    public interface IObjectSpec : ITypeSpec {
        #region Properties

        /// <summary>
        ///     Return all the properties that exist in an object of this specification,
        ///     although they need not all be accessible or visible.
        /// </summary>
        IAssociationSpec[] Properties { get; }

        /// <summary>
        ///     Get the <see cref="IAssociationSpec" /> representing the field with the specified field identifier.
        /// </summary>
        IAssociationSpec GetProperty(string id);

        /// <summary>
        ///     Returns an array of actions that would be contributed to a collection of this object type
        /// </summary>
        IActionSpec[] GetCollectionContributedActions();

        /// <summary>
        ///     Returns an array of actions that can find objects of this type
        /// </summary>
        IActionSpec[] GetFinderActions();

        /// <summary>
        ///     Returns an array of actions that are contributed to a collection of type
        /// </summary>
        IActionSpec[] GetLocallyContributedActions(ITypeSpec typeSpec, string id);

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}