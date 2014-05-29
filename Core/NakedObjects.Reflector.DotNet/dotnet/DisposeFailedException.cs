// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Services {
    /// <summary>
    ///     Indicates that the persistence of an object failed.
    /// </summary>
    public class DisposeFailedException : Exception {
        public DisposeFailedException() {}

        public DisposeFailedException(string msg)
            : base(msg) {}

        public DisposeFailedException(string msg, Exception cause)
            : base(msg, cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}