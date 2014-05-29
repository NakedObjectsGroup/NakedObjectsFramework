// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;

namespace NakedObjects.Core.Util {
    public class UnknownTypeException : NakedObjectSystemException {
        public UnknownTypeException() {}

        public UnknownTypeException(string message)
            : base(message) {}

        public UnknownTypeException(Exception cause)
            : base(cause) {}

        public UnknownTypeException(string message, Exception cause)
            : base(message, cause) {}

        public UnknownTypeException(object obj)
            : this(obj == null ? "null" : obj.ToString()) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}