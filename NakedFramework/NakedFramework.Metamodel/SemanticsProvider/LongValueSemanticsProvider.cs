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
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class LongValueSemanticsProvider : ValueSemanticsProviderAbstract<long>, ILongValueFacet {
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 20;
        private const long DefaultValueConst = 0;

        public LongValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        public static Type Type => typeof(ILongValueFacet);

        public static Type AdaptedType => typeof(long);

        #region ILongValueFacet Members

        public long LongValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<long>();

        #endregion

        public static bool IsAdaptedType(Type type) => type == typeof(long);

        protected override long DoParse(string entry) {
            try {
                return long.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, long.MinValue, long.MaxValue));
            }
        }

        protected override long DoParseInvariant(string entry) => long.Parse(entry, CultureInfo.InvariantCulture);

        protected override string GetInvariantString(long obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override string TitleStringWithMask(string mask, long value) => value.ToString(mask);

        protected override string DoEncode(long obj) => obj.ToString("G", CultureInfo.InvariantCulture);

        protected override long DoRestore(string data) => long.Parse(data, CultureInfo.InvariantCulture);

        public override string ToString() => "LongAdapter: ";
    }
}