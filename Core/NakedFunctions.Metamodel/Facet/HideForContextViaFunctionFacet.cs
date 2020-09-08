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
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Meta.Facet
{
    [Serializable]
    public sealed class HideForContextViaFunctionFacet : FacetAbstract, IHideForContextFacet, IImperativeFacet {
        private readonly MethodInfo method;
       

        public HideForContextViaFunctionFacet(MethodInfo method, ISpecification holder)
            : base(typeof (IHideForContextFacet), holder) {
            this.method = method;
        }

        #region IHideForContextFacet Members

        public string Hides(IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager)
        {
            INakedObjectAdapter target = ic.Target;
            return HiddenReason(target, ic.Session, ic.Persistor);
        }

        public Exception CreateExceptionFor(IInteractionContext ic, ILifecycleManager lifecycleManager, IMetamodelManager manager)
        {
            return new HiddenException(ic, Hides(ic, lifecycleManager, manager));
        }

        public string HiddenReason(INakedObjectAdapter nakedObjectAdapter, ISession session, IObjectPersistor persistor) {
            if (nakedObjectAdapter == null)
            {
                return null;
            }
            var isHidden = (bool)method.Invoke(null, method.GetParameterValues(nakedObjectAdapter, session, persistor));
            return isHidden ? NakedObjects.Resources.NakedObjects.Hidden : null;
        }

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        public Func<object, object[], object> GetMethodDelegate() {
            return null;
        }

        #endregion

        protected override string ToStringValues() {
            return "method=" + method;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            
        }

     
    }

    // Copyright (c) Naked Objects Group Ltd.
}