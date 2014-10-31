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
using NakedObjects.Util;

namespace NakedObjects.Reflect {
    public class OrderSet<T> : IComparable<IOrderSet<T>>, IOrderSet<T> where T : IOrderableElement<T>, ISpecification {
        private readonly List<IOrderableElement<T>> childOrderSets = new List<IOrderableElement<T>>();
        private readonly List<IOrderableElement<T>> elements = new List<IOrderableElement<T>>();
        private readonly string groupFullName;
        private readonly string groupName;
        private readonly string groupPath;
        private readonly T[] members;
        private readonly OrderSet<T> parent;

        public OrderSet(string groupFullName) {
            this.groupFullName = groupFullName;
            groupName = DeriveGroupName(groupFullName);
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
            parent.AddElement(this);
            this.members = members;
            Add(name);
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

        public IOrderSet<T> Parent { set; get; }

        public IList<IOrderableElement<T>> Children {
            get { return new ReadOnlyCollection<IOrderableElement<T>>(childOrderSets); }
        }

        public IList<T> Flattened {
            get {
                var list = new List<T>();
                foreach (var e in ElementList()) {
                    if (e.Spec != null) {
                        list.Add(e.Spec);
                    }
                    else {
                        list.AddRange(e.Set.Cast<T>());
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

        /// <summary>
        ///     Returns a copy of the elements, in sequence.
        /// </summary>
        public IList<IOrderableElement<T>> ElementList() {
            return new ReadOnlyCollection<IOrderableElement<T>>(elements);
        }

        public int Size() {
            return elements.Count;
        }

        //public IEnumerator<IOrderableElement<T>> GetEnumerator() {
        //    return elements.GetEnumerator();
        //}

        public T Spec {
            get { return default(T); }
        }

        public IList<IOrderableElement<T>> Set {
            get { return elements.ToList(); }
        }

      

        public List<IOrderableElement<T>> Elements {
            get { return elements; }
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


        protected void AddElement(IOrderableElement<T> element) {
            elements.Add(element);
        }

        protected void AddAll(IEnumerable<IOrderableElement<T>> sortedMembers) {
            foreach (IOrderableElement<T> o in sortedMembers) {
                AddElement(o);
            }
        }

        public static OrderSet<T> CreateSimpleOrderSet(string order, T[] members) {
            var set = new OrderSet<T>(members);

            string[] st = order.Split(new[] {','});
            foreach (string element in st) {
                string tempStr = element.Trim();

                bool ends = tempStr.EndsWith(")");
                if (ends) {
                    tempStr = tempStr.Substring(0, tempStr.Length - 1).Trim();
                }

                if (tempStr.StartsWith("(")) {
                    int colon = tempStr.IndexOf(':');
                    string groupName = tempStr.Substring(1, colon).Trim();
                    tempStr = tempStr.Substring(colon + 1).Trim();
                    set = set.CreateSubOrderSet(groupName, tempStr);
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
                AddElement(memberWithName);
            }
        }

        private void AddAnyRemainingMember() {
            for (int i = 0; i < members.Length; i++) {
                if (members[i] != null) {
                    AddElement(members[i]);
                }
            }
        }

        private OrderSet<T> CreateSubOrderSet(string groupName, string memberName) {
            return new OrderSet<T>(this, groupName, memberName, members);
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

        public OrderSet(string groupName, bool isDewey) : this(groupName) {
            IsDewey = true;
        }

        public bool IsDewey { get; private set; }

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
                var deweyOrderSet = new OrderSet<T>(groupName, true);
                orderSetsByGroup.Add(groupName, deweyOrderSet);
                EnsureParentFor(orderSetsByGroup, deweyOrderSet);
            }

            // now populate the OrderSets
            foreach (string groupName in groupNames) {
                OrderSet<T> deweyOrderSet = orderSetsByGroup[groupName];
                IList<T> sortedMembers = sortedMembersByGroup[groupName];
                foreach (var ordeableElement in sortedMembers) {
                    deweyOrderSet.AddElement(ordeableElement);
                }
                deweyOrderSet.CopyOverChildren();
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
                parentOrderSet = new OrderSet<T>(parentGroup, true);
                orderSetsByGroup[parentGroup] = parentOrderSet;
                if (!parentGroup.Equals("")) {
                    EnsureParentFor(orderSetsByGroup, deweyOrderSet);
                }
            }
            // check in case at root
            if (deweyOrderSet != parentOrderSet) {
                deweyOrderSet.Parent = parentOrderSet;
                parentOrderSet.AddChild(deweyOrderSet);
            }
        }

        /// <summary>
        ///     Gets the SortedSet with the specified group from the supplied Map of SortedSets.
        /// </summary>
        /// <para>
        ///     If there is no such SortedSet, creates.
        /// </para>
        private static List<T> GetSortedSet(IDictionary<string, List<T>> sortedMembersByGroup,
                                            string groupName) {
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