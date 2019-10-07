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
    public sealed class DoubleValueSemanticsProvider : ValueSemanticsProviderAbstract<double>, IDoubleFloatingPointValueFacet {
        private const double DefaultValueConst = 0;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 22;

        public DoubleValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        private static Type Type => typeof(IDoubleFloatingPointValueFacet);

        public static Type AdaptedType => typeof(double);

        #region IDoubleFloatingPointValueFacet Members

        public Double DoubleValue(INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.GetDomainObject<double>();
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof(double);
        }

        protected override double DoParse(string entry) {
            try {
                return double.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(Resources.NakedObjects.NotANumber);
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, double.MinValue, double.MaxValue));
            }
        }

        protected override double DoParseInvariant(string entry) {
            return double.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(double obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, double value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(double obj) {
            return obj.ToString("G", CultureInfo.InvariantCulture);
        }

        protected override double DoRestore(string data) {
            return double.Parse(data, CultureInfo.InvariantCulture);
        }

        public override string ToString() {
            return "DoubleAdapter: ";
        }
    }
}