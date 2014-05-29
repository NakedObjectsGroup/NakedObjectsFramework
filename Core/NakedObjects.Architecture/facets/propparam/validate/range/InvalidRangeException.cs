// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.Range {
    /// <summary>
    ///     The interaction is invalid because the input value is outside the specified range.
    /// </summary>
    public class InvalidRangeException : InvalidException {
        public InvalidRangeException(InteractionContext ic, object min, object max)
            : this(ic, min, max, Resources.NakedObjects.OutOfRangeMessage) {}

        public InvalidRangeException(InteractionContext ic, object min, object max, string message)
            : base(ic, message) {
            Min = min;
            Max = max;
        }

        public object Min { get; private set; }

        public object Max { get; private set; }
    }
}