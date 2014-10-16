// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    /// <summary>
    ///     Represents a nested hierarchy of ordered members.
    /// </summary>
    /// <para>
    ///     At each level the elements are either <see cref="IMemberSpecImmutable" />s or they are
    ///     instances of <see cref="OrderSet" /> represent a group of <see cref="IMemberSpecImmutable" />s that
    ///     have a <see cref="IMemberOrderFacet" /> of the same name.
    /// </para>
    /// <para>
    ///     With no name, (ie <c>name=""</c> is the default), at the top level
    /// </para>
    /// <code>
    ///  MemberOrder(sequence="1")
    ///  MemberOrder(sequence="1.1")
    ///  MemberOrder(sequence="1.2")
    ///  MemberOrder(sequence="1.2.1")
    ///  MemberOrder(sequence="1.3")
    ///  </code>
    /// <para>
    ///     With names, creates a hierarchy.
    /// </para>
    /// <code>
    ///  MemberOrder(sequence="1.1")                   // no parent
    ///  MemberOrder(sequence="1.2.1")
    ///  MemberOrder(sequence="1.3")
    ///  MemberOrder(name="abc", sequence="1")         // group is abc, parent is ""
    ///  MemberOrder(name="abc", sequence="1.2")
    ///  MemberOrder(name="abc,def", sequence="1")     // group is def, parent is abc
    ///  MemberOrder(name="abc,def", sequence="1.2")
    ///  </code>
    public class DeweyOrderSet<T> : OrderSet<T> where T : IOrderableElement<T>, ISpecification {
        private DeweyOrderSet(string groupName)
            : base(groupName) {}

        public static DeweyOrderSet<T> CreateOrderSet(T[] members) {
            var sortedMembersByGroup = new SortedList<string, List<T>>();
            var nonAnnotatedGroup = new List<T>();

            // spin over all the members and put them into a Map of SortedSets
            // any non-annotated members go into additional nonAnnotatedGroup set.
            foreach (var member in members) {
                var memberOrder = member.Peer.GetFacet<IMemberOrderFacet>();
                if (memberOrder != null) {
                    List<T> sortedMembersForGroup = GetSortedSet(sortedMembersByGroup, memberOrder.Name);
                    sortedMembersForGroup.Add(member);
                }
                else {
                    nonAnnotatedGroup.Add(member.Peer);
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
            IDictionary<string, DeweyOrderSet<T>> orderSetsByGroup = new SortedList<string, DeweyOrderSet<T>>();

            foreach (string groupName in groupNames) {
                var deweyOrderSet = new DeweyOrderSet<T>(groupName);
                orderSetsByGroup.Add(groupName, deweyOrderSet);
                EnsureParentFor(orderSetsByGroup, deweyOrderSet);
            }

            // now populate the OrderSets
            foreach (string groupName in groupNames) {
                DeweyOrderSet<T> deweyOrderSet = orderSetsByGroup[groupName];
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
        private static void EnsureParentFor(IDictionary<string, DeweyOrderSet<T>> orderSetsByGroup, DeweyOrderSet<T> deweyOrderSet) {
            string parentGroup = deweyOrderSet.GroupPath;
            var parentOrderSet = orderSetsByGroup[parentGroup];
            if (parentOrderSet == null) {
                parentOrderSet = new DeweyOrderSet<T>(parentGroup);
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
}