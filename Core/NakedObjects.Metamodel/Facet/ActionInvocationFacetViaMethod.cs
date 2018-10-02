// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ActionInvocationFacetViaMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ActionInvocationFacetViaMethod));
        [field: NonSerialized]
        private Func<object, object[], object> actionDelegate;
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

            try {
                actionDelegate = DelegateUtils.CreateDelegate(actionMethod);
            }
            catch (Exception e) {
                Log.ErrorFormat("Failed to get Delegate for {0}:{1} reason {2}", onType, method, e.Message);
            }
        }

        // for testing only 
        internal Func<object, object[], object> ActionDelegate => actionDelegate;

        public override MethodInfo ActionMethod => actionMethod;

        public override IObjectSpecImmutable ReturnType => returnType;

        public override ITypeSpecImmutable OnType => onType;

        public override IObjectSpecImmutable ElementType => elementType;

        public override bool IsQueryOnly => isQueryOnly;

        #region IImperativeFacet Members

        /// <summary>
        ///     See <see cref="IImperativeFacet" />
        /// </summary>
        public MethodInfo GetMethod() {
            return actionMethod;
        }

        public Func<object, object[], object> GetMethodDelegate() {
            return actionDelegate;
        }

        #endregion

        public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            if (parameters.Length != paramCount) {
                Log.Error(actionMethod + " requires " + paramCount + " parameters, not " + parameters.Length);
            }

            object result;
            if (actionDelegate != null) {
                result = actionDelegate(inObjectAdapter.GetDomainObject(), parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                Log.WarnFormat("Invoking action via reflection as no delegate {0}.{1}", onType, actionMethod);
                result = InvokeUtils.Invoke(actionMethod, inObjectAdapter, parameters);
            }

            INakedObjectAdapter adaptedResult = nakedObjectManager.CreateAdapter(result, null, null);

            return adaptedResult;
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            return Invoke(nakedObjectAdapter, parameters, lifecycleManager, manager, session, nakedObjectManager, messageBroker, transactionManager);
        }

        protected override string ToStringValues() {
            return "method=" + actionMethod;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            actionDelegate = DelegateUtils.CreateDelegate(actionMethod);
        }

    }

    // Copyright (c) Naked Objects Group Ltd.
}