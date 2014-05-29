// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Surface {
    public class NakedObjectsSurfaceException : Exception {
        public NakedObjectsSurfaceException(string message, Exception e) : base(message, e) {}

        public NakedObjectsSurfaceException(Exception e) : base("", e) {}

        public NakedObjectsSurfaceException() {}

        public NakedObjectsSurfaceException(string message) : base(message) {}
    }


    // todo rename to be less coupled to httpstatus code
}