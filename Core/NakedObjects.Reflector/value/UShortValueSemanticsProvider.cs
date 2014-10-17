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
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Value {
    public class UShortValueSemanticsProvider : ValueSemanticsProviderAbstract<ushort>, IPropertyDefaultFacet {
        private const ushort defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 5;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public UShortValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public UShortValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, spec) {}

        public static Type Type {
            get { return typeof (IShortValueFacet); }
        }


        public static Type AdaptedType {
            get { return typeof (ushort); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (ushort);
        }


        protected override ushort DoParse(string entry) {
            try {
                return ushort.Parse(entry, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, ushort.MinValue, ushort.MaxValue));
            }
        }

        protected override ushort DoParseInvariant(string entry) {
            return ushort.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(ushort obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, ushort value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(ushort obj) {
            return obj.ToString();
        }

        protected override ushort DoRestore(string data) {
            return ushort.Parse(data);
        }


        public ushort UnsignedShortValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<ushort>();
        }


        public override string ToString() {
            return "UShortAdapter: ";
        }
    }
}