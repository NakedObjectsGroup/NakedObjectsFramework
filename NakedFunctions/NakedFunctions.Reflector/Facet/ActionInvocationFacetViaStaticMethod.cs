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
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Persist;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;
using NakedFunctions.Reflector.Component;

namespace NakedFunctions.Reflector.Facet {
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

        private static INakedObjectAdapter AdaptResult(INakedObjectManager nakedObjectManager, object result) =>
            result is null ? null : nakedObjectManager.CreateAdapter(result, null, null);

        private static Func<IDictionary<object, object>, bool> GetPostSaveFunction(FunctionalContext functionalContext, INakedObjectsFramework framework) {
            var postSaveFunction = functionalContext.PostSaveFunction;

            if (postSaveFunction is not null) {
                return map => {
                    var newContext = new FunctionalContext {Persistor = functionalContext.Persistor, Provider = functionalContext.Provider, ProxyMap = map};
                    var innerContext = (FunctionalContext) postSaveFunction(newContext);
                    var updated = PersistResult(framework.LifecycleManager, innerContext.New, innerContext.Deleted, innerContext.Updated, GetPostSaveFunction(innerContext, framework));
                    return updated.Any();
                };
            }

            return _ => false;
        }

        private static (object original, object updated)[] PersistResult(ILifecycleManager lifecycleManager, object[] newObjects, object[] deletedObjects, (object proxy, object updated)[] updatedObjects, Func<IDictionary<object, object>, bool> postSaveFunction) =>
            lifecycleManager.Persist(new DetachedObjects(newObjects, deletedObjects, updatedObjects, postSaveFunction)).ToArray();

        private static (object, FunctionalContext) CastTuple(ITuple tuple) => (tuple[0], (FunctionalContext) tuple[1]);

        private static (object original, object updated)[] HandleContext(FunctionalContext functionalContext, INakedObjectsFramework framework) =>
            PersistResult(framework.LifecycleManager, functionalContext.New, functionalContext.Deleted, functionalContext.Updated, GetPostSaveFunction(functionalContext, framework));

        private static object HandleTupleResult((object, FunctionalContext) tuple, INakedObjectsFramework framework) {
            var (toReturn, context) = tuple;
            var allPersisted = HandleContext(context, framework);

            foreach (var valueTuple in allPersisted) {
                if (ReferenceEquals(valueTuple.original, toReturn)) {
                    return valueTuple.updated;
                }
            }

            return toReturn;
        }

        private static object HandleContextResult(FunctionalContext functionalContext, INakedObjectsFramework framework) {
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