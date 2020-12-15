// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Util;

namespace NakedObjects.Meta.I18N {
    [Serializable]
    public sealed class NamedFacetI18N : NamedFacetAbstract {
        public NamedFacetI18N(string valueString, ISpecification holder)
            : base(valueString, holder) {
            ShortName = TypeNameUtils.GetShortName(valueString);
            CapitalizedName = NameUtils.CapitalizeName(ShortName);
            SimpleName = NameUtils.SimpleName(ShortName);
            NaturalName = NameUtils.NaturalName(ShortName);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}