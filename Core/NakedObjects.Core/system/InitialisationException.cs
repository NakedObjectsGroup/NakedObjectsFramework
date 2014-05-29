// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;

namespace NakedObjects.Core.NakedObjectsSystem {
    /// <summary>
    ///     Indicates a problem initialising the naked objects system
    /// </summary>
    public class InitialisationException : NakedObjectApplicationException {
        public InitialisationException() {}

        public InitialisationException(string s)
            : base(s) {}

        public InitialisationException(Exception cause)
            : base(cause) {}

        public InitialisationException(string msg, Exception cause)
            : base(msg, cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}