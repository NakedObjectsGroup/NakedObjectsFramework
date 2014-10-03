using System.Collections.Generic;
using NakedObjects.Architecture.Facets;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering {
    public interface IOrderSet<T> : IOrderableElement<T> where T : IOrderableElement<T>, IFacetHolder {
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
        string GroupFullName { get; }

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
        IEnumerator<IOrderableElement<T>> GetEnumerator();
    }
}