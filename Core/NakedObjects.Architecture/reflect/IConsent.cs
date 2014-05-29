// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Reflect {
    public interface IConsent {
        /// <summary>
        ///     Returns the permission's reason, detailing why, or why not, permission is being given, or denied
        /// </summary>
        /// <para>
        ///     If an <see cref="Exception" /> is present, then will just equal that exception's
        ///     <see
        ///         cref="System.Exception.Message" />
        /// </para>
        string Reason { get; }

        /// <summary>
        ///     Returns true if this object is giving permission
        /// </summary>
        bool IsAllowed { get; }

        /// <summary>
        ///     Returns true if this object is NOT giving permission
        /// </summary>
        bool IsVetoed { get; }

        /// <summary>
        ///     Represents the reason for an vetoed exception as an inheritance hierarchy
        /// </summary>
        /// <para>
        ///     Consents that are <see cref="IsVetoed" /> will not necessary have a
        ///     <see cref="Reason" /> nor a <see cref="Exception" />, though typically these should
        /// </para>
        /// <para>
        ///     This design allows us to add new checks, (eg for new annotation semantics) without the
        ///     intermediary <see cref="INakedObjectAction" />s and ActionPeer (etc) needing to be aware of
        ///     these new subtypes
        /// </para>
        Exception Exception { get; }
    }

    // Copyright (c) Naked Objects Group Ltd.
}