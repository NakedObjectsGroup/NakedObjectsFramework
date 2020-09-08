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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Facet;

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

            return result.Count == 1 ? result.First() : result;
        }

        private (IEnumerable<object>, IEnumerable<Action>) HandleTupleItem(object item, IEnumerable<object> persisting,
                                                                           IEnumerable<Action> acting) =>
            item switch {
                Action action => (persisting, acting.Append(action)),
                ITuple tuple => HandleNestedTuple(tuple, persisting, acting),
                { } o => (persisting.Append(o), acting),
                null => (persisting, acting)
            };


        private (IEnumerable<object>, IEnumerable<Action>) IterateTuple(ITuple tuple, int start,
                                                                        IEnumerable<object> persisting,
                                                                        IEnumerable<Action> acting) {
            for (var i = start; i < tuple.Length; i++) {
                (persisting, acting) = HandleTupleItem(tuple[i], persisting, acting);
            }

            return (persisting, acting);
        }

        private (IEnumerable<object>, IEnumerable<Action>) HandleNestedTuple(ITuple tuple,
                                                                             IEnumerable<object> persisting,
                                                                             IEnumerable<Action> acting) =>
            IterateTuple(tuple, 0, persisting, acting);

        private (object, (IEnumerable<object>, IEnumerable<Action>)) HandleTuple(ITuple tuple, IEnumerable<object> persisting, IEnumerable<Action> acting) =>
            (tuple[0], IterateTuple(tuple, 1, persisting, acting));

        private INakedObjectAdapter HandleInvokeResult(INakedObjectManager nakedObjectManager,
                                                       ILifecycleManager lifecycleManager, IMessageBroker messageBroker,
                                                       object result) {
            object toReturn;
            IEnumerable<object> toPersist = new List<object>();
            IEnumerable<Action> toAct = new List<Action>();

            if (result is ITuple tuple) {
                var size = tuple.Length;

                if (size < 2) {
                    throw new InvokeException("Invalid return type", new Exception());
                }

                (toReturn, (toPersist, toAct)) = HandleTuple(tuple, toPersist, toAct);
            }
            else {
                toReturn = result;
            }

            var persisted = PersistResult(lifecycleManager, toPersist);

            // TODO handle injection on actions 
            toAct.ForEach(a => a());

            toReturn = ReplacePersisted(toReturn, persisted);

            return AdaptResult(nakedObjectManager, toReturn);
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter,
                                                   INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager,
                                                   IMetamodelManager manager,
                                                   ISession session, INakedObjectManager nakedObjectManager,
                                                   IMessageBroker messageBroker,
                                                   ITransactionManager transactionManager) {
            if (parameters.Length != paramCount) {
                logger.LogError($"{ActionMethod} requires {paramCount} parameters, not {parameters.Length}");
            }

            return HandleInvokeResult(nakedObjectManager, lifecycleManager, messageBroker, InvokeUtils.InvokeStatic(ActionMethod, parameters));
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter,
                                                   INakedObjectAdapter[] parameters,
                                                   int resultPage,
                                                   ILifecycleManager lifecycleManager,
                                                   IMetamodelManager manager,
                                                   ISession session,
                                                   INakedObjectManager nakedObjectManager,
                                                   IMessageBroker messageBroker,
                                                   ITransactionManager transactionManager) =>
            Invoke(nakedObjectAdapter, parameters, lifecycleManager, manager, session, nakedObjectManager,
                   messageBroker, transactionManager);

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