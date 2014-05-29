// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture {
    /// <summary>
    ///     A NakedObjectSystemException represents an exception that has occurred within the framework code.
    /// </summary>
    public class NakedObjectSystemException : NakedObjectException {
        public NakedObjectSystemException() {}

        public NakedObjectSystemException(string messsage) : base(messsage) {}

        public NakedObjectSystemException(Exception cause) : base(cause) {}

        public NakedObjectSystemException(string messsage, Exception cause) : base(messsage, cause) {}
    }
}