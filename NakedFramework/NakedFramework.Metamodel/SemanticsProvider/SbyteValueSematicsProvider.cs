// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class SbyteValueSemanticsProvider : ValueSemanticsProviderAbstract<sbyte>, ISbyteValueFacet {
        private const sbyte DefaultValueConst = 0;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 3; // include sign 

        public SbyteValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        public static Type Type => typeof(ISbyteValueFacet);

        public static Type AdaptedType => typeof(sbyte);

        #region ISbyteValueFacet Members

        public sbyte SByteValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<sbyte>();

        #endregion

        public static bool IsAdaptedType(Type type) => type == typeof(sbyte);

        protected override sbyte DoParse(string entry) {
            try {
                return sbyte.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, sbyte.MinValue, sbyte.MaxValue));
            }
        }

        protected override sbyte DoParseInvariant(string entry) => sbyte.Parse(entry, CultureInfo.InvariantCulture);

        protected override string GetInvariantString(sbyte obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override string TitleStringWithMask(string mask, sbyte value) => value.ToString(mask);

        protected override string DoEncode(sbyte obj) => obj.ToString("G", CultureInfo.InvariantCulture);

        protected override sbyte DoRestore(string data) => sbyte.Parse(data, CultureInfo.InvariantCulture);

        public override string ToString() => "SByteAdapter: ";
    }
}