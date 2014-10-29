// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.Reflect {
    public interface IOrderSet<T> : IEnumerable<T>, IOrderableElement<T> where T : IOrderableElement<T>, ISpecification {
        IOrderSet<T> Parent { set; get; }
        IList<IOrderableElement<T>> Children { get; }

        /// <summary>
        ///     Last component of the comma-separated group name supplied in the constructor (analogous to the file
        ///     name extracted from a fully qualified file name)
        /// </summary>
        /// <para>
        ///     For example, if supplied <c>abc,def,ghi</c> in the constructor, then this will return <c>ghi</c>.
        /// </para>
        string GroupName { get; }

        /// <summary>
        ///     The group name exactly as it was supplied in the constructor (analogous to a fully qualified file
        ///     name)
        /// </summary>
        /// <para>
        ///     For example, if supplied <c>abc,def,ghi</c> in the constructor, then this will return the same
        ///     string <c>abc,def,ghi</c>.
        /// </para>
        //string GroupFullName { get; }
        /// <summary>
        ///     Represents the parent groups, derived from the group name supplied in the constructor (analogous to the
        ///     directory portion of a fully qualified file name).
        /// </summary>
        /// <para>
        ///     For example, if supplied <c>abc,def,ghi</c> in the constructor, then this will return
        ///     <c>abc,def</c>.
        /// </para>
        string GroupPath { get; }

        IList<T> Flattened { get; }

        /// <summary>
        ///     Natural ordering is to compare by <see cref="OrderSet{T}.GroupFullName" />
        /// </summary>
        int CompareTo(IOrderSet<T> o);

        /// <summary>
        ///     Returns a copy of the elements, in sequence.
        /// </summary>
        IList<IOrderableElement<T>> ElementList();

        int Size();
    }
}