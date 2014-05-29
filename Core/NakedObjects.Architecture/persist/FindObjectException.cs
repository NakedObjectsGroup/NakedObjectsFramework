// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Persist {
    public class FindObjectException : ObjectPersistenceException {
        public FindObjectException() {}

        public FindObjectException(object oid)
            : base(string.Format(Resources.NakedObjects.FindObjectMessage, oid)) {}

        public FindObjectException(string s)
            : base(s) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}