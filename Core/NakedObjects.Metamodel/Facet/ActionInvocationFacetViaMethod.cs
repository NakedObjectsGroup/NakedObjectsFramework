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
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

[assembly: InternalsVisibleTo("NakedObjects.Metamodel.Test")]

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ActionInvocationFacetViaMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        private readonly ILogger<ActionInvocationFacetViaMethod> logger;
        private readonly int paramCount;

        public ActionInvocationFacetViaMethod(MethodInfo method, ITypeSpecImmutable onType, IObjectSpecImmutable returnType, IObjectSpecImmutable elementType, ISpecification holder, bool isQueryOnly, ILogger<ActionInvocationFacetViaMethod> logger)
            : base(holder) {
            this.logger = logger;
            ActionMethod = method;
            paramCount = method.GetParameters().Length;
            OnType = onType;
            ReturnType = returnType;
            ElementType = elementType;
            IsQueryOnly = isQueryOnly;
            ActionDelegate = LogNull(DelegateUtils.CreateDelegate(ActionMethod), logger);
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
        public MethodInfo GetMethod() => ActionMethod;

        public Func<object, object[], object> GetMethodDelegate() => ActionDelegate;

        #endregion

        public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            if (parameters.Length != paramCount) {
                logger.LogError($"{ActionMethod} requires {paramCount} parameters, not {parameters.Length}");
            }

            object result;
            if (ActionDelegate != null) {
                result = ActionDelegate(inObjectAdapter.GetDomainObject(), parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                logger.LogWarning($"Invoking action via reflection as no delegate {OnType}.{ActionMethod}");
                result = InvokeUtils.Invoke(ActionMethod, inObjectAdapter, parameters);
            }

            return nakedObjectManager.CreateAdapter(result, null, null);
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) => Invoke(nakedObjectAdapter, parameters, lifecycleManager, manager, session, nakedObjectManager, messageBroker, transactionManager);

        protected override string ToStringValues() => $"method={ActionMethod}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => ActionDelegate = LogNull(DelegateUtils.CreateDelegate(ActionMethod), logger);
    }

    // Copyright (c) Naked Objects Group Ltd.
}