using System;
using System.Collections.Generic;
using System.Linq;
using Legacy.NakedObjects.Application.Collection;
using Legacy.NakedObjects.Application.ValueHolder;

namespace Legacy.Rest.Test.Data {
    public static class DomainHelpers {
        public static TextString NewTextString(string initialValue, Action<string> callback) => new(initialValue) { BackingField = callback };

        public static InternalCollection NewInternalCollection<T>(ICollection<T> backingCollection) {
            var newCollection = new InternalCollection(typeof(T).ToString());
            newCollection.init(backingCollection.Cast<object>().ToArray());

            newCollection.BackingField = (obj, add) => {
                if (add) {
                    backingCollection.Add((T)obj);
                }
                else {
                    backingCollection.Remove((T)obj);
                }
            };
            return newCollection;
        }
    }
}