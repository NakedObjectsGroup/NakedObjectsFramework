// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class LoadedCallbackFacetViaMethod : LoadedCallbackFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        [field: NonSerialized] private Action<object> loadedDelegate;

        public LoadedCallbackFacetViaMethod(MethodInfo method, ISpecification holder)
            : base(holder) {
            this.method = method;
            loadedDelegate = DelegateUtils.CreateCallbackDelegate(method);
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() =>
            (tgt, p) => {
                loadedDelegate(tgt);
                return null;
            };

        #endregion

        public override void Invoke(INakedObjectAdapter nakedObjectAdapter, ISession session, ILifecycleManager lifecycleManager, IMetamodelManager metamodelManager) => loadedDelegate(nakedObjectAdapter.GetDomainObject());

        protected override string ToStringValues() => $"method={method}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => loadedDelegate = DelegateUtils.CreateCallbackDelegate(method);
    }

    // Copyright (c) Naked Objects Group Ltd.
}