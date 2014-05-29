// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength {
    /// <summary>
    ///     The interaction is invalid because the input value has exceeded the specified maximum length.
    /// </summary>
    public class InvalidMaxLengthException : InvalidException {
        private readonly int maximumLength;

        public InvalidMaxLengthException(InteractionContext ic, int maximumLength)
            : this(ic, maximumLength, Resources.NakedObjects.MaximumLengthMessage) {}

        public InvalidMaxLengthException(InteractionContext ic, int maximumLength, string message)
            : base(ic, message) {
            this.maximumLength = maximumLength;
        }

        public virtual int MaximumLength {
            get { return maximumLength; }
        }
    }
}