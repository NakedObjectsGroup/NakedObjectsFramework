// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Core.Persist;
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

        private static Func<bool> GetPostSaveFunction(FunctionalContext functionalContext, INakedObjectsFramework framework) {
            var postSaveFunction = functionalContext.PostSaveFunction;
            var newContext = new FunctionalContext() {Persistor = functionalContext.Persistor, Provider = functionalContext.Provider};

            return () => {
                var innerContext = (FunctionalContext) postSaveFunction(newContext);
                var updated = PersistResult(framework.LifecycleManager, innerContext.New, innerContext.Updated, GetPostSaveFunction(innerContext, framework));
                return updated.Any();
            };
        }


        private static (object original, object updated)[] PersistResult(ILifecycleManager lifecycleManager, object[] newObjects, (object proxy, object updated)[] updatedObjects, Func<bool> postSaveFunction) =>
            lifecycleManager.Persist(new DetachedObjects(newObjects, updatedObjects, postSaveFunction)).ToArray();

        private static (object, FunctionalContext) CastTuple(ITuple tuple) => (tuple[0], (FunctionalContext)tuple[1]);

        private (object original, object updated)[] HandleContext(FunctionalContext functionalContext, INakedObjectsFramework framework) {
            //PerformActions(framework.ServicesManager, framework.ServiceProvider, new[] {functionalContext.Action});
            return PersistResult(framework.LifecycleManager, functionalContext.New, functionalContext.Updated, GetPostSaveFunction(functionalContext, framework));
        }

        private object HandleTupleResult((object, FunctionalContext) tuple, INakedObjectsFramework framework) {
            var (toReturn, context) = tuple;
            var allPersisted = HandleContext(context, framework);

            foreach (var valueTuple in allPersisted) {
                if (ReferenceEquals(valueTuple.original, toReturn)) {
                    return valueTuple.updated;
                }
            }

            return toReturn;
        }

        private object HandleContextResult(FunctionalContext functionalContext, INakedObjectsFramework framework) {
            HandleContext(functionalContext, framework);
            return null;
        }

        private INakedObjectAdapter HandleInvokeResult(INakedObjectsFramework framework, object result) {
            // if any changes made by invocation fail 

            if (framework.Persistor.HasChanges()) {
                throw new PersistFailedException($"method {ActionMethod} on {ActionMethod.DeclaringType} made database changes and so is not pure");
            }

            var toReturn = result switch {
                ITuple tuple => HandleTupleResult(CastTuple(ValidateTuple(tuple)), framework),
                FunctionalContext context => HandleContextResult(context, framework),
                _ => result
            };

            return AdaptResult(framework.NakedObjectManager, toReturn);
        }

        private ITuple ValidateTuple(ITuple tuple) {
            var size = tuple.Length;

            if (size is not 2) {
                throw new InvokeException($"Invalid return type {size} item tuple on {ActionMethod.Name}");
            }

            return tuple;
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