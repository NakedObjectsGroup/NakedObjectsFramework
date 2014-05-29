// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture;

namespace NakedObjects.Core.Util {
    public class NakedObjectAssertException : NakedObjectSystemException {
        public NakedObjectAssertException() {}

        public NakedObjectAssertException(string s)
            : base(s) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}