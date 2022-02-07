// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace NOF2.Collection {
    public interface InternalCollection : IList { }

    public class InternalCollection<T> : InternalCollection, ICollection<T> {
        private readonly ICollection<T> backingCollection;

        public InternalCollection(ICollection<T> backingCollection) => this.backingCollection = backingCollection;

        public IEnumerator<T> GetEnumerator() => backingCollection.GetEnumerator();

        public void Add(T item) => backingCollection.Add(item);

        public bool Contains(T item) => backingCollection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => backingCollection.CopyTo(array, arrayIndex);

        public bool Remove(T item) => backingCollection.Remove(item);

        public int Add(object value) => throw new NotImplementedException();

        public void Clear() => backingCollection.Clear();
        public bool Contains(object value) => throw new NotImplementedException();

        public int IndexOf(object value) => throw new NotImplementedException();

        public void Insert(int index, object value) {
            throw new NotImplementedException();
        }

        public void Remove(object value) {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index) {
            throw new NotImplementedException();
        }

        public bool IsFixedSize { get; }

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public int Count => backingCollection.Count;
        public bool IsSynchronized { get; } = false;
        public object SyncRoot { get; } = new();
        public bool IsReadOnly => backingCollection.IsReadOnly;

        public object this[int index] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}