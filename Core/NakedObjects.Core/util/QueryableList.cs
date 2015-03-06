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

        [Hidden]
        public IEnumerator GetEnumerator() {
            return wrappedList.GetEnumerator();
        }

        [Hidden]
        public void CopyTo(Array array, int index) {
            wrappedList.CopyTo((T[]) array, index);
        }

        [Hidden]
        public int Count {
            get { return wrappedList.Count; }
        }

        [Hidden]
        public object SyncRoot {
            get { return ((ICollection) wrappedList).SyncRoot; }
        }

        [Hidden]
        public bool IsSynchronized {
            get { return ((ICollection)wrappedList).IsSynchronized; }
        }

        [Hidden]
        public int Add(object value) {
            wrappedList.Add((T) value);
            return wrappedList.Count - 1;
        }

        [Hidden]
        public bool Contains(object value) {
            return wrappedList.Contains((T) value);
        }

        [Hidden]
        public void Clear() {
            wrappedList.Clear();
        }

        [Hidden]
        public int IndexOf(object value) {
            return wrappedList.IndexOf((T) value);
        }

        [Hidden]
        public void Insert(int index, object value) {
            wrappedList.Insert(index, (T) value);
        }

        [Hidden]
        public void Remove(object value) {
            wrappedList.Remove((T) value);
        }

        [Hidden]
        public void RemoveAt(int index) {
            wrappedList.RemoveAt(index);
        }

        [Hidden]
        public object this[int index] {
            get { return wrappedList[index]; }
            set { wrappedList[index] = (T) value; }
        }

        [Hidden]
        public bool IsReadOnly {
            get { return wrappedList.IsReadOnly; }
        }

        [Hidden]
        public bool IsFixedSize {
            get { return ((IList)wrappedList).IsFixedSize; }
        }

        #endregion

        #region IList<T> Members

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return wrappedList.GetEnumerator();
        }

        [Hidden]
        public void CopyTo(T[] array, int arrayIndex) {
            wrappedList.CopyTo(array, arrayIndex);
        }

        [Hidden]
        public bool Remove(T item) {
            return wrappedList.Remove(item);
        }

        [Hidden]
        public void Add(T item) {
            wrappedList.Add(item);
        }

        [Hidden]
        public bool Contains(T item) {
            return wrappedList.Contains(item);
        }

        [Hidden]
        public int IndexOf(T item) {
            return wrappedList.IndexOf(item);
        }

        [Hidden]
        public void Insert(int index, T item) {
            wrappedList.Insert(index, item);
        }

        [Hidden]
        T IList<T>.this[int index] {
            get { return wrappedList[index]; }
            set { wrappedList[index] = value; }
        }

        #endregion

        #region IQueryable<T> Members

        [Hidden]
        public Expression Expression {
            get { return wrappedListAsQueryable.Expression; }
        }

        [Hidden]
        public Type ElementType {
            get { return wrappedListAsQueryable.ElementType; }
        }

        [Hidden]
        public IQueryProvider Provider {
            get { return wrappedListAsQueryable.Provider; }
        }

        #endregion
    }
}