// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Spec {
    /// <summary>
    ///     Provides reflective access to a field on a domain object
    /// </summary>
    public interface IAssociationSpec : IMemberSpec {
        /// <summary>
        ///     Returns true if this field is persisted, and not calculated from other data in the object or
        ///     used transiently
        /// </summary>
        bool IsPersisted { get; }

        /// <summary>
        ///     Determines if this field must be complete before the object is in a valid state
        /// </summary>
        bool IsMandatory { get; }

        /// <summary>
        ///     Returns true is this property cannot be externally set
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        ///     Returns <c>true</c> if this field on the specified object is inlined
        /// </summary>
        bool IsInline { get; }

        /// <summary>
        ///     Returns the <see cref="INakedObjectAdapter" /> adapting this field's object. This invokes the specified
        ///     instances accessor method and creates an adapter for the returned value
        /// </summary>
        INakedObjectAdapter GetNakedObject(INakedObjectAdapter target);

        /// <summary>
        ///     Return the default for this property
        /// </summary>
        INakedObjectAdapter GetDefault(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     Return the default for this property
        /// </summary>
        TypeOfDefaultValue GetDefaultType(INakedObjectAdapter nakedObjectAdapter);

        /// <summary>
        ///     Set the property to its default references or values
        /// </summary>
        void ToDefault(INakedObjectAdapter target);

        /// <summary>
        ///     Returns <c>true</c> if this field on the specified object is deemed to be empty, or has no content
        /// </summary>
        bool IsEmpty(INakedObjectAdapter inObjectAdapter);
    }

    // Copyright (c) Naked Objects Group Ltd.
}