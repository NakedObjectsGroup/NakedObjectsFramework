// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Interactions {
    /// <summary>
    ///     Superclass of exceptions which indicate an attempt to interact with an object or member in a way that is invalid.
    /// </summary>
    public class InvalidException : InteractionException {
        public InvalidException(InteractionContext ic)
            : this(ic, Resources.NakedObjects.Invalid) {}

        public InvalidException(InteractionContext ic, string message)
            : base(ic, message) {}
    }
}