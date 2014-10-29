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
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflect.DotNet.Value {
    public class LongValueSemanticsProvider : ValueSemanticsProviderAbstract<long>, IPropertyDefaultFacet {
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 20;
        private const long defaultValue = 0;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public LongValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public LongValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, spec) {}

        public static Type Type {
            get { return typeof (ILongValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (long); }
        }

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        public static bool IsAdaptedType(Type type) {
            return type == typeof (long);
        }


        protected override long DoParse(string entry) {
            try {
                return long.Parse(entry, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, long.MinValue, long.MaxValue));
            }
        }

        protected override long DoParseInvariant(string entry) {
            return long.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(long obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, long value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(long obj) {
            return obj.ToString("G");
        }

        protected override long DoRestore(string data) {
            return long.Parse(data);
        }


        public long LongValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<long>();
        }


        public override string ToString() {
            return "LongAdapter: ";
        }
    }
}