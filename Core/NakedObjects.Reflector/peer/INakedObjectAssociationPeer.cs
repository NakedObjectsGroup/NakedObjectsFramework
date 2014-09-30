// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.Peer {
    /// <summary>
    ///     Additional reflective details about field members
    /// </summary>
    public interface INakedObjectAssociationPeer : INakedObjectMemberPeer {
        /// <summary>
        ///     The <see cref="INakedObjectSpecification" /> of the associated
        ///     object if <see cref="IsOneToOne" /> is <c>true</c>, or, the type of the
        ///     associated object (rather than a <see cref="IList" />, say), if <see cref="IsOneToMany" /> is <c>true</c>.
        /// </summary>
        INakedObjectSpecification Specification { get; }

        /// <summary>
        ///     If this is a scalar association, representing (in old terminology)
        ///     a reference to another entity or a value.
        /// </summary>
        /// <para>
        ///     Opposite of <see cref="IsOneToMany" />
        /// </para>
        bool IsOneToOne { get; }

        /// <summary>
        ///     If this is a collection
        /// </summary>
        /// <para>
        ///     Opposite of <see cref="IsOneToOne" />
        /// </para>
        bool IsOneToMany { get; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}