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
using NakedFramework.Core.Exception;
using NakedFramework.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class IntValueSemanticsProvider : ValueSemanticsProviderAbstract<int>, IIntegerValueFacet {
        private const int DefaultValueConst = 0;
        private const bool EqualBycontent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 11;

        public IntValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualBycontent, DefaultValueConst, spec) { }

        public static Type Type => typeof(IIntegerValueFacet);

        public static Type AdaptedType => typeof(int);

        #region IIntegerValueFacet Members

        public int IntegerValue(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.GetDomainObject<int>();

        #endregion

        public static bool IsAdaptedType(Type type) => type == typeof(int);

        protected override int DoParse(string entry) {
            try {
                return int.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, int.MinValue, int.MaxValue));
            }
        }

        protected override int DoParseInvariant(string entry) => int.Parse(entry, CultureInfo.InvariantCulture);

        protected override string GetInvariantString(int obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override string TitleStringWithMask(string mask, int value) => value.ToString(mask);

        protected override string DoEncode(int obj) => obj.ToString("G", CultureInfo.InvariantCulture);

        protected override int DoRestore(string data) => int.Parse(data, CultureInfo.InvariantCulture);

        public override string ToString() => "IntAdapter: ";
    }
}