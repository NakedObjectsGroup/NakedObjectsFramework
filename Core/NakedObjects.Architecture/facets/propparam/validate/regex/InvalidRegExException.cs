// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.RegEx {
    /// <summary>
    ///     The interaction is invalid because the input value does not match the specified RegEx.
    /// </summary>
    public class InvalidRegExException : InvalidException {
        private readonly bool caseSensitive;
        private readonly string format;
        private readonly string validation;

        public InvalidRegExException(InteractionContext ic, string format, string validation, bool caseSensitive)
            : this(ic, format, validation, caseSensitive, Resources.NakedObjects.PatternMessage) {}

        public InvalidRegExException(InteractionContext ic, string format, string validation, bool caseSensitive, string message)
            : base(ic, message) {
            this.format = format;
            this.validation = validation;
            this.caseSensitive = caseSensitive;
        }

        public virtual string FormatPattern {
            get { return format; }
        }

        public virtual string ValidationPattern {
            get { return validation; }
        }

        public virtual bool IsCaseSensitive {
            get { return caseSensitive; }
        }
    }
}