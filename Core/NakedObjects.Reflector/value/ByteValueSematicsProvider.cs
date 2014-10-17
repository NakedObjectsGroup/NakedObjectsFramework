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
    public class ByteValueSemanticsProvider : ValueSemanticsProviderAbstract<byte>, IPropertyDefaultFacet, IByteValueFacet {
        private const byte defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 3;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public ByteValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public ByteValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, spec) {}

        public static Type Type {
            get { return typeof (IByteValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (byte); }
        }

        #region IByteValueFacet Members

        public byte ByteValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<byte>();
        }

        #endregion

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (byte);
        }


        protected override byte DoParse(string entry) {
            try {
                return byte.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, byte.MinValue, byte.MaxValue));
            }
        }

        protected override byte DoParseInvariant(string entry) {
            return byte.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(byte obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, byte value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(byte obj) {
            return obj.ToString();
        }

        protected override byte DoRestore(string data) {
            return byte.Parse(data);
        }

        public override string ToString() {
            return "ByteAdapter: ";
        }
    }
}