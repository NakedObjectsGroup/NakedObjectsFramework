// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    internal class ActionInvocationFacetViaMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ActionInvocationFacetViaMethod));
        private readonly MethodInfo actionMethod;
        private readonly IObjectSpecImmutable elementType;
        private readonly bool isQueryOnly;
        private readonly ITypeSpecImmutable onType;
        private readonly int paramCount;
        private readonly IObjectSpecImmutable returnType;

        public ActionInvocationFacetViaMethod(MethodInfo method, ITypeSpecImmutable onType, IObjectSpecImmutable returnType, IObjectSpecImmutable elementType, ISpecification holder, bool isQueryOnly)
            : base(holder) {
            actionMethod = method;
            paramCount = method.GetParameters().Length;
            this.onType = onType;
            this.returnType = returnType;
            this.elementType = elementType;
            this.isQueryOnly = isQueryOnly;
        }

        public override IObjectSpecImmutable ReturnType {
            get { return returnType; }
        }

        public override ITypeSpecImmutable OnType {
            get { return onType; }
        }

        public override IObjectSpecImmutable ElementType {
            get { return elementType; }
        }

        public override bool IsQueryOnly {
            get { return isQueryOnly; }
        }

        #region IImperativeFacet Members

        /// <summary>
        ///     See <see cref="IImperativeFacet" />
        /// </summary>
        public MethodInfo GetMethod() {
            return actionMethod;
        }

        #endregion

        public override INakedObject Invoke(INakedObject inObject, INakedObject[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager) {
            if (parameters.Length != paramCount) {
                Log.Error(actionMethod + " requires " + paramCount + " parameters, not " + parameters.Length);
            }

            object result = InvokeUtils.Invoke(actionMethod, inObject, parameters);
            INakedObject adaptedResult = nakedObjectManager.CreateAdapter(result, null, null);

            Log.DebugFormat("Action result {0}", adaptedResult);
            return adaptedResult;
        }

        public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager) {
            return Invoke(nakedObject, parameters, lifecycleManager, manager, session, nakedObjectManager);
        }

        protected override string ToStringValues() {
            return "method=" + actionMethod;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}