// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class TimeValueSemanticsProvider : ValueSemanticsProviderAbstract<TimeSpan>, ITimeValueFacet {
        private const bool EqualByContent = false;
        private const bool Immutable = false;
        private const int TypicalLengthConst = 6;
        private static readonly TimeSpan DefaultValueConst = new TimeSpan();

        public TimeValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        public static Type Type => typeof(ITimeValueFacet);

        public static Type AdaptedType => typeof(TimeSpan);

        #region ITimeValueFacet Members

        public TimeSpan TimeValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<TimeSpan>();

        #endregion

        public static bool IsAdaptedType(Type type) => type == typeof(TimeSpan);

        protected override string DoEncode(TimeSpan time) => time.ToString();

        protected override TimeSpan DoParse(string entry) {
            var dateString = entry.Trim();
            try {
                return DateTime.Parse(dateString).TimeOfDay;
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(dateString));
            }
        }

        protected override TimeSpan DoParseInvariant(string entry) => TimeSpan.Parse(entry, CultureInfo.InvariantCulture);

        protected override string GetInvariantString(TimeSpan obj) => obj.ToString(null, CultureInfo.InvariantCulture);

        protected override TimeSpan DoRestore(string data) => TimeSpan.Parse(data);

        protected override string TitleString(TimeSpan obj) => DateTime.Today.Add(obj).ToShortTimeString();

        protected override string TitleStringWithMask(string mask, TimeSpan obj) => DateTime.Today.Add(obj).ToString(mask);
    }

    // Copyright (c) Naked Objects Group Ltd.
}