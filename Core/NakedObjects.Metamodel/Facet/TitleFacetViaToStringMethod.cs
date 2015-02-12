// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    internal class TitleFacetViaToStringMethod : TitleFacetAbstract, IImperativeFacet {
        private readonly MethodInfo maskMethod;
        private readonly MethodInfo method;

        public TitleFacetViaToStringMethod(MethodInfo method, ISpecification holder)
            : this(method, null, holder) {}

        public TitleFacetViaToStringMethod(MethodInfo method, MethodInfo maskMethod, ISpecification holder)
            : base(holder) {
            this.method = method;
            this.maskMethod = maskMethod;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override string GetTitle(INakedObject nakedObject, INakedObjectManager nakedObjectManager) {
            return nakedObject.Object.ToString();
        }

        public override string GetTitleWithMask(string mask, INakedObject nakedObject, INakedObjectManager nakedObjectManager) {
            if (maskMethod == null) {
                return GetTitle(nakedObject, nakedObjectManager);
            }
            return (string) maskMethod.Invoke(nakedObject.Object, new object[] {mask});
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}