// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Text.RegularExpressions;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class RegExFacet : FacetAbstract, IRegExFacet {
        public RegExFacet(string validation, string format, bool caseSensitive, string message, ISpecification holder)
            : base(typeof(IRegExFacet), holder) {
            ValidationPattern = validation;
            Pattern = new Regex(validation, PatternFlags);
            FormatPattern = format;
            IsCaseSensitive = caseSensitive;
            FailureMessage = message;
        }

        private RegexOptions PatternFlags => !IsCaseSensitive ? RegexOptions.IgnoreCase : RegexOptions.None;

        public string ValidationPattern { get; }

        public string FormatPattern { get; }

        public bool IsCaseSensitive { get; }

        #region IRegExFacet Members

        public Regex Pattern { get; private set; }

        public string Format(string text) {
            if (text == null) {
                return Resources.NakedObjects.EmptyString;
            }

            return string.IsNullOrEmpty(FormatPattern) ? text : Pattern.Replace(text, FormatPattern);
        }

        public bool DoesNotMatch(string text) {
            if (text == null) {
                return true;
            }

            return !Pattern.IsMatch(text);
        }

        public string FailureMessage { get; }

        public string Invalidates(IInteractionContext ic) {
            INakedObjectAdapter proposedArgument = ic.ProposedArgument;
            if (proposedArgument == null) {
                return null;
            }

            string titleString = proposedArgument.TitleString();
            if (!DoesNotMatch(titleString)) {
                return null;
            }

            return FailureMessage ?? Resources.NakedObjects.InvalidEntry;
        }

        public Exception CreateExceptionFor(IInteractionContext ic) {
            return new InvalidRegExException(ic, FormatPattern, ValidationPattern, IsCaseSensitive, Invalidates(ic));
        }

        #endregion

        protected override string ToStringValues() {
            return Pattern.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}