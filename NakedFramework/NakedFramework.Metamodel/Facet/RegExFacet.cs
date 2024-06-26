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
using NakedFramework.Architecture.Interactions;
using NakedFramework.Metamodel.Error;

[assembly: InternalsVisibleTo("NakedObjects.Reflector.Test")]
[assembly: InternalsVisibleTo("NakedFunctions.Reflector.Test")]

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class RegExFacet : FacetAbstract, IRegExFacet {
    [NonSerialized]
    private Regex pattern;

    public RegExFacet(string validation, string format, bool caseSensitive, string message) {
        ValidationPattern = validation;
        FormatPattern = format;
        IsCaseSensitive = caseSensitive;
        FailureMessage = message;
    }

    public RegExFacet(string validation, bool caseSensitive) {
        ValidationPattern = validation;
        IsCaseSensitive = caseSensitive;
    }

    private RegexOptions PatternFlags => !IsCaseSensitive ? RegexOptions.IgnoreCase : RegexOptions.None;

    internal string ValidationPattern { get; }

    private string FormatPattern { get; }

    internal bool IsCaseSensitive { get; }

    public override Type FacetType => typeof(IRegExFacet);

    #region IRegExFacet Members

    public Regex Pattern => pattern ??= new Regex(ValidationPattern, PatternFlags);

    public string Format(string text) =>
        text is null
            ? NakedObjects.Resources.NakedObjects.EmptyString
            : string.IsNullOrEmpty(FormatPattern)
                ? text
                : Pattern.Replace(text, FormatPattern);

    public bool DoesNotMatch(string text) => text is null || !Pattern.IsMatch(text);

    public string FailureMessage { get; }

    public string Invalidates(IInteractionContext ic) {
        var proposedArgument = ic.ProposedArgument;
        if (proposedArgument is null) {
            return null;
        }

        var titleString = proposedArgument.TitleString();
        if (!DoesNotMatch(titleString)) {
            return null;
        }

        return FailureMessage ?? NakedObjects.Resources.NakedObjects.InvalidEntry;
    }

    public Exception CreateExceptionFor(IInteractionContext ic) => new InvalidRegExException(ic, FormatPattern, ValidationPattern, IsCaseSensitive, Invalidates(ic));

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.