// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    public class ActionDefaultsFacetViaMethod : ActionDefaultsFacetAbstract, IImperativeFacet {
        private readonly MethodInfo actionMethod;
        private readonly MethodInfo method;

        public ActionDefaultsFacetViaMethod(MethodInfo method, ISpecification holder)
            : base(holder) {
            this.method = method;
            var actionInvocationFacet = holder.GetFacet<IActionInvocationFacet>();
            if (actionInvocationFacet is ActionInvocationFacetViaMethod) {
                var facetViaMethod = (ActionInvocationFacetViaMethod) actionInvocationFacet;
                actionMethod = facetViaMethod.GetMethod();
            }
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override Tuple<object, TypeOfDefaultValue> GetDefault(INakedObject nakedObject) {
            // type safety is given by the reflector only identifying methods that match the 
            // parameter type
            return new Tuple<object, TypeOfDefaultValue>(InvokeUtils.Invoke(method, nakedObject), TypeOfDefaultValue.Explicit);
        }

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}