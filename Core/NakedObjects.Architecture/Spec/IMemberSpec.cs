// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    /// Base interface for specifications covering properties and actions.
    /// </summary>
    public interface IMemberSpec : IFeatureSpec {
        /// <summary>
        ///     Returns the identifier of the member, which must not change. This should be all Pascal-case with no
        ///     spaces: so if the member is called 'Return Date' then the a suitable id would be 'ReturnDate'.
        /// </summary>
        string Id { get; }

        IObjectSpec ReturnSpec { get; }

        /// <summary>
        ///     Determines if this member is visible imperatively (ie <c>HideXxx(...)</c>).
        /// </summary>
        /// <param name="target">
        ///     may be <c>null</c> if just checking for authorization
        /// </param>
        bool IsVisible(INakedObjectAdapter target);

        /// <summary>
        ///     Determines if this member is visible imperatively (ie <c>HideXxx(...)</c>).
        ///     Ignores 'UntilPersisted' anotations
        /// </summary>
        /// <param name="target">
        ///     may be <c>null</c> if just checking for authorization
        /// </param>
        bool IsVisibleWhenPersistent(INakedObjectAdapter target);
    }
}