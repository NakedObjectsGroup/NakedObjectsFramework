// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;

namespace NakedObjects.Surface {
    public class BadRequestNOSException : WithContextNOSException {
        public BadRequestNOSException() {}
        public BadRequestNOSException(string message) : base(message) {}
        public BadRequestNOSException(string message, Exception e) : base(message, e) {}
        public BadRequestNOSException(string message, IList<ContextSurface> contexts) : base(message, contexts) {}
        public BadRequestNOSException(string message, ContextSurface context) : base(message, context) {}
    }
}