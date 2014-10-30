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
    public class CharValueSemanticsProvider : ValueSemanticsProviderAbstract<char>, IPropertyDefaultFacet {
        private const char defaultValue = ' ';
        private const bool equalByContent = true;
        private const bool immutable = true;
        private const int typicalLength = 2;


        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public CharValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        public CharValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, typicalLength, immutable, equalByContent, defaultValue, spec) {}

        public static Type Type {
            get { return typeof (ICharValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (char); }
        }

        #region IPropertyDefaultFacet Members

        public object GetDefault(INakedObject inObject) {
            return defaultValue;
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }

        public override object ParseTextEntry(string entry) {
            if (entry == null) {
                throw new ArgumentException();
            }
            if (entry.Equals("")) {
                return null;
            }
            return DoParse(entry);
        }

        protected override char DoParse(string entry) {
            try {
                return char.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
        }

        protected override char DoParseInvariant(string entry) {
            return char.Parse(entry);
        }

        protected override string GetInvariantString(char obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, char value) {
            return value.ToString();
        }

        protected override string DoEncode(char obj) {
            return obj.ToString();
        }

        protected override char DoRestore(string data) {
            return char.Parse(data);
        }


        public char CharValue(INakedObject nakedObject) {
            return nakedObject.GetDomainObject<char>();
        }


        public override string ToString() {
            return "CharAdapter: ";
        }
    }
}