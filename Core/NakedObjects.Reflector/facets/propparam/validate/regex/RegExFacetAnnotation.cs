// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Text.RegularExpressions;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.RegEx;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.RegEx {
    public class RegExFacetAnnotation : RegExFacetAbstract, IRegExFacet {
        public RegExFacetAnnotation(string validation, string format, bool caseSensitive, string message, ISpecification holder)
            : base(validation, format, caseSensitive, message, holder) {
            Pattern = new Regex(ValidationPattern, PatternFlags);
        }

        private RegexOptions PatternFlags {
            get { return !IsCaseSensitive ? RegexOptions.IgnoreCase : RegexOptions.None; }
        }

        #region IRegExFacet Members

        public Regex Pattern { get; private set; }

        public override string Format(string text) {
            if (text == null) {
                return Resources.NakedObjects.EmptyString;
            }
            return string.IsNullOrEmpty(FormatPattern) ? text : Pattern.Replace(text, FormatPattern);
        }

        public override bool DoesNotMatch(string text) {
            if (text == null) {
                return true;
            }
            return !Pattern.IsMatch(text);
        }

        #endregion

        protected override string ToStringValues() {
            return Pattern.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}