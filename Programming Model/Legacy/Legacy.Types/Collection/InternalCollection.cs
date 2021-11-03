using System.Collections;
using System.Collections.Generic;

namespace Legacy.Types {
    public interface InternalCollection { }

    public class InternalCollection<T> : InternalCollection, ICollection<T> {
        private readonly ICollection<T> backingCollection;

        public InternalCollection(ICollection<T> backingCollection) => this.backingCollection = backingCollection;

        public IEnumerator<T> GetEnumerator() => backingCollection.GetEnumerator();

        public void Add(T item) => backingCollection.Add(item);

        public void Clear() => backingCollection.Clear();

        public bool Contains(T item) => backingCollection.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => backingCollection.CopyTo(array, arrayIndex);

        public bool Remove(T item) => backingCollection.Remove(item);

        public int Count => backingCollection.Count;
        public bool IsReadOnly => backingCollection.IsReadOnly;
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}