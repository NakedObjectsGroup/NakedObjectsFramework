// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture {
    /// <summary>
    ///     Indicates an error raised by the application code
    /// </summary>
    public class NakedObjectDomainException : NakedObjectApplicationException {
        public NakedObjectDomainException(string msg)
            : base(msg) {}

        public NakedObjectDomainException(string msg, Exception cause)
            : base(msg, cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}