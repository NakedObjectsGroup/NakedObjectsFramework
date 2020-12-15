// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class EnumValueSemanticsProvider<T> : ValueSemanticsProviderAbstract<T>, IEnumValueFacet {
        private const bool EqualBycontent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 11;

        public EnumValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualBycontent, GetDefault(), spec) { }

        public static Type Type => typeof(IEnumValueFacet);

        public static Type AdaptedType => typeof(T);

        #region IEnumValueFacet Members

        public string IntegralValue(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter.Object is T || TypeUtils.IsIntegralValueForEnum(nakedObjectAdapter.Object)) {
                return Convert.ChangeType(nakedObjectAdapter.Object, Enum.GetUnderlyingType(typeof(T)))?.ToString();
            }

            return null;
        }

        #endregion

        private static bool IsEnumSubtype() => TypeUtils.IsEnum(AdaptedType) && !(AdaptedType == typeof(Enum));

        private static T GetDefault() {
            // default(T) for an enum just returns 0 - but that's 
            // not necessarily a valid value for the enum - and that breaks serialization
            // return the first value. 
            // Value could be an EnumDataType rather than Enum - so check first
            if (IsEnumSubtype()) {
                var values = Enum.GetValues(AdaptedType);
                if (values.Length > 0) {
                    return (T) values.GetValue(0);
                }
            }

            return default;
        }

        public static bool IsAdaptedType(Type type) => type == AdaptedType;

        protected override T DoParse(string entry) {
            try {
                return (T) Enum.Parse(typeof(T), entry);
            }
            catch (ArgumentException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException oe) {
                throw new InvalidEntryException(oe.Message);
            }
        }

        protected override T DoParseInvariant(string entry) => (T) Enum.Parse(typeof(T), entry);

        protected override string GetInvariantString(T obj) => obj.ToString();

        protected override string TitleString(T obj) => NameUtils.NaturalName(obj.ToString());

        protected override string TitleStringWithMask(string mask, T value) => TitleString(value);

        protected override string DoEncode(T obj) => $"{obj.GetType().FullName}:{obj}";

        protected override T DoRestore(string data) {
            var typeAndValue = data.Split(':');
            return (T) Enum.Parse(TypeUtils.GetType(typeAndValue[0]), typeAndValue[1]);
        }

        public override string ToString() => "EnumAdapter: ";
    }
}