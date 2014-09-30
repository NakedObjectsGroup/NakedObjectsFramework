// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture;

namespace NakedObjects.Reflector.Peer {
    /// <summary>
    ///     ModelException represents a problem with the definition of the domain model.
    /// </summary>
    public class ModelException : NakedObjectApplicationException {
        public ModelException(string messsage) : base(messsage) {}

        public ModelException(Exception cause) : base(cause) {}

        public ModelException(string messsage, Exception cause) : base(messsage, cause) {}
    }
}