// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Surface;

namespace RestfulObjects.Snapshot.Utility {
    public class BadPersistArgumentsException : BadArgumentsNOSException {
        public BadPersistArgumentsException(string message, IList<ContextSurface> contexts, RestControlFlags flags) : base(message, contexts) {
            Flags = flags;
        }

        public RestControlFlags Flags { get; private set; }
    }
}