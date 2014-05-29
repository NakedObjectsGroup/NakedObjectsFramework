// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Reflect {
    public static class NullUtils {
        public static bool NullSafeEquals(object previousValue, object newValue) {
            if (previousValue == null && newValue == null)
                return true;
            if (previousValue == null || newValue == null)
                return false;
            if (previousValue.Equals(newValue))
                return true;
            if (previousValue is INakedObject && newValue is INakedObject) {
                var previousNV = (INakedObject) previousValue;
                var newNV = (INakedObject) newValue;
                return NullSafeEquals(previousNV.Object, newNV.Object);
            }
            return false;
        }
    }
}