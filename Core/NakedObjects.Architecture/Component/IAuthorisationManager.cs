// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Component {
    /// <summary>
    ///     Authorizes the user in the current session to view, and to edit/invoke members of an object
    /// </summary>
    public interface IAuthorizationManager {
        /// <summary>
        ///     Returns true when the user represented by the specified session is authorized to view the member of the
        ///     class/object represented by the member identifier. Normally the view of the specified field, or the
        ///     display of the action will be suppress if this returns false.
        /// </summary>
        bool IsVisible(ISession session, ILifecycleManager lifecycleManager, INakedObjectAdapter target, IIdentifier identifier);

        /// <summary>
        ///     Returns true when the use represented by the specified session is authorized to change the field
        ///     represented by the member identifier. Normally the specified field will be not appear editable if this
        ///     returns false.
        /// </summary>
        bool IsEditable(ISession session, ILifecycleManager lifecycleManager, INakedObjectAdapter target, IIdentifier identifier);
    }

    // Copyright (c) Naked Objects Group Ltd.
}