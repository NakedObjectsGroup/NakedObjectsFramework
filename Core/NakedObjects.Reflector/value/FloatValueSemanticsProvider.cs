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
    public class FloatValueSemanticsProvider : ValueSemanticsProviderAbstract<float>, IPropertyDefaultFacet {
        private const float defaultValue = 0;
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLenth = 12;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public FloatValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public FloatValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLenth, immutable, equalByContent, defaultValue, spec) {}

        public static Type Type {
            get { return typeof (IFloatingPointValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (float); }
        }

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        public static bool IsAdaptedType(Type type) {
            return type == typeof (float);
        }


        protected override float DoParse(string entry) {
            try {
                return float.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, float.MinValue, float.MaxValue));
            }
        }

        protected override float DoParseInvariant(string entry) {
            return float.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(float obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, float value) {
            return value.ToString(mask);
        }


        protected override string DoEncode(float obj) {
            return obj.ToString("G");
        }

        protected override float DoRestore(string data) {
            return float.Parse(data);
        }


        public float FloatValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<float>();
        }


        public override string ToString() {
            return "FloatAdapter: ";
        }
    }
}