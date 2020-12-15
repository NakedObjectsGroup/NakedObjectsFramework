// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class HideForSessionFacetNone : HideForSessionFacetAbstract {
        public HideForSessionFacetNone(ISpecification holder)
            : base(holder) { }

        public override bool IsNoOp => true;

        /// <summary>
        ///     Always returns <c>null</c>
        /// </summary>
        public override string HiddenReason(ISession session, INakedObjectAdapter target, ILifecycleManager lifecycleManager, IMetamodelManager manager) => null;
    }

    // Copyright (c) Naked Objects Group Ltd.
}