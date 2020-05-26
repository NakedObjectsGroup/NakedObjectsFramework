// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ActionChoicesFacetNone : ActionChoicesFacetAbstract {
        public ActionChoicesFacetNone(ISpecification holder)
            : base(holder) { }

        public override bool IsNoOp => true;

        public override (string, IObjectSpecImmutable)[] ParameterNamesAndTypes => new ValueTuple<string, IObjectSpecImmutable>[] { };

        public override bool IsMultiple => false;

        public override object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) => new object[0];
    }

    // Copyright (c) Naked Objects Group Ltd.
}