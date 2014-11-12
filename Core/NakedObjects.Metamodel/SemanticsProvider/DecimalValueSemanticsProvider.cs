// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public class DecimalValueSemanticsProvider : ValueSemanticsProviderAbstract<decimal>, IPropertyDefaultFacet {
        private const decimal DefaultValueConst = 0;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 18;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public DecimalValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public DecimalValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) {}

        public static Type Type {
            get { return typeof (IDecimalValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (decimal); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return DefaultValueConst;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (decimal);
        }


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

        protected override decimal DoParseInvariant(string entry) {
            return decimal.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(decimal obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, decimal value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(decimal obj) {
            return obj.ToString();
        }

        protected override decimal DoRestore(string data) {
            return decimal.Parse(data);
        }


        public decimal DecimalValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<decimal>();
        }


        public override string ToString() {
            return "DecimalAdapter: ";
        }
    }
}