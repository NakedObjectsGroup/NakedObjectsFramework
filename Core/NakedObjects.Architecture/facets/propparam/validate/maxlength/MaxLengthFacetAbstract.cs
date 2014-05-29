// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets.Propparam.Validate.MaxLength {
    public abstract class MaxLengthFacetAbstract : SingleIntValueFacetAbstract, IMaxLengthFacet {
        protected MaxLengthFacetAbstract(int intValue, IFacetHolder holder)
            : base(Type, holder, intValue) {}

        public static Type Type {
            get { return typeof (IMaxLengthFacet); }
        }

        #region IMaxLengthFacet Members

        /// <summary>
        ///     Whether the provided argument exceeds the <see cref="SingleIntValueFacetAbstract.Value" /> maximum length}.
        /// </summary>
        public virtual bool Exceeds(INakedObject nakedObject) {
            string str = UnwrapString(nakedObject);
            if (str == null) {
                return false;
            }
            int maxLength = Value;
            return maxLength != 0 && str.Length > maxLength;
        }

        public virtual string Invalidates(InteractionContext ic) {
            INakedObject proposedArgument = ic.ProposedArgument;
            if (!Exceeds(proposedArgument)) {
                return null;
            }
            return string.Format(Resources.NakedObjects.MaximumLengthMismatch, Value);
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new InvalidMaxLengthException(ic, Value, Invalidates(ic));
        }

        #endregion

        protected override string ToStringValues() {
            return Value == 0 ? "unlimited" : Value.ToString();
        }
    }
}