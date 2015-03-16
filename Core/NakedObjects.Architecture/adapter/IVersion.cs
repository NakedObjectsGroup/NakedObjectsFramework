// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;

namespace NakedObjects.Architecture.Adapter {
    /// <summary>
    ///     Version marks an INakedObjectAdapter as being a particular version of that object.
    ///     This is used in concurrency checking.
    /// </summary>
    /// <para>
    ///     This is normally done using some form of incrementing number or timestamp, which would be held within
    ///     the implementing class. The numbers, timestamps, etc should change for each changed object, and the
    ///     <see cref="IVersion.IsDifferent(IVersion)" /> method should indicate that the two Version objects are different
    /// </para>
    /// <para>
    ///     The user's name and a timestamp should also be kept so that when an message is passed to the user it can be
    ///     of the form "user X has changed object Y at time Z"
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