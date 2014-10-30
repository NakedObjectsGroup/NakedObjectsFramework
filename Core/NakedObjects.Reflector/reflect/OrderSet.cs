// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflect {
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
                foreach (var e in this) {
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
            get { return this.Cast<IOrderableElement<T>>().ToList(); }
        }

        public IEnumerator<T> GetEnumerator() {
            return elements.Select(orderableElement => (T) orderableElement).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}