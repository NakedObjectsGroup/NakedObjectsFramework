// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Reflect {
    public class OrderSet<T> : IComparable<IOrderSet<T>>, IOrderSet<T> where T : IOrderableElement<T>, ISpecification {
        private readonly List<IOrderableElement<T>> childOrderSets = new List<IOrderableElement<T>>();
        private readonly List<IOrderableElement<T>> elements = new List<IOrderableElement<T>>();
        private readonly string groupFullName;
        private readonly string groupPath;
        private readonly T[] members;
        private OrderSet<T> parent;

        private OrderSet(string groupFullName) {
            this.groupFullName = groupFullName;
            groupPath = DeriveGroupPath(groupFullName);
        }

        private OrderSet(T[] members)
            : this("") {
            this.members = members;
            parent = null;
        }

        private OrderSet(OrderSet<T> set, string groupName, string name, T[] members)
            : this(groupName) {
            parent = set;
            parent.elements.Add(this);
            this.members = members;
            Add(name);
        }

        private IList<IOrderableElement<T>> Children {
            get { return new ReadOnlyCollection<IOrderableElement<T>>(childOrderSets); }
        }

        /// <summary>
        ///     Represents the parent groups, derived from the group name supplied in the constructor (analogous to the
        ///     directory portion of a fully qualified file name).
        /// </summary>
        /// <para>
        ///     For example, if supplied <c>abc,def,ghi</c> in the constructor, then this will return
        ///     <c>abc,def</c>.
        /// </para>
        private string GroupPath {
            get { return groupPath; }
        }

        #region IComparable<IOrderSet<T>> Members

        /// <summary>
        ///     Natural ordering is to compare by <see cref="OrderSet{T}.GroupFullName" />
        /// </summary>
        public int CompareTo(IOrderSet<T> o) {
            return GroupFullName.CompareTo(o.GroupFullName);
        }

        #endregion

        #region IOrderSet<T> Members

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
        ///     Returns a copy of the elements, in sequence.
        /// </summary>
        public IList<IOrderableElement<T>> ElementList() {
            return new ReadOnlyCollection<IOrderableElement<T>>(elements);
        }

        public T Spec {
            get { return default(T); }
        }

        public IList<IOrderableElement<T>> Set {
            get { return elements.ToList(); }
        }

        #endregion

        private int Size() {
            return elements.Count;
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

        public static OrderSet<T> CreateSimpleOrderSet(string order, T[] members) {
            var set = new OrderSet<T>(members);

            string[] st = order.Split(new[] {','}).Select(s => s.Trim()).ToArray();
            foreach (string element in st) {
                var ends = element.EndsWith(")");
                var tempStr = ends ? element.Substring(0, element.Length - 1).Trim() : element;

                if (tempStr.StartsWith("(")) {
                    int colon = tempStr.IndexOf(':');
                    string groupName = tempStr.Substring(1, colon).Trim();
                    tempStr = tempStr.Substring(colon + 1).Trim();
                    set = new OrderSet<T>(set, groupName, tempStr, set.members);
                }
                else {
                    set.Add(tempStr);
                }

                if (ends) {
                    set = set.parent;
                }
            }
            set.AddAnyRemainingMember();
            return set;
        }

        private void Add(string name) {
            T memberWithName = GetMemberWithName(name);
            if (memberWithName != null) {
                elements.Add(memberWithName);
            }
        }

        private void AddAnyRemainingMember() {
            members.Where(t => t != null).ForEach(m => elements.Add(m));
        }

        private T GetMemberWithName(string name) {
            string searchName = NameUtils.SimpleName(name);
            for (int i = 0; i < members.Length; i++) {
                T member = members[i];
                if (member != null) {
                    string testName = NameUtils.SimpleName(member.Spec.Identifier.MemberName);
                    if (testName.Equals(searchName)) {
                        members[i] = default(T);
                        return member;
                    }
                }
            }
            return default(T);
        }

        public static IOrderSet<T> CreateDeweyOrderSet(T[] members) {
            var sortedMembersByGroup = new SortedList<string, List<T>>();
            var nonAnnotatedGroup = new List<T>();


            // spin over all the members and put them into a Map of SortedSets
            // any non-annotated members go into additional nonAnnotatedGroup set.
            foreach (var member in members) {
                var memberOrder = member.Spec.GetFacet<IMemberOrderFacet>();
                if (memberOrder != null) {
                    List<T> sortedMembersForGroup = GetSortedSet(sortedMembersByGroup, memberOrder.Name);
                    sortedMembersForGroup.Add(member);
                }
                else {
                    nonAnnotatedGroup.Add(member.Spec);
                }
            }

            nonAnnotatedGroup.Sort(new MemberIdentifierComparator<T>());

            foreach (var list in sortedMembersByGroup.Values) {
                list.Sort(new MemberOrderComparator<T>(true));
            }

            // add the non-annotated group to the first "" group.
            IList<T> defaultSet = GetSortedSet(sortedMembersByGroup, "");
            foreach (var member in nonAnnotatedGroup) {
                defaultSet.Add(member);
            }

            // create OrderSets, wiring up parents and children.

            // since sortedMembersByGroup is a SortedMap, the 
            // iteration will be in alphabetical order (ie parent groups before their children). 
            ICollection<string> groupNames = sortedMembersByGroup.Keys;
            IDictionary<string, OrderSet<T>> orderSetsByGroup = new SortedList<string, OrderSet<T>>();

            foreach (string groupName in groupNames) {
                var deweyOrderSet = new OrderSet<T>(groupName);
                orderSetsByGroup.Add(groupName, deweyOrderSet);
                EnsureParentFor(orderSetsByGroup, deweyOrderSet);
            }

            // now populate the OrderSets
            foreach (string groupName in groupNames) {
                OrderSet<T> deweyOrderSet = orderSetsByGroup[groupName];
                IList<T> sortedMembers = sortedMembersByGroup[groupName];
                foreach (var ordeableElement in sortedMembers) {
                    deweyOrderSet.elements.Add(ordeableElement);
                }
                ((IEnumerable<IOrderableElement<T>>) deweyOrderSet.childOrderSets).ForEach(m => deweyOrderSet.elements.Add(m));
            }

            return orderSetsByGroup[""];
        }

        /// <summary>
        ///     Recursively creates parents all the way up to root (<c>""</c>),
        ///     along the way associating each child with its parent and adding
        ///     the child as an element of its parent.
        /// </summary>
        private static void EnsureParentFor(IDictionary<string, OrderSet<T>> orderSetsByGroup, OrderSet<T> deweyOrderSet) {
            string parentGroup = deweyOrderSet.GroupPath;
            var parentOrderSet = orderSetsByGroup[parentGroup];
            if (parentOrderSet == null) {
                parentOrderSet = new OrderSet<T>(parentGroup);
                orderSetsByGroup[parentGroup] = parentOrderSet;
                if (!parentGroup.Equals("")) {
                    EnsureParentFor(orderSetsByGroup, deweyOrderSet);
                }
            }
            // check in case at root
            if (deweyOrderSet != parentOrderSet) {
                deweyOrderSet.parent = parentOrderSet;
                parentOrderSet.childOrderSets.Add(deweyOrderSet);
            }
        }

        /// <summary>
        ///     Gets the SortedSet with the specified group from the supplied Map of SortedSets.
        /// </summary>
        /// <para>
        ///     If there is no such SortedSet, creates.
        /// </para>
        private static List<T> GetSortedSet(IDictionary<string, List<T>> sortedMembersByGroup, string groupName) {
            if (!sortedMembersByGroup.ContainsKey(groupName)) {
                sortedMembersByGroup[groupName] = new List<T>(); // (new MemberOrderComparator(true));
            }
            return sortedMembersByGroup[groupName];
        }

        /// <summary>
        ///     Format is: <c>abc,def:XXel/YYm/ZZch</c>
        ///     Where <c>abc,def</c> is group name,
        ///     <c>XX</c> is number of elements, <c>YY</c> is number of members, and
        ///     <c>ZZ</c> is number of child order sets.
        /// </summary>
        public override string ToString() {
            return GroupFullName + ":" + Size() + "el/" + (Size() - Children.Count) + "m/" + Children.Count + "ch";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}