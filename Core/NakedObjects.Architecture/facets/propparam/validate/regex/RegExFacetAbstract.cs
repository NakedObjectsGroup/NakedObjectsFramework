// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.RegEx {
    public abstract class RegExFacetAbstract : MultipleValueFacetAbstract {
        private readonly string failureMessage;
        private readonly string formatPattern;
        private readonly bool isCaseSensitive;
        private readonly string validationPattern;

        protected RegExFacetAbstract(string validation, string format, bool caseSensitive, string failureMessage, IFacetHolder holder)
            : base(Type, holder) {
            validationPattern = validation;
            formatPattern = format;
            isCaseSensitive = caseSensitive;
            this.failureMessage = failureMessage;
        }

        public static Type Type {
            get { return typeof (IRegExFacet); }
        }


        public string ValidationPattern {
            get { return validationPattern; }
        }

        public virtual string FailureMessage {
            get { return failureMessage; }
        }

        public string FormatPattern {
            get { return formatPattern; }
        }

        public bool IsCaseSensitive {
            get { return isCaseSensitive; }
        }


        public virtual string Invalidates(InteractionContext ic) {
            INakedObject proposedArgument = ic.ProposedArgument;
            if (proposedArgument == null) {
                return null;
            }
            string titleString = proposedArgument.TitleString();
            if (!DoesNotMatch(titleString)) {
                return null;
            }

            return failureMessage ?? Resources.NakedObjects.InvalidEntry;
        }


        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidRegExException(ic, FormatPattern, ValidationPattern, IsCaseSensitive, Invalidates(ic));
        }

        public abstract bool DoesNotMatch(string param1);
        public abstract string Format(string param1);
    }
}