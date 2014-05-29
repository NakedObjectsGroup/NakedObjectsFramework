// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;

namespace NakedObjects {
    /// <summary>
    ///     Indicates that the persistence of an object failed.
    /// </summary>
    public class PersistFailedException : NakedObjectApplicationException {
        public PersistFailedException(string msg)
            : base(msg) {}

        public PersistFailedException(string msg, Exception cause)
            : base(msg, cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}