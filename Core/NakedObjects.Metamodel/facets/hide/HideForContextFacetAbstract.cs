// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets.Hide {
    public abstract class HideForContextFacetAbstract : FacetAbstract, IHideForContextFacet {
        protected HideForContextFacetAbstract(ISpecification holder)
            : base(Type, holder) {}

        public static Type Type {
            get { return typeof (IHideForContextFacet); }
        }

        #region IHideForContextFacet Members

        public virtual string Hides(InteractionContext ic, ILifecycleManager persistor) {
            return HiddenReason(ic.Target);
        }

        public virtual HiddenException CreateExceptionFor(InteractionContext ic, ILifecycleManager persistor) {
            return new HiddenException(ic, Hides(ic, persistor));
        }

        public abstract string HiddenReason(INakedObject nakedObject);

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}