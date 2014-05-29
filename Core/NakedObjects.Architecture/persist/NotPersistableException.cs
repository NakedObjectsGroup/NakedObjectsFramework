// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Persist {
    public class NotPersistableException : ObjectPersistenceException {
        public NotPersistableException() {}

        public NotPersistableException(string message)
            : base(message) {}

        public NotPersistableException(Exception cause)
            : base(cause) {}

        public NotPersistableException(string message, Exception cause)
            : base(message, cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}