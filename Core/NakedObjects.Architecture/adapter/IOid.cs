// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Adapter {
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

        INakedObjectSpecification Specification { get; }

        /// <summary>
        ///     Copies the content of the specified oid into this oid. After this call the hash code return by
        ///     both the specified object and this object will be the same, and both objects will be equal
        ///     (<c>Equals(IOid)</c> returns true)
        /// </summary>
        void CopyFrom(IOid oid);
    }

    // Copyright (c) Naked Objects Group Ltd.
}