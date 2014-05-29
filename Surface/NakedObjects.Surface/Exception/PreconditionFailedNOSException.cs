// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class PreconditionFailedNOSException : NakedObjectsSurfaceException {
        public PreconditionFailedNOSException() {}
        public PreconditionFailedNOSException(string message) : base(message) {}
        public PreconditionFailedNOSException(string message, Exception e) : base(message, e) {}

        public override string Message {
            get { return "Object changed by another user"; }
        }
    }
}