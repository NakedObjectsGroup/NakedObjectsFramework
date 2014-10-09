// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;

namespace NakedObjects.Reflector.Peer {
    public class ReflectionException : NakedObjectSystemException {
        public ReflectionException(string message)
            : base(message) {}

        public ReflectionException(Exception cause)
            : base(cause) {}

        public ReflectionException(string message, Exception cause)
            : base(message, cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}