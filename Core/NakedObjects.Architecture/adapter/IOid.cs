// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Adapter {
    /// <summary>
    /// A unique Object Identifier associated with an INakedObjectAdapter (adaptor), and hence, effectively
    /// with a domain object instance.
    /// </summary>
    public interface IOid {
        /// <summary>
        ///     Returns the previous OID if there is one (<see cref="HasPrevious" /> returns true). Returns
        ///     <c>null</c> otherwise (<see cref="HasPrevious" /> returns false)
        /// </summary>
        IOid Previous { get; }

        /// <summary>
        ///     Flags whether this OID is temporary, and is for a transient object
        /// </summary>
        bool IsTransient { get; }

        /// <summary>
        ///     Returns true if this oid contains a previous oid. This is needed when oids are not static and
        ///     change when the identified object is changed
        /// </summary>
        bool HasPrevious { get; }

        ITypeSpec Spec { get; }

        /// <summary>
        ///     Copies the content of the specified oid into this oid. After this call the hash code return by
        ///     both the specified object and this object will be the same, and both objects will be equal
        ///     (<c>Equals(IOid)</c> returns true)
        /// </summary>
        void CopyFrom(IOid oid);
    }

    // Copyright (c) Naked Objects Group Ltd.
}