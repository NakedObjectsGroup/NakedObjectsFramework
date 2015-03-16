// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    ///     Provides reflective access to a field on a domain object that is used to reference another domain object
    /// </summary>
    public interface IOneToOneAssociationSpec : IAssociationSpec, IOneToOneFeatureSpec {
        /// <summary>
        ///     Initialise this field in the specified object with the specified reference - this call should only
        ///     affect the specified object, and not any related objects. It should also not be distributed. This is
        ///     strictly for re-initialising the object and not specifying an association, which is only done once.
        /// </summary>
        void InitAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate);

        /// <summary>
        ///     Determines if the specified reference is valid for setting this field in the specified object
        /// </summary>
        IConsent IsAssociationValid(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate);

        /// <summary>
        ///     Set up the association represented by this field in the specified object with the specified reference -
        ///     this call sets up the logical state of the object and might affect other objects that share this
        ///     association (such as back-links or bidirectional association). To initialise a recreated object to this
        ///     logical state the <see cref="InitAssociation" /> method should be used on each of the objects.
        /// </summary>
        void SetAssociation(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter associate);
    }

    // Copyright (c) Naked Objects Group Ltd.
}