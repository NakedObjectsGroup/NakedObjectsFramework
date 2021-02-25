// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class TitleFacetViaToStringMethod : TitleFacetAbstract, IImperativeFacet {
        private readonly ILogger<TitleFacetViaToStringMethod> logger;
        private readonly MethodInfo maskMethod;

        [field: NonSerialized] private Func<object, object[], object> maskDelegate;

        public TitleFacetViaToStringMethod(MethodInfo maskMethod, ISpecification holder, ILogger<TitleFacetViaToStringMethod> logger)
            : base(holder) {
            this.maskMethod = maskMethod;
            this.logger = logger;
            maskDelegate = maskMethod == null ? null : LogNull(DelegateUtils.CreateDelegate(maskMethod), logger);
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => maskMethod;

        public Func<object, object[], object> GetMethodDelegate() => maskDelegate;

        #endregion

        public override string GetTitle(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) => nakedObjectAdapter.Object.ToString();

        public override string GetTitleWithMask(string mask, INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            if (maskDelegate != null) {
                return (string) maskDelegate(nakedObjectAdapter.GetDomainObject(), new object[] {mask});
            }

            if (maskMethod != null) {
                return (string) maskMethod.Invoke(nakedObjectAdapter.GetDomainObject(), new object[] {mask});
            }

            return GetTitle(nakedObjectAdapter, framework);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => maskDelegate = maskMethod == null ? null : LogNull(DelegateUtils.CreateDelegate(maskMethod), logger);
    }

    // Copyright (c) Naked Objects Group Ltd.
}