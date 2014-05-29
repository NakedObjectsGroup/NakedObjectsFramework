// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture.Persist {
    public abstract class ObjectPersistenceException : NakedObjectApplicationException {
        protected ObjectPersistenceException() {}

        protected ObjectPersistenceException(string message)
            : base(message) {}

        protected ObjectPersistenceException(string message, Exception cause)
            : base(message, cause) {}

        protected ObjectPersistenceException(Exception cause)
            : base(cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}