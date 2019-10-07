// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NakedObjects.Core.Util {
    public class QueryableList<T> : IList, IList<T>, IQueryable<T> {
        private readonly IList<T> wrappedList;
        private readonly IQueryable<T> wrappedListAsQueryable;

        public QueryableList(IList<T> wrappedList) {
            this.wrappedList = wrappedList;
            wrappedListAsQueryable = wrappedList.AsQueryable();
        }

        public QueryableList() {
            wrappedList = new List<T>();
            wrappedListAsQueryable = wrappedList.AsQueryable();
        }

        #region IList Members

        [Hidden(WhenTo.Always)]
        public IEnumerator GetEnumerator() {
            return wrappedList.GetEnumerator();
        }

        [Hidden(WhenTo.Always)]
        public void CopyTo(Array array, int index) {
            wrappedList.CopyTo((T[]) array, index);
        }

        [Hidden(WhenTo.Always)]
        public int Count {
            get { return wrappedList.Count; }
        }

        [Hidden(WhenTo.Always)]
        public object SyncRoot {
            get { return ((ICollection) wrappedList).SyncRoot; }
        }

        [Hidden(WhenTo.Always)]
        public bool IsSynchronized {
            get { return ((ICollection)wrappedList).IsSynchronized; }
        }

        [Hidden(WhenTo.Always)]
        public int Add(object value) {
            wrappedList.Add((T) value);
            return wrappedList.Count - 1;
        }

        [Hidden(WhenTo.Always)]
        public bool Contains(object value) {
            return wrappedList.Contains((T) value);
        }

        [Hidden(WhenTo.Always)]
        public void Clear() {
            wrappedList.Clear();
        }

        [Hidden(WhenTo.Always)]
        public int IndexOf(object value) {
            return wrappedList.IndexOf((T) value);
        }

        [Hidden(WhenTo.Always)]
        public void Insert(int index, object value) {
            wrappedList.Insert(index, (T) value);
        }

        [Hidden(WhenTo.Always)]
        public void Remove(object value) {
            wrappedList.Remove((T) value);
        }

        [Hidden(WhenTo.Always)]
        public void RemoveAt(int index) {
            wrappedList.RemoveAt(index);
        }

        [Hidden(WhenTo.Always)]
        public object this[int index] {
            get { return wrappedList[index]; }
            set { wrappedList[index] = (T) value; }
        }

        [Hidden(WhenTo.Always)]
        public bool IsReadOnly {
            get { return wrappedList.IsReadOnly; }
        }

        [Hidden(WhenTo.Always)]
        public bool IsFixedSize {
            get { return ((IList)wrappedList).IsFixedSize; }
        }

        #endregion

        #region IList<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return wrappedList.GetEnumerator();
        }

        [Hidden(WhenTo.Always)]
        public void CopyTo(T[] array, int arrayIndex) {
            wrappedList.CopyTo(array, arrayIndex);
        }

        [Hidden(WhenTo.Always)]
        public bool Remove(T item) {
            return wrappedList.Remove(item);
        }

        [Hidden(WhenTo.Always)]
        public void Add(T item) {
            wrappedList.Add(item);
        }

        [Hidden(WhenTo.Always)]
        public bool Contains(T item) {
            return wrappedList.Contains(item);
        }

        [Hidden(WhenTo.Always)]
        public int IndexOf(T item) {
            return wrappedList.IndexOf(item);
        }

        [Hidden(WhenTo.Always)]
        public void Insert(int index, T item) {
            wrappedList.Insert(index, item);
        }

        [Hidden(WhenTo.Always)]
        T IList<T>.this[int index] {
            get { return wrappedList[index]; }
            set { wrappedList[index] = value; }
        }

        #endregion

        #region IQueryable<T> Members

        [Hidden(WhenTo.Always)]
        public Expression Expression {
            get { return wrappedListAsQueryable.Expression; }
        }

        [Hidden(WhenTo.Always)]
        public Type ElementType {
            get { return wrappedListAsQueryable.ElementType; }
        }

        [Hidden(WhenTo.Always)]
        public IQueryProvider Provider {
            get { return wrappedListAsQueryable.Provider; }
        }

        #endregion
    }
}