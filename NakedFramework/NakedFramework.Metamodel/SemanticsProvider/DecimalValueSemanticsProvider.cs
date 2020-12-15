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
    public sealed class DecimalValueSemanticsProvider : ValueSemanticsProviderAbstract<decimal>, IDecimalValueFacet {
        private const decimal DefaultValueConst = 0;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 18;

        public DecimalValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        public static Type Type => typeof(IDecimalValueFacet);

        public static Type AdaptedType => typeof(decimal);

        #region IDecimalValueFacet Members

        public decimal DecimalValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<decimal>();

        #endregion

        public static bool IsAdaptedType(Type type) => type == typeof(decimal);

        protected override decimal DoParse(string entry) {
            try {
                return decimal.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, decimal.MinValue, decimal.MaxValue));
            }
        }

        protected override decimal DoParseInvariant(string entry) => decimal.Parse(entry, CultureInfo.InvariantCulture);

        protected override string GetInvariantString(decimal obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override string TitleStringWithMask(string mask, decimal value) => value.ToString(mask);

        protected override string DoEncode(decimal obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override decimal DoRestore(string data) => decimal.Parse(data, CultureInfo.InvariantCulture);

        public override string ToString() => "DecimalAdapter: ";
    }
}