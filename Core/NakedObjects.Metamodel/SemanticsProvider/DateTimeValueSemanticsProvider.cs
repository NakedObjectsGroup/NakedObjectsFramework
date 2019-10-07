// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class DateTimeValueSemanticsProvider : ValueSemanticsProviderAbstract<DateTime>, IDateValueFacet {
        private const bool EqualByContent = false;
        private const bool Immutable = false;
        private const int TypicalLengthConst = 18;
        private static readonly DateTime DefaultValueConst = new DateTime();

        public DateTimeValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        // inject for testing 
        public static DateTime? TestDateTime { get; set; }

        public static Type Type => typeof(IDateValueFacet);

        public static Type AdaptedType => typeof(DateTime);

        #region IDateValueFacet Members

        public DateTime DateValue(INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter == null ? Now() : (DateTime) nakedObjectAdapter.Object;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof(DateTime);
        }

        protected override string DoEncode(DateTime obj) {
            DateTime date = obj;
            return date.ToString("s");
        }

        protected override DateTime DoParse(string entry) {
            string dateString = entry.Trim();
            try {
                return DateTime.Parse(dateString);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(dateString));
            }
        }

        protected override DateTime DoParseInvariant(string entry) {
            return DateTime.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(DateTime obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override DateTime DoRestore(string data) {
            return DateTime.Parse(data);
        }

        protected override string TitleStringWithMask(string mask, DateTime value) {
            return value.ToString(mask);
        }

        private static DateTime Now() {
            return TestDateTime ?? DateTime.Now;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}