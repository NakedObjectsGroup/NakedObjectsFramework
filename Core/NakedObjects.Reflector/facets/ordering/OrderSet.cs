// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering {
    public class OrderSet<T> : IComparable<IOrderSet<T>>, IOrderSet<T> where T : IOrderableElement<T>, ISpecification {
        private readonly List<IOrderableElement<T>> childOrderSets = new List<IOrderableElement<T>>();
        private readonly List<IOrderableElement<T>> elements = new List<IOrderableElement<T>>();
        private readonly string groupFullName;
        private readonly string groupName;
        private readonly string groupPath;

        public OrderSet(string groupFullName) {
            this.groupFullName = groupFullName;
            groupName = DeriveGroupName(groupFullName);
            groupPath = DeriveGroupPath(groupFullName);
        }

        public IOrderSet<T> Parent { set; get; }

        public IList<IOrderableElement<T>> Children {
            get { return new ReadOnlyCollection<IOrderableElement<T>>(childOrderSets); }
        }

        public IList<T> Flattened {
            get {
                var list = new List<T>();
                foreach (var e in this) {
                    if (e.Peer != null) {
                        list.Add(e.Peer);
                    }
                    else {
                        list.AddRange(e.Set.Flattened);
                    }
                }
                return list; 
            }
        }



        /// <summary>
        ///     Last component of the comma-separated group name supplied in the constructor (analogous to the file
        ///     name extracted from a fully qualified file name)
        /// </summary>
        /// <para>
        ///     For example, if supplied <c>abc,def,ghi</c> in the constructor, then this will return <c>ghi</c>.
        /// </para>
        public string GroupName {
            get { return groupName; }
        }

        /// <summary>
        ///     The group name exactly as it was supplied in the constructor (analogous to a fully qualified file
        ///     name)
        /// </summary>
        /// <para>
        ///     For example, if supplied <c>abc,def,ghi</c> in the constructor, then this will return the same
        ///     string <c>abc,def,ghi</c>.
        /// </para>
        public string GroupFullName {
            get { return groupFullName; }
        }

        /// <summary>
        ///     Represents the parent groups, derived from the group name supplied in the constructor (analogous to the
        ///     directory portion of a fully qualified file name).
        /// </summary>
        /// <para>
        ///     For example, if supplied <c>abc,def,ghi</c> in the constructor, then this will return
        ///     <c>abc,def</c>.
        /// </para>
        public string GroupPath {
            get { return groupPath; }
        }

        #region IComparable<OrderSet> Members

        /// <summary>
        ///     Natural ordering is to compare by <see cref="OrderSet.GroupFullName" />
        /// </summary>
        public int CompareTo(IOrderSet<T> o) {
            return GroupFullName.CompareTo(o.GroupFullName);
        }

        #endregion

        /// <summary>
        ///     Splits name by comma, then title case the last component.
        /// </summary>
        private static string DeriveGroupName(string groupFullName) {
            string[] groupNameComponents = groupFullName.Split(new[] {','});

            string groupSimpleName = groupNameComponents.Length > 0 ? groupNameComponents[groupNameComponents.Length - 1] : "";
            if (groupSimpleName.Length > 1) {
                return groupSimpleName.Substring(0, 1).ToUpper() + groupSimpleName.Substring(1);
            }
            return groupSimpleName.ToUpper();
        }

        /// <summary>
        ///     Everything upto the last comma, else empty string if none.
        /// </summary>
        private static string DeriveGroupPath(string groupFullName) {
            int lastComma = groupFullName.LastIndexOf(",");
            if (lastComma == -1) {
                return "";
            }
            return groupFullName.Substring(0, lastComma);
        }


        /// <summary>
        ///     A staging area until we are ready to add the child sets to the collection of elements owned by the
        ///     superclass.
        /// </summary>
        public void AddChild(IOrderSet<T> childOrderSet) {
            childOrderSets.Add(childOrderSet);
        }

        protected void CopyOverChildren() {
            AddAll(childOrderSets);
        }


        /// <summary>
        ///     Returns a copy of the elements, in sequence.
        /// </summary>
        public IList<IOrderableElement<T>> ElementList() {
            return new ReadOnlyCollection<IOrderableElement<T>>(elements);
        }

        public int Size() {
            return elements.Count;
        }

        protected void AddElement(IOrderableElement<T> element) {
            elements.Add(element);
        }

        public IEnumerator<IOrderableElement<T>> GetEnumerator() {
            return elements.GetEnumerator();
        }

        protected void AddAll(IEnumerable<IOrderableElement<T>> sortedMembers) {
            foreach (IOrderableElement<T> o in sortedMembers) {
                AddElement(o);
            }
        }

        public T Peer {
            get { return default(T); }
        }
        public IOrderSet<T> Set {
            get { return this; }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}