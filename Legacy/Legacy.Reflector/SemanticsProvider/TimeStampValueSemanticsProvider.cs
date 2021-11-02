// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using Legacy.NakedObjects.Application.ValueHolder;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.SemanticsProvider;

namespace Legacy.Reflector.SemanticsProvider {
    [Serializable]
    public sealed class TimeStampValueSemanticsProvider : ValueSemanticsProviderAbstract<TimeStamp>, IDateValueFacet {
        private const bool EqualByContent = false;
        private const bool Immutable = false;
        private const int TypicalLengthConst = 18;
        private static readonly TimeStamp DefaultValueConst = new();

        public TimeStampValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        // inject for testing 
        public static DateTime? TestDateTime { get; set; }

        public static Type Type => typeof(IDateValueFacet);

        public static Type AdaptedType => typeof(TimeStamp);

        #region IDateValueFacet Members

        public DateTime DateValue(INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.GetDomainObject<TimeStamp>().dateValue().GetValueOrDefault();
        }

        #endregion

        public static bool IsAdaptedType(Type type) => type == typeof(TimeStamp);

        protected override string DoEncode(TimeStamp obj) {
            var date = obj;
            return date.asEncodedString();
        }

        protected override TimeStamp DoParse(string entry) {
            var dateString = entry.Trim();
            try {
                var d = new TimeStamp();
                d.parseUserEntry(entry);
                return d;
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(dateString));
            }
        }

        protected override TimeStamp DoParseInvariant(string entry) => DoParse(entry);

        protected override string GetInvariantString(TimeStamp obj) => obj.asEncodedString();

        protected override TimeStamp DoRestore(string data) {
            var d = new TimeStamp();
            d.restoreFromEncodedString(data);
            return d;
        }

        protected override string TitleStringWithMask(string mask, TimeStamp value) => value.titleString();

        private static DateTime Now() => TestDateTime ?? DateTime.Now;
    }

    // Copyright (c) Naked Objects Group Ltd.
}