// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFunctions.Reflector.Component;
using NakedObjects;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedFunctions.Meta.Facet {
    [Serializable]
    public sealed class ActionInvocationFacetViaStaticMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        private readonly ILogger<ActionInvocationFacetViaStaticMethod> logger;

        private readonly int paramCount;

        public ActionInvocationFacetViaStaticMethod(MethodInfo method,
                                                    ITypeSpecImmutable onType,
                                                    IObjectSpecImmutable returnType,
                                                    IObjectSpecImmutable elementType,
                                                    ISpecification holder,
                                                    bool isQueryOnly,
                                                    ILogger<ActionInvocationFacetViaStaticMethod> logger)
            : base(holder) {
            ActionMethod = method;
            this.logger = logger;
            paramCount = method.GetParameters().Length;
            OnType = onType;
            ReturnType = returnType;
            ElementType = elementType;
            IsQueryOnly = isQueryOnly;
        }

        [field: NonSerialized] public override MethodInfo ActionMethod { get; }

        public override IObjectSpecImmutable ReturnType { get; }

        public override ITypeSpecImmutable OnType { get; }

        public override IObjectSpecImmutable ElementType { get; }

        public override bool IsQueryOnly { get; }

        private static INakedObjectAdapter AdaptResult(INakedObjectManager nakedObjectManager, object result) {
            if (result is null) {
                return null;
            }

            if (CollectionUtils.IsCollection(result.GetType()) ||
                CollectionUtils.IsQueryable(result.GetType())) {
                return nakedObjectManager.CreateAdapter(result, null, null);
            }

            return nakedObjectManager.CreateAdapterForExistingObject(result);
        }


        private static (object, object)[] PersistResult(ILifecycleManager lifecycleManager,
                                                        IEnumerable<object> toPersist) =>
            toPersist.Select(obj => (obj, lifecycleManager.Persist(obj))).ToArray();

        private static object ReplacePersisted(object toReturn, (object, object)[] persisted) {
            var returnList = toReturn is IEnumerable;
            var asEnumerable = toReturn as IEnumerable ?? new[] {toReturn};
            var result = new List<object>();

            foreach (var obj in asEnumerable) {
                var found = false;
                foreach (var (item1, item2) in persisted) {
                    if (item1 == obj) {
                        result.Add(item2);
                        found = true;
                        break;
                    }
                }

                if (!found) {
                    result.Add(obj);
                }
            }

            return returnList ? result : result.First();
        }

        private (object, Context) HandleTuple(ITuple tuple) =>
            (tuple[0], (Context)tuple[1]);

        private object HandleContext(Context context, object toReturn,  INakedObjectsFramework framework) {
            object[] toAct = {context.Action};
            PerformActions(framework.ServicesManager, framework.ServiceProvider, toAct);

            object[] toPersist = context.PendingSave;

            var persisted = PersistResult(framework.LifecycleManager, toPersist);
            return ReplacePersisted(toReturn, persisted);
        }

        private INakedObjectAdapter HandleInvokeResult(INakedObjectsFramework framework,
                                                       object result) {
            object toReturn;
            IEnumerable<object> toPersist = new List<object>();

            if (result is ITuple tuple) {
                var size = tuple.Length;

                if (size < 2) {
                    throw new InvokeException($"Invalid return type single item tuple on {ActionMethod.Name}");
                }

                Context context;
                (toReturn, context) = HandleTuple(tuple);
                toReturn = HandleContext(context, toReturn, framework);
            }
            else {
                toReturn = result;
            }

            return AdaptResult(framework.NakedObjectManager, toReturn);
        }

        private void PerformActions(IServicesManager servicesManager, IServiceProvider serviceProvider, IEnumerable<object> toAct) => toAct.ForEach(a => PerformAction(servicesManager, serviceProvider, a));

        private static object GetInjectedService(IServicesManager servicesManager, IServiceProvider serviceProvider, Type injectType) {
            return servicesManager.GetServices().Select(no => no.Object).SingleOrDefault(service => injectType.IsInstanceOfType(service)) ??
                   serviceProvider.GetService(injectType);
        }


        private void PerformAction(IServicesManager servicesManager, IServiceProvider serviceProvider, object action) {
            if (action is not null) {
                var injectType = GetInjectArgumentType(action);

                var injectedService = GetInjectedService(servicesManager, serviceProvider, injectType);

                if (injectedService != null) {
                    var f = typeof(InjectUtils).GetMethod("PerformAction")?.MakeGenericMethod(injectType);
                    f?.Invoke(null, new object[] {action, injectedService});
                }
                else {
                    throw new InvokeException($"Failed to get service for injection argument type {injectType} on action {ActionMethod.Name}");
                }
            }
        }

        private Type GetInjectArgumentType(object action) {
            try {
                return action.GetType().GetGenericArguments().Single();
            }
            catch (Exception e) {
                throw new InvokeException($"Failed to get Single injection argument for action on {ActionMethod.Name}", e);
            }
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter,
                                                   INakedObjectAdapter[] parameters,
                                                   INakedObjectsFramework framework) {
            if (parameters.Length != paramCount) {
                logger.LogError($"{ActionMethod} requires {paramCount} parameters, not {parameters.Length}");
            }

            return HandleInvokeResult(framework, InvokeUtils.InvokeStatic(ActionMethod, parameters));
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter,
                                                   INakedObjectAdapter[] parameters,
                                                   int resultPage,
                                                   INakedObjectsFramework framework) =>
            Invoke(nakedObjectAdapter, parameters, framework);

        protected override string ToStringValues() => $"method={ActionMethod}";

        [OnDeserialized]
        private static void OnDeserialized(StreamingContext context) { }

        #region IImperativeFacet Members

        /// <summary>
        ///     See <see cref="IImperativeFacet" />
        /// </summary>
        public MethodInfo GetMethod() => ActionMethod;

        public Func<object, object[], object> GetMethodDelegate() => null;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}