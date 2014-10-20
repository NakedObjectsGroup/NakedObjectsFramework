// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Exception;
using NakedObjects.Metamodel.Utils;

namespace NakedObjects.Metamodel.Facet {
    public class ActionParameterValidationFacetViaMethod : FacetAbstract, IActionParameterValidationFacet, IImperativeFacet {
        private readonly MethodInfo method;

        public ActionParameterValidationFacetViaMethod(MethodInfo method, int param, ISpecification holder)
            : base(typeof (IActionParameterValidationFacet), holder) {
            this.method = method;
        }

        #region IActionParameterValidationFacet Members

        public virtual string Invalidates(InteractionContext ic) {
            return InvalidReason(ic.Target, ic.ProposedArgument);
        }

        public virtual InvalidException CreateExceptionFor(InteractionContext ic) {
            return new ActionArgumentsInvalidException(ic, Invalidates(ic));
        }

        public string InvalidReason(INakedObject target, INakedObject proposedArgument) {
            return (string) InvokeUtils.Invoke(method, target, new[] {proposedArgument});
        }

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}