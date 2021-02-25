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
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.interactions;
using NakedFramework.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class HideForContextFacet : FacetAbstract, IHideForContextFacet, IImperativeFacet {
        private readonly ILogger<HideForContextFacet> logger;
        private readonly MethodInfo method;

        [field: NonSerialized] private Func<object, object[], object> methodDelegate;

        public HideForContextFacet(MethodInfo method, ISpecification holder, ILogger<HideForContextFacet> logger)
            : base(typeof(IHideForContextFacet), holder) {
            this.method = method;
            this.logger = logger;
            methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
        }

        #region IHideForContextFacet Members

        public string Hides(IInteractionContext ic) => HiddenReason(ic.Target, ic.Framework);

        public Exception CreateExceptionFor(IInteractionContext ic) => new HiddenException(ic, Hides(ic));

        public string HiddenReason(INakedObjectAdapter nakedObjectAdapter, INakedObjectsFramework framework) {
            if (nakedObjectAdapter == null) {
                return null;
            }

            var isHidden = (bool) methodDelegate(nakedObjectAdapter.GetDomainObject(), new object[] { });
            return isHidden ? Resources.NakedObjects.Hidden : null;
        }

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

        #endregion

        protected override string ToStringValues() => $"method={method}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    // Copyright (c) Naked Objects Group Ltd.
}