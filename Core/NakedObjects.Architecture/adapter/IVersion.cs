// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Adapter {
    /// <summary>
    ///     Version marks a INakedObject as being a particular variant of that object
    /// </summary>
    /// <para>
    ///     This is normally done using some form of incrementing number or timestamp, which would be held within
    ///     the implementing class. The numbers, timestamps, etc should change for each changed object, and the
    ///     <see cref="IsDifferent" /> method should indicate that the two Version objects are different
    /// </para>
    /// <para>
    ///     The user's name and a timestamp should also be kept so that when an message is passed to the user it can be
    ///     of the form "user has change object at time"
    /// </para>
    public interface IVersion : IEquatable<IVersion> {
        /// <summary>
        ///     Returns the user who made the last change
        /// </summary>
        string User { get; }

        /// <summary>
        ///     Returns the time of the last change
        /// </summary>
        DateTime? Time { get; }

        /// <summary>
        ///     Returns a hash of the version
        /// </summary>
        string Digest { get; }

        /// <summary>
        ///     Compares this version against the specified version and returns true if they are different versions
        /// </summary>
        /// <para>
        ///     This is use for optimistic checking, where the existence of a different version will normally cause a
        ///     concurrency exception
        /// </para>
        bool IsDifferent(IVersion version);

        /// <summary>
        ///     Compares this version against the specified digest and returns true if they are different versions
        /// </summary>
        /// <para>
        ///     This is use for optimistic checking, where the existence of a different version will normally cause a
        ///     concurrency exception
        /// </para>
        bool IsDifferent(string digest);

        /// <summary>
        ///     Returns the sequence for printing/display
        /// </summary>
        string AsSequence();
    }

    // Copyright (c) Naked Objects Group Ltd.
}