// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflect.DotNet.Value {
    public class GuidValueSemanticsProvider : ValueSemanticsProviderAbstract<Guid>, IPropertyDefaultFacet {
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 36;
        private static readonly Guid defaultValue = Guid.Empty;

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public GuidValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public GuidValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, spec) {}

        public static Type Type {
            get { return typeof (IGuidValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (Guid); }
        }

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        public static bool IsAdaptedType(Type type) {
            return type == typeof (Guid);
        }


        protected override Guid DoParse(string entry) {
            try {
                return new Guid(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
        }

        protected override Guid DoParseInvariant(string entry) {
            return Guid.Parse(entry);
        }

        protected override string GetInvariantString(Guid obj) {
            return obj.ToString();
        }

        protected override string TitleStringWithMask(string mask, Guid value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(Guid obj) {
            return obj.ToString();
        }

        protected override Guid DoRestore(string data) {
            return new Guid(data);
        }

        public Guid GuidValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<Guid>();
        }


        public override string ToString() {
            return "GuidAdapter: ";
        }
    }
}