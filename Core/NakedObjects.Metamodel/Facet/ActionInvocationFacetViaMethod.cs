// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

[assembly:InternalsVisibleTo("NakedObjects.Metamodel.Test")]

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ActionInvocationFacetViaMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ActionInvocationFacetViaMethod));
        private readonly int paramCount;

        public ActionInvocationFacetViaMethod(MethodInfo method, ITypeSpecImmutable onType, IObjectSpecImmutable returnType, IObjectSpecImmutable elementType, ISpecification holder, bool isQueryOnly)
            : base(holder) {
            ActionMethod = method;
            paramCount = method.GetParameters().Length;
            this.OnType = onType;
            this.ReturnType = returnType;
            this.ElementType = elementType;
            this.IsQueryOnly = isQueryOnly;

            try {
                ActionDelegate = DelegateUtils.CreateDelegate(ActionMethod);
            }
            catch (Exception e) {
                Log.ErrorFormat("Failed to get Delegate for {0}:{1} reason {2}", onType, method, e.Message);
            }
        }

        // for testing only 
        [field: NonSerialized]
        internal Func<object, object[], object> ActionDelegate { get; private set; }

        public override MethodInfo ActionMethod { get; }

        public override IObjectSpecImmutable ReturnType { get; }

        public override ITypeSpecImmutable OnType { get; }

        public override IObjectSpecImmutable ElementType { get; }

        public override bool IsQueryOnly { get; }

        #region IImperativeFacet Members

        /// <summary>
        ///     See <see cref="IImperativeFacet" />
        /// </summary>
        public MethodInfo GetMethod() {
            return ActionMethod;
        }

        public Func<object, object[], object> GetMethodDelegate() {
            return ActionDelegate;
        }

        #endregion

        public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            if (parameters.Length != paramCount) {
                Log.Error(ActionMethod + " requires " + paramCount + " parameters, not " + parameters.Length);
            }

            object result;
            if (ActionDelegate != null) {
                result = ActionDelegate(inObjectAdapter.GetDomainObject(), parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                Log.WarnFormat("Invoking action via reflection as no delegate {0}.{1}", OnType, ActionMethod);
                result = InvokeUtils.Invoke(ActionMethod, inObjectAdapter, parameters);
            }

            INakedObjectAdapter adaptedResult = nakedObjectManager.CreateAdapter(result, null, null);

            return adaptedResult;
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            return Invoke(nakedObjectAdapter, parameters, lifecycleManager, manager, session, nakedObjectManager, messageBroker, transactionManager);
        }

        protected override string ToStringValues() {
            return "method=" + ActionMethod;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            ActionDelegate = DelegateUtils.CreateDelegate(ActionMethod);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}