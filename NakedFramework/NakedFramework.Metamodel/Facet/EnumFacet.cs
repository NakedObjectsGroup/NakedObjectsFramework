// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Core.Configuration;
using NakedFramework.Metamodel.Serialization;

namespace NakedFramework.Metamodel.Facet;

[Serializable]
public sealed class EnumFacet : FacetAbstract, IEnumFacet, IMarkerFacet {
    private readonly TypeSerializationWrapper typeWrapper;

    public EnumFacet(Type typeOfEnum) => typeWrapper = new TypeSerializationWrapper(typeOfEnum, ReflectorDefaults.JitSerialization);

    public Type TypeOfEnum => typeWrapper.Type;

    public override Type FacetType => typeof(IEnumFacet);

    private string ToDisplayName(string enumName) => NameUtils.NaturalName(Enum.Parse(TypeOfEnum, enumName).ToString());

    #region Nested type: EnumNameComparer

    private class EnumNameComparer : IComparer<string> {
        private readonly EnumFacet facet;

        public EnumNameComparer(EnumFacet facet) => this.facet = facet;

        #region IComparer<string> Members

        public int Compare(string x, string y) => string.Compare(facet.ToDisplayName(x), facet.ToDisplayName(y), StringComparison.CurrentCulture);

        #endregion
    }

    #endregion

    #region IEnumFacet Members

    public object[] GetChoices(INakedObjectAdapter inObjectAdapter) => Enum.GetNames(TypeOfEnum).OrderBy(s => s, new EnumNameComparer(this)).Select(s => Enum.Parse(TypeOfEnum, s)).ToArray();

    public object[] GetChoices(INakedObjectAdapter inObjectAdapter, object[] choiceValues) => choiceValues.Select(o => Enum.Parse(TypeOfEnum, o.ToString())).ToArray();

    public string GetTitle(INakedObjectAdapter inObjectAdapter) => ToDisplayName(inObjectAdapter.Object.ToString());

    #endregion
}

// Copyright (c) Naked Objects Group Ltd.