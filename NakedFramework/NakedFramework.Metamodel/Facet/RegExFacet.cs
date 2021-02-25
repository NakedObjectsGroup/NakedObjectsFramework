// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.interactions;
using NakedFramework.Architecture.Spec;
using NakedFramework.Metamodel.Exception;

[assembly:InternalsVisibleTo("NakedObjects.Reflector.Test")]
[assembly: InternalsVisibleTo("NakedFunctions.Reflector.Test")]

namespace NakedFramework.Metamodel.Facet {
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

        public RegExFacet(string validation, bool caseSensitive, ISpecification holder)
            : base(typeof(IRegExFacet), holder)
        {
            ValidationPattern = validation;
            Pattern = new Regex(validation, PatternFlags);
            IsCaseSensitive = caseSensitive;
        }

        private RegexOptions PatternFlags => !IsCaseSensitive ? RegexOptions.IgnoreCase : RegexOptions.None;

        internal string ValidationPattern { get; }

        private string FormatPattern { get; }

        internal bool IsCaseSensitive { get; }

        #region IRegExFacet Members

        public Regex Pattern { get; private set; }

        public string Format(string text) =>
            text == null
                ? NakedObjects.Resources.NakedObjects.EmptyString
                : string.IsNullOrEmpty(FormatPattern)
                    ? text
                    : Pattern.Replace(text, FormatPattern);

        public bool DoesNotMatch(string text) => text == null || !Pattern.IsMatch(text);

        public string FailureMessage { get; }

        public string Invalidates(IInteractionContext ic) {
            var proposedArgument = ic.ProposedArgument;
            if (proposedArgument == null) {
                return null;
            }

            var titleString = proposedArgument.TitleString();
            if (!DoesNotMatch(titleString)) {
                return null;
            }

            return FailureMessage ?? NakedObjects.Resources.NakedObjects.InvalidEntry;
        }

        public System.Exception CreateExceptionFor(IInteractionContext ic) => new InvalidRegExException(ic, FormatPattern, ValidationPattern, IsCaseSensitive, Invalidates(ic));

        #endregion

        protected override string ToStringValues() => Pattern.ToString();
    }

    // Copyright (c) Naked Objects Group Ltd.
}