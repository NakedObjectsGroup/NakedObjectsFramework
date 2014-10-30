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
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    public class BooleanValueSemanticsProvider : ValueSemanticsProviderAbstract<bool>, IBooleanValueFacet {
        private const bool DefaultValueConst = false;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 5;

        public BooleanValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) {}

        /// <summary>
        ///     Required because implementation of <see cref="IParser{T}" /> and <see cref="IEncoderDecoder{T}" />.
        /// </summary>
        public BooleanValueSemanticsProvider(IObjectSpecImmutable spec)
            : this(spec, null) {}

        private static Type Type {
            get { return typeof (IBooleanValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (bool); }
        }

        #region IBooleanValueFacet Members

        public bool IsSet(INakedObject nakedObject) {
            if (!nakedObject.Exists()) {
                return false;
            }
            return nakedObject.GetDomainObject<bool>();
        }


        public void Reset(INakedObject nakedObject) {
            nakedObject.ReplacePoco(false);
        }

        public void Set(INakedObject nakedObject) {
            nakedObject.ReplacePoco(true);
        }

        public void Toggle(INakedObject nakedObject) {
            bool newValue = !(bool) nakedObject.Object;
            nakedObject.ReplacePoco(newValue);
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == AdaptedType;
        }


        protected override bool DoParse(string entry) {
            if ("true".StartsWith(entry.ToLower())) {
                return true;
            }
            if ("false".StartsWith(entry.ToLower())) {
                return false;
            }
            throw new InvalidEntryException(string.Format(Resources.NakedObjects.NotALogical, entry));
        }

        protected override bool DoParseInvariant(string entry) {
            return bool.Parse(entry);
        }

        protected override string GetInvariantString(bool obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string DoEncode(bool obj) {
            return obj ? "T" : "F";
        }

        protected override bool DoRestore(string data) {
            if (data.Length != 1) {
                throw new InvalidDataException(string.Format(Resources.NakedObjects.InvalidLogicalLength, data.Length));
            }
            switch (data[0]) {
                case 'T':
                    return true;
                case 'F':
                    return false;
                default:
                    throw new InvalidDataException(string.Format(Resources.NakedObjects.InvalidLogicalType, data[0]));
            }
        }
    }
}