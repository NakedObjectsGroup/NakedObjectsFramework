// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Metamodel.Facet {
    public class EnumFacet : EnumFacetAbstract {
        private readonly EnumNameComparer comparer;
        private readonly Type typeOfEnum;

        public EnumFacet(ISpecification holder, Type typeOfEnum)
            : base(holder) {
            this.typeOfEnum = typeOfEnum;
            comparer = new EnumNameComparer(this);
        }

        private string ToDisplayName(string enumName) {
            return NameUtils.NaturalName(Enum.Parse(typeOfEnum, enumName).ToString());
        }

        public override object[] GetChoices(INakedObject inObject) {
            return Enum.GetNames(typeOfEnum).OrderBy(s => s, comparer).Select(s => Enum.Parse(typeOfEnum, s)).ToArray();
        }

        public override object[] GetChoices(INakedObject inObject, object[] choiceValues) {
            return choiceValues.Select(o => Enum.Parse(typeOfEnum, o.ToString())).ToArray();
        }

        public override string GetTitle(INakedObject inObject) {
            return ToDisplayName(inObject.Object.ToString());
        }

        #region Nested type: EnumNameComparer

        private class EnumNameComparer : IComparer<string> {
            private readonly EnumFacet facet;

            public EnumNameComparer(EnumFacet facet) {
                this.facet = facet;
            }

            #region IComparer<string> Members

            public int Compare(string x, string y) {
                return string.Compare(facet.ToDisplayName(x), facet.ToDisplayName(y));
            }

            #endregion
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}