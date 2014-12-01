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
    public class OrderSet<T> : IOrderSet<T> where T : IOrderableElement<T>, ISpecification {
        private readonly List<IOrderableElement<T>> elements = new List<IOrderableElement<T>>();

        //Constructor
        public OrderSet(T[] members) {
            var annotatedMembers = new List<T>();
            var nonAnnotatedMembers = new List<T>();

            foreach (T member in members) {
                var memberOrder = member.Spec.GetFacet<IMemberOrderFacet>();
                if (memberOrder != null) {
                    annotatedMembers.Add(member);
                } else {
                    nonAnnotatedMembers.Add(member.Spec);
                }
            }

            annotatedMembers.Sort(new MemberOrderComparator<T>());
            nonAnnotatedMembers.Sort(new MemberIdentifierComparator<T>());

            //elements.AddRange(annotatedMembers);
            foreach (T ordeableElement in annotatedMembers) {
                elements.Add(ordeableElement);
            }
            foreach (T ordeableElement in nonAnnotatedMembers) {
                elements.Add(ordeableElement);
            }
        }

        #region IOrderSet<T> Members
        /// <summary>
        ///     Returns a copy of the elements, in sequence.
        /// </summary>
        public IList<IOrderableElement<T>> ElementList() {
            return new ReadOnlyCollection<IOrderableElement<T>>(elements);
        }
       #endregion

        /// <summary>
        ///     Format is: <c>XXel</c>
        ///     Where 
        ///     <c>XX</c> is number of elements
        /// </summary>
        public override string ToString() {
            return "" + elements.Count() + "el";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}