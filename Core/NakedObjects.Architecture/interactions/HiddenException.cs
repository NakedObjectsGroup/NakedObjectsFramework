// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Architecture.Interactions {
    /// <summary>
    ///     Superclass of exceptions which indicate an attempt to interact
    ///     with a class member that is in some way hidden or invisible.
    /// </summary>
    public class HiddenException : InteractionException {
        public HiddenException(InteractionContext ic)
            : this(ic, Resources.NakedObjects.Hidden) {}

        public HiddenException(InteractionContext ic, string message)
            : base(ic, message) {}
    }
}