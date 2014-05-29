// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Interactions {
    /// <summary>
    ///     Superclass of exceptions which indicate an attempt to interact with a class member that is disabled.
    /// </summary>
    public class DisabledException : InteractionException {
        public DisabledException(InteractionContext ic)
            : this(ic, Resources.NakedObjects.Disabled) {}

        public DisabledException(InteractionContext ic, string message)
            : base(ic, message) {}
    }
}