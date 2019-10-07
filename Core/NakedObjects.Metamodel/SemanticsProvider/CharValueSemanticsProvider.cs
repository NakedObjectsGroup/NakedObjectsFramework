// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using System.Threading;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class CharValueSemanticsProvider : ValueSemanticsProviderAbstract<char>, ICharValueFacet {
        private const char DefaultValueConst = ' ';
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 2;

        public CharValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) { }

        public static Type Type => typeof(ICharValueFacet);

        public static Type AdaptedType => typeof(char);

        #region ICharValueFacet Members

        public char CharValue(INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.GetDomainObject<char>();
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
            return value.ToString(Thread.CurrentThread.CurrentCulture);
        }

        protected override string DoEncode(char obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override char DoRestore(string data) {
            return char.Parse(data);
        }

        public override string ToString() {
            return "CharAdapter: ";
        }
    }
}