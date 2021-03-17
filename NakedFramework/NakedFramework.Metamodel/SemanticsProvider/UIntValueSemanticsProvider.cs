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
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Metamodel.SemanticsProvider {
    [Serializable]
    public sealed class UIntValueSemanticsProvider : ValueSemanticsProviderAbstract<uint>, IUnsignedIntegerValueFacet {
        private const uint DefaultValueConst = 0;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 10;

        public UIntValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        public static Type Type => typeof(IUnsignedIntegerValueFacet);

        public static Type AdaptedType => typeof(uint);

        #region IUnsignedIntegerValueFacet Members

        public uint UnsignedIntegerValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<uint>();

        #endregion

        public static object GetDefault(INakedObjectAdapter inObjectAdapter) => DefaultValueConst;

        public static bool IsAdaptedType(Type type) => type == typeof(uint);

        protected override uint DoParse(string entry) {
            try {
                return uint.Parse(entry, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, uint.MinValue, uint.MaxValue));
            }
        }

        protected override uint DoParseInvariant(string entry) => uint.Parse(entry, CultureInfo.InvariantCulture);

        protected override string GetInvariantString(uint obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override string TitleStringWithMask(string mask, uint value) => value.ToString(mask);

        protected override string DoEncode(uint obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override uint DoRestore(string data) => uint.Parse(data, CultureInfo.InvariantCulture);

        public override string ToString() => "UIntAdapter: ";
    }
}