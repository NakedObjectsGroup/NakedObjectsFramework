// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Surface;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Mvc.Model {
    public class ScalarValue : IValue {
        private readonly object internalValue;

        public ScalarValue(object value) {
            internalValue = value;
        }

        #region IValue Members

        public object GetValue(INakedObjectsSurface surface, UriMtHelper helper) {
            return internalValue;
        }

        #endregion
    }
}