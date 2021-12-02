// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;

namespace NakedFramework.Core.Util;

/// <summary>
///     Compares by <see cref="IMemberOrderFacet" /> obtained from each <see cref="IMemberSpecImmutable" />
/// </summary>
/// <para>
///     If there is no attribute on either member, then will compare the members by name instead.
/// </para>
public class MemberOrderComparator<T> : IComparer<T> where T : ISpecification {
    private readonly MemberIdentifierComparator<T> fallbackComparator = new();

    #region IComparer<T> Members

    public int Compare(T o1, T o2) {
        var m1 = GetMemberOrder(o1);
        var m2 = GetMemberOrder(o2);

        if (m1 is null && m2 is null) {
            return fallbackComparator.Compare(o1, o2);
        }

        if (m1 is null) {
            return +1; // annotated before non-annotated
        }

        if (m2 is null) {
            return -1; // annotated before non-annotated
        }

        var components1 = m1.Sequence.Split('.');
        var components2 = m2.Sequence.Split('.');

        var length1 = components1.Length;
        var length2 = components2.Length;

        // shouldn't happen but just in case.
        if (length1 == 0 && length2 == 0) {
            return fallbackComparator.Compare(o1, o2);
        }

        // continue to loop until we run out of components.
        var n = 0;
        while (true) {
            var length = n + 1;
            // check if run out of components in either side
            if (length1 < length && length2 >= length) {
                return -1; // o1 before o2
            }

            if (length2 < length && length1 >= length) {
                return +1; // o2 before o1
            }

            if (length1 < length && length2 < length) {
                // run out of components
                return fallbackComparator.Compare(o1, o2);
            }

            // we have this component on each side
            int componentCompare;
            if (int.TryParse(components1[n], out var c1) && int.TryParse(components2[n], out var c2)) {
                componentCompare = c1.CompareTo(c2);
            }
            else {
                componentCompare = string.Compare(components1[n], components2[n], StringComparison.InvariantCulture);
            }

            if (componentCompare != 0) {
                return componentCompare;
            }

            // this component is the same; lets look at the next
            n++;
        }
    }

    #endregion

    private static IMemberOrderFacet GetMemberOrder(ISpecification specification) => specification.GetFacet<IMemberOrderFacet>();
}