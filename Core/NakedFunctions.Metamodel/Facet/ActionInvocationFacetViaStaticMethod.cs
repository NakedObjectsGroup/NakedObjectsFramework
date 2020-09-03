// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;
using NakedObjects.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ActionInvocationFacetViaStaticMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(ActionInvocationFacetViaMethod));

        [field: NonSerialized] private readonly MethodInfo actionMethod;

        private readonly int paramCount;

        public ActionInvocationFacetViaStaticMethod(MethodInfo method, ITypeSpecImmutable onType, IObjectSpecImmutable returnType, IObjectSpecImmutable elementType, ISpecification holder, bool isQueryOnly)
            : base(holder) {
            actionMethod = method;
            paramCount = method.GetParameters().Length;
            this.OnType = onType;
            this.ReturnType = returnType;
            this.ElementType = elementType;
            this.IsQueryOnly = isQueryOnly;
        }

        public override MethodInfo ActionMethod => actionMethod;

        public override IObjectSpecImmutable ReturnType { get; }

        public override ITypeSpecImmutable OnType { get; }

        public override IObjectSpecImmutable ElementType { get; }

        public override bool IsQueryOnly { get; }

        #region IImperativeFacet Members

        /// <summary>
        ///     See <see cref="IImperativeFacet" />
        /// </summary>
        public MethodInfo GetMethod() {
            return actionMethod;
        }

        public Func<object, object[], object> GetMethodDelegate() {
            return null;
        }

        #endregion

        private INakedObjectAdapter AdaptResult(INakedObjectManager nakedObjectManager, object result) {
            if (CollectionUtils.IsCollection(result.GetType()) ||
                CollectionUtils.IsQueryable(result.GetType())) {
                return nakedObjectManager.CreateAdapter(result, null, null);
            }

            return nakedObjectManager.CreateAdapterForExistingObject(result);
        }

        private static IEnumerable<object> UnpackTuples(object result) {

            if (FacetUtils.IsValueTuple(result.GetType())) {
                return result.GetType().GetTypeInfo().DeclaredFields.SelectMany<FieldInfo, object>(p => UnpackTuples(p.GetValue(result))).ToArray();
            }

            return result as IEnumerable<object> ?? new[] {result};
        }



        private (object,object)[] PersistResult(ILifecycleManager lifecycleManager, object result) {
            var ret = new List<(object,object)>();

            if (result != null) {
                // already filtered strings

                foreach (var obj in UnpackTuples(result)) {
                    ret.Add((obj, lifecycleManager.Persist(obj)));
                }
            }

            return ret.ToArray();
        }

        private void MessageResult(IMessageBroker messageBroker, string message) {
            if (message != null) {
                messageBroker.AddWarning(message);
            }
        }

        private object ReplacePersisted(object toReturn, (object, object)[] persisted)
        {
            var asEnumerable = toReturn as IEnumerable ?? new[] { toReturn };
            var result = new List<object>();

            foreach (var obj in asEnumerable) {
                var found = false;
                foreach (var tuple in persisted) {
                    if (tuple.Item1 == obj) {
                        result.Add(tuple.Item2);
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


        private INakedObjectAdapter HandleInvokeResult(INakedObjectManager nakedObjectManager, ILifecycleManager lifecycleManager, IMessageBroker messageBroker, object result) {
            var type = result.GetType();
            string message = null;
            object toReturn;
            object toPersist = null;

            if (FacetUtils.IsEitherTuple(type)) {
                // TODO dynamic just for spike do a proper cast in real code
                dynamic tuple = result;
                int size = FacetUtils.ValueTupleSize(type);
               
                if (size < 2) {
                    throw new InvokeException("Invalid return type", new Exception());
                }

                toReturn = tuple.Item1;

                if (size == 2) {
                    if (tuple.Item2 is string) {
                        message = tuple.Item2;
                    }
                    else {
                        toPersist = tuple.Item2;
                    }
                }

                if (size == 3) {
                    toReturn = tuple.Item1;
                    toPersist = tuple.Item2;
                    message = tuple.Item3;
                }
            }
            else {
                toReturn = result;
            }

            var persisted = PersistResult(lifecycleManager, toPersist);
            MessageResult(messageBroker, message);

            toReturn = ReplacePersisted(toReturn, persisted);
            

            return AdaptResult(nakedObjectManager, toReturn);
        }

       

        public override INakedObjectAdapter Invoke(INakedObjectAdapter inObjectAdapter, INakedObjectAdapter[] parameters, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            if (parameters.Length != paramCount) {
                //Log.Error(actionMethod + " requires " + paramCount + " parameters, not " + parameters.Length);
            }

            return HandleInvokeResult(nakedObjectManager, lifecycleManager, messageBroker, InvokeUtils.InvokeStatic(actionMethod, parameters));
        }

        public override INakedObjectAdapter Invoke(INakedObjectAdapter nakedObjectAdapter, INakedObjectAdapter[] parameters, int resultPage, ILifecycleManager lifecycleManager, IMetamodelManager manager, ISession session, INakedObjectManager nakedObjectManager, IMessageBroker messageBroker, ITransactionManager transactionManager) {
            return Invoke(nakedObjectAdapter, parameters, lifecycleManager, manager, session, nakedObjectManager, messageBroker, transactionManager);
        }

        protected override string ToStringValues() {
            return "method=" + actionMethod;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) { }
    }

    // Copyright (c) Naked Objects Group Ltd.
}