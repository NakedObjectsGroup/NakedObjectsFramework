// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class BooleanValueSemanticsProvider : ValueSemanticsProviderAbstract<bool>, IBooleanValueFacet {
        private const bool DefaultValueConst = false;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 5;

        public BooleanValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        private static Type Type => typeof(IBooleanValueFacet);

        public static Type AdaptedType => typeof(bool);

        #region IBooleanValueFacet Members

        public bool IsSet(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.Exists() && nakedObjectAdapter.GetDomainObject<bool>();

        public void Reset(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ReplacePoco(false);

        public void Set(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter.ReplacePoco(true);

        public void Toggle(INakedObjectAdapter nakedObjectAdapter) {
            var newValue = !(bool) nakedObjectAdapter.Object;
            nakedObjectAdapter.ReplacePoco(newValue);
        }

        #endregion

        public static bool IsAdaptedType(Type type) => type == AdaptedType;

        protected override bool DoParse(string entry) {
            if ("true".StartsWith(entry.ToLower())) {
                return true;
            }

            if ("false".StartsWith(entry.ToLower())) {
                return false;
            }

            throw new InvalidEntryException(string.Format(Resources.NakedObjects.NotALogical, entry));
        }

        protected override bool DoParseInvariant(string entry) => bool.Parse(entry);

        protected override string GetInvariantString(bool obj) => obj.ToString(CultureInfo.InvariantCulture);

        protected override string DoEncode(bool obj) => obj ? "T" : "F";

        protected override bool DoRestore(string data) {
            if (data.Length != 1) {
                throw new InvalidDataException(string.Format(Resources.NakedObjects.InvalidLogicalLength, data.Length));
            }

            return data[0] switch {
                'T' => true,
                'F' => false,
                _ => throw new InvalidDataException(string.Format(Resources.NakedObjects.InvalidLogicalType, data[0]))
            };
        }
    }
}