// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory {
    /// <summary>
    ///     The interaction is invalid because the property or action parameter is mandatory
    ///     (eg not annotated with <see cref="OptionallyAttribute" />).
    /// </summary>
    public class InvalidMandatoryException : InvalidException {
        public InvalidMandatoryException(InteractionContext ic)
            : this(ic, Resources.NakedObjects.Mandatory) {}

        public InvalidMandatoryException(InteractionContext ic, string message)
            : base(ic, message) {}
    }
}