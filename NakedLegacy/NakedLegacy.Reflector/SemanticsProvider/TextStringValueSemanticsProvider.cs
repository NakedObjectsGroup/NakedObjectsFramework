// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.SemanticsProvider;
using NakedLegacy.Types;

namespace NakedLegacy.Reflector.SemanticsProvider;

[Serializable]
public sealed class TextStringValueSemanticsProvider : ValueSemanticsProviderAbstract<TextString>, IStringValueFacet {
    private const TextString DefaultValueConst = null;
    private const bool Immutable = true;
    private const int TypicalLengthConst = 25;

    public TextStringValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
        : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, DefaultValueConst, spec) { }

    public static Type Type => typeof(IStringValueFacet);

    public static Type AdaptedType => typeof(TextString);

    #region IStringValueFacet Members

    public string StringValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<TextString>().Text;

    #endregion

    public static bool IsAdaptedType(Type type) => type == AdaptedType;

    protected override TextString DoParse(string entry) => entry.Trim().Equals("") ? null : new TextString(entry);

    protected override TextString DoParseInvariant(string entry) => new(entry);

    protected override string GetInvariantString(TextString textString) => textString.Text;

    protected override string DoEncode(TextString textString) => textString.Text;

    protected override TextString DoRestore(string data) => new TextString(data);

    private static bool IsEscaped(string text) => text.StartsWith("/");

    private static string EscapeText(string text) => $"/{text}";
}