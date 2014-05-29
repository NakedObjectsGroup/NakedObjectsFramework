// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;

namespace NakedObjects.Architecture {
    /// <summary>
    ///     A NakedObjectApplicationException represents exception that has occurred within the domain code, or as a result
    ///     of the domain code.  These indicate that the application developer need to fix their code.
    /// </summary>
    public abstract class NakedObjectApplicationException : NakedObjectException {
        protected NakedObjectApplicationException() {}

        protected NakedObjectApplicationException(string messsage) : base(messsage) {}

        protected NakedObjectApplicationException(Exception cause) : base(cause) {}

        protected NakedObjectApplicationException(string messsage, Exception cause) : base(messsage, cause) {}
    }
}