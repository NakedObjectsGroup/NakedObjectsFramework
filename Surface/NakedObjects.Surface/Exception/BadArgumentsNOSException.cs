// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;

namespace NakedObjects.Surface {
    public class BadArgumentsNOSException : WithContextNOSException {
        public BadArgumentsNOSException(string message, IList<ContextSurface> contexts) : base(message, contexts) {}

        public BadArgumentsNOSException(string message, ContextSurface context)
            : base(message, context) {}
    }
}