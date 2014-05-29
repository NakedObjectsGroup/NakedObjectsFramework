// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;

namespace NakedObjects.Core.Adapter {
    public class TitleException : NakedObjectApplicationException {
        public TitleException() {}

        public TitleException(string msg, Exception cause)
            : base(msg, cause) {}

        public TitleException(string msg)
            : base(msg) {}

        public TitleException(Exception cause)
            : base(cause) {}
    }

    // Copyright (c) Naked Objects Group Ltd.
}