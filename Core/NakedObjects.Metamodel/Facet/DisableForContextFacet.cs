// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Utils;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Meta.Facet {
    public class DisableForContextFacet : FacetAbstract, IDisableForContextFacet, IImperativeFacet {
        private readonly MethodInfo method;

        public DisableForContextFacet(MethodInfo method, ISpecification holder)
            : base(typeof (IDisableForContextFacet), holder) {
            this.method = method;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        #region IDisableForContextFacet Members

        public virtual string Disables(InteractionContext ic) {
            INakedObject target = ic.Target;
            return DisabledReason(target);
        }

        public virtual DisabledException CreateExceptionFor(InteractionContext ic) {
            return new DisabledException(ic, Disables(ic));
        }

        public string DisabledReason(INakedObject nakedObject) {
            return (string) InvokeUtils.Invoke(method, nakedObject);
        }
        #endregion

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}