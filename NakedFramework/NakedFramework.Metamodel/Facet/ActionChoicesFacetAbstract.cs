// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public abstract class ActionChoicesFacetAbstract : FacetAbstract, IActionChoicesFacet {
        protected ActionChoicesFacetAbstract(ISpecification holder)
            : base(Type, holder) { }

        public static Type Type => typeof(IActionChoicesFacet);

        #region IActionChoicesFacet Members

        public abstract object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues, INakedObjectsFramework framework);
        public abstract (string, IObjectSpecImmutable)[] ParameterNamesAndTypes { get; }
        public abstract bool IsMultiple { get; }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}