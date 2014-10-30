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
using NakedObjects.Capabilities;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    public class SbyteValueSemanticsProvider : ValueSemanticsProviderAbstract<sbyte>, IPropertyDefaultFacet {
        private const sbyte defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 3; // include sign 


        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public SbyteValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public SbyteValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, spec) {}

        public static Type Type {
            get { return typeof (ISbyteValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (sbyte); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (sbyte);
        }


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

        protected override sbyte DoParseInvariant(string entry) {
            return sbyte.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(sbyte obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, sbyte value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(sbyte obj) {
            return obj.ToString("G");
        }

        protected override sbyte DoRestore(string data) {
            return sbyte.Parse(data);
        }


        public sbyte ByteValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<sbyte>();
        }


        public override string ToString() {
            return "SByteAdapter: ";
        }
    }
}