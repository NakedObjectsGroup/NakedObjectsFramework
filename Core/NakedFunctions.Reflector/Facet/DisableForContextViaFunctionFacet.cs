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
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Meta.Facet {
    [Serializable]
    public sealed class DisableForContextViaFunctionFacet : FacetAbstract, IDisableForContextFacet, IImperativeFacet {
        private readonly MethodInfo method;


        public DisableForContextViaFunctionFacet(MethodInfo method, ISpecification holder) : base(typeof(IDisableForContextFacet), holder) => this.method = method;

        protected override string ToStringValues() => $"method={method}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) { }

        #region IDisableForContextFacet Members

        public string Disables(IInteractionContext ic) => DisabledReason(ic.Target, ic.Session, ic.Persistor);

        public Exception CreateExceptionFor(IInteractionContext ic) => new DisabledException(ic, Disables(ic));

        public string DisabledReason(INakedObjectAdapter nakedObjectAdapter,
                                     ISession session,
                                     IObjectPersistor persistor) =>
            (string) method.Invoke(null, method.GetParameterValues(nakedObjectAdapter, session, persistor));

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => null;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}