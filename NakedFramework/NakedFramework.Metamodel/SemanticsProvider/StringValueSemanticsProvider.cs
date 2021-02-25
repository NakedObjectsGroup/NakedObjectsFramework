// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Spec;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class StringValueSemanticsProvider : ValueSemanticsProviderAbstract<string>, IStringValueFacet {
        private const string DefaultValueConst = null;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 25;

        public StringValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        public static Type Type => typeof(IStringValueFacet);

        public static Type AdaptedType => typeof(string);

        #region IStringValueFacet Members

        public string StringValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<string>();

        #endregion

        public static bool IsAdaptedType(Type type) => type == typeof(string);

        protected override string DoParse(string entry) => entry.Trim().Equals("") ? null : entry;

        protected override string DoParseInvariant(string entry) => entry;

        protected override string GetInvariantString(string obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override string DoEncode(string obj) {
            var text = obj;
            return text.Equals("NULL") || IsEscaped(text) ? EscapeText(text) : text;
        }

        protected override string DoRestore(string data) => IsEscaped(data) ? data.Substring(1) : data;

        private static bool IsEscaped(string text) => text.StartsWith("/");

        private static string EscapeText(string text) => $"/{text}";
    }
}