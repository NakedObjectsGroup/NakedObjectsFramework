using System;
using System.Collections;
using System.Collections.Generic;

namespace Legacy.Types {
    public interface InternalCollection : IList { }

    public class InternalCollection<T> : InternalCollection, ICollection<T> {

       
        private readonly ICollection<T> backingCollection;

        public InternalCollection(ICollection<T> backingCollection) => this.backingCollection = backingCollection;

        public IEnumerator<T> GetEnumerator() => backingCollection.GetEnumerator();

        public void Add(T item) => backingCollection.Add(item);

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

        public bool Contains(T item) => backingCollection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => backingCollection.CopyTo(array, arrayIndex);

        public bool Remove(T item) => backingCollection.Remove(item);

        public void CopyTo(Array array, int index) {
            throw new NotImplementedException();
        }

        public int Count => backingCollection.Count;
        public bool IsSynchronized { get; } = false;
        public object SyncRoot { get; } = new object();
        public bool IsReadOnly => backingCollection.IsReadOnly;

        public object this[int index] {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

      



    }
}