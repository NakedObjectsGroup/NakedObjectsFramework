// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering {
    public class OrderSet : IComparable<OrderSet>, IOrderableElement {
        private readonly List<IOrderableElement> childOrderSets = new List<IOrderableElement>();
        private readonly List<IOrderableElement> elements = new List<IOrderableElement>();
        private readonly string groupFullName;
        private readonly string groupName;
        private readonly string groupPath;

        public OrderSet(string groupFullName) {
            this.groupFullName = groupFullName;
            groupName = DeriveGroupName(groupFullName);
            groupPath = DeriveGroupPath(groupFullName);
        }

        public OrderSet Parent { set; get; }

        public IList<IOrderableElement> Children {
            get { return new ReadOnlyCollection<IOrderableElement>(childOrderSets); }
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
        public int CompareTo(OrderSet o) {
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
        protected void AddChild(DeweyOrderSet childOrderSet) {
            childOrderSets.Add(childOrderSet);
        }

        protected void CopyOverChildren() {
            AddAll(childOrderSets);
        }


        /// <summary>
        ///     Returns a copy of the elements, in sequence.
        /// </summary>
        public IList<IOrderableElement> ElementList() {
            return new ReadOnlyCollection<IOrderableElement>(elements);
        }

        public int Size() {
            return elements.Count;
        }

        protected void AddElement(IOrderableElement element) {
            elements.Add(element);
        }

        public IEnumerator<IOrderableElement> GetEnumerator() {
            return elements.GetEnumerator();
        }

        protected void AddAll(IEnumerable<IOrderableElement> sortedMembers) {
            foreach (IOrderableElement o in sortedMembers) {
                AddElement(o);
            }
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}