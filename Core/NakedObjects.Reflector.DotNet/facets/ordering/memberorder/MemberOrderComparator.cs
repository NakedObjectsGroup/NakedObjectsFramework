// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.MemberOrder {
    /// <summary>
    ///     Compares by <see cref="IMemberOrderFacet" /> obtained from each <see cref="INakedObjectMemberPeer" />
    /// </summary>
    /// <para>
    ///     Will also compare <see cref="OrderSet" />s; these are put after any <see cref="INakedObjectMemberPeer" />s.
    /// </para>
    /// <para>
    ///     If there is no attribute on either member, then will compare the members by name instead.
    /// </para>
    /// <para>
    ///     Can specify if requires that members are in the same (group) name.
    /// </para>
    public class MemberOrderComparator : IComparer<INakedObjectMemberPeer> {
        private readonly bool ensureInSameGroup;
        private readonly MemberIdentifierComparator fallbackComparator = new MemberIdentifierComparator();

        public MemberOrderComparator(bool ensureGroupIsSame) {
            ensureInSameGroup = ensureGroupIsSame;
        }

        #region IComparer<INakedObjectMemberPeer> Members

        public int Compare(INakedObjectMemberPeer o1, INakedObjectMemberPeer o2) {
            IMemberOrderFacet m1 = GetMemberOrder(o1);
            IMemberOrderFacet m2 = GetMemberOrder(o2);

            if (m1 == null && m2 == null) {
                return fallbackComparator.Compare(o1, o2);
            }

            if (m1 == null && m2 != null) {
                return +1; // annotated before non-annotated
            }

            if (m1 != null && m2 == null) {
                return -1; // annotated before non-annotated
            }

            if (ensureInSameGroup && !m1.Name.Equals(m2.Name)) {
                throw new ArgumentException("Not in same group");
            }

            string[] components1 = m1.Sequence.Split(new[] {'.'});
            string[] components2 = m2.Sequence.Split(new[] {'.'});

            int length1 = components1.Length;
            int length2 = components2.Length;

            // shouldn't happen but just in case.
            if (length1 == 0 && length2 == 0) {
                return fallbackComparator.Compare(o1, o2);
            }

            // continue to loop until we run out of components.
            int n = 0;
            while (true) {
                int Length = n + 1;
                // check if run out of components in either side
                if (length1 < Length && length2 >= Length) {
                    return -1; // o1 before o2
                }
                if (length2 < Length && length1 >= Length) {
                    return +1; // o2 before o1
                }
                if (length1 < Length && length2 < Length) {
                    // run out of components
                    return fallbackComparator.Compare(o1, o2);
                }
                // we have this component on each side
                int c1;
                int c2;
                int componentCompare;
                if (int.TryParse(components1[n], out c1) && int.TryParse(components2[n], out c2)) {
                    componentCompare = c1.CompareTo(c2);
                }
                else {
                    componentCompare = components1[n].CompareTo(components2[n]);
                }
                if (componentCompare != 0) {
                    return componentCompare;
                }
                // this component is the same; lets look at the next
                n++;
            }
        }

        #endregion

        private static IMemberOrderFacet GetMemberOrder(IFacetHolder facetHolder) {
            return facetHolder.GetFacet<IMemberOrderFacet>();
        }
    }
}