// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using NakedFramework.Architecture.Adapter;
using NakedFunctions.Reflector.Utils;
using NakedObjects;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;

namespace NakedFunctions.Reflector.Facet {
    [Serializable]
    public sealed class PersistingCallbackFacetViaFunction : PersistingCallbackFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        public PersistingCallbackFacetViaFunction(MethodInfo method, ISpecification holder) : base(holder) => this.method = method;

        public override void Invoke(INakedObjectAdapter nakedObjectAdapter,
                                    INakedObjectsFramework framework) {
            // do nothing should always be called via Invoke and Return
        }

        public override object InvokeAndReturn(INakedObjectAdapter nakedObjectAdapter,
                                               INakedObjectsFramework framework) =>
            method.Invoke(null, method.GetParameterValues(nakedObjectAdapter, framework));

        protected override string ToStringValues() => $"method={method}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) { }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => null;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}