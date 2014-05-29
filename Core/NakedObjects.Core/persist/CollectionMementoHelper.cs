// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Core.Persist {
    public static class CollectionMementoHelper {
        public static bool IsPaged(this INakedObject nakedObject) {
            if (nakedObject.Oid is CollectionMemento) {
                return ((CollectionMemento)nakedObject.Oid).IsPaged;
            }
            return false;
        }

        public static bool IsNotQueryable(this INakedObject nakedObject) {
            if (nakedObject.Oid is CollectionMemento) {
                return ((CollectionMemento)nakedObject.Oid).IsNotQueryable;
            }
            return false;
        }

        public static void SetNotQueryable(this INakedObject nakedObject, bool isNotQueryable) {
            if (nakedObject.Oid is CollectionMemento) {
                ((CollectionMemento)nakedObject.Oid).IsNotQueryable = isNotQueryable;
            }
        }
    }
}