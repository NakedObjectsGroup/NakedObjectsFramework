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
        private readonly Func<object, object[], object> actionDelegate;
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
                actionDelegate = CreateDelegate(actionMethod);
            }
            catch (Exception e) {
                Log.WarnFormat("Failed to get Delegate for {0}:{1} reason {2}", onType, method, e.Message);
            }
        }

        // for testing only 
        internal Func<object, object[], object> ActionDelegate {
            get {
                return actionDelegate;
            }
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

            object result;
            if (actionDelegate != null) {
                result = actionDelegate.Invoke(inObject.GetDomainObject(), parameters.Select(no => no.GetDomainObject()).ToArray());
            }
            else {
                Log.WarnFormat("Invoking action via reflection as no delegate {0}.{1}", onType, actionMethod);
                result = InvokeUtils.Invoke(actionMethod, inObject, parameters);
            }

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

        #region Delegate Code

        // This is all based on http://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/comment-page-1/

        private static Func<object, object[], object> CreateDelegate(MethodInfo method) {
            if (method.IsSecurityTransparent) {
                // don't seem to be able to bind delegates to these just return null
                Log.InfoFormat("Not creating delegate for IsSecurityTransparent method {0}.{1}", method.DeclaringType, method);
                return null;
            }

            if (method.GetParameters().Count() > 6) {
                // only support 6 parameters via delegates - return null and default to reflection
                Log.InfoFormat("Not creating delegate for method {0}.{1} as has too many parameters", method.DeclaringType, method);
                return null;
            }

            MethodInfo delegateHelper = MakeDelegateHelper(method.DeclaringType, method);

            // Now call it. The null argument is because it’s a static method.
            object ret = delegateHelper.Invoke(null, new object[] {method});

            // Cast the result to the right kind of delegate and return it
            return (Func<object, object[], object>) ret;
        }

        private static MethodInfo MakeDelegateHelper(Type targetType, MethodInfo method) {
            var helperName = method.ReturnType == typeof (void) ? "ActionHelper" : "FuncHelper";

            helperName += method.GetParameters().Count();

            // First fetch the generic form
            MethodInfo genericHelper = typeof (ActionInvocationFacetViaMethod).GetMethod(helperName, BindingFlags.Static | BindingFlags.NonPublic);

            // Now supply the type arguments
            var typeArgs = new List<Type> {targetType};
            typeArgs.AddRange(method.GetParameters().Select(p => p.ParameterType));

            if (method.ReturnType != typeof (void)) {
                typeArgs.Add(method.ReturnType);
            }

            return genericHelper.MakeGenericMethod(typeArgs.ToArray());
        }

        
        // These are all called via reflection - so do not delete or change name without changing the Invoke above !
        // in each convert the slow MethodInfo into a fast, strongly typed, open delegate
        // then create a more weakly typed delegate which will call the strongly typed one
        // at some point in the future these may be replaced with generated code.  

        private static Func<object, object[], object> ActionHelper0<TTarget>(MethodInfo method) where TTarget : class {
            var action = (Action<TTarget>) Delegate.CreateDelegate(typeof (Action<TTarget>), method);
            return (target, param) => {
                action((TTarget) target);
                return null;
            };
        }

        private static Func<object, object[], object> ActionHelper1<TTarget, TParam0>(MethodInfo method) where TTarget : class {
            var action = (Action<TTarget, TParam0>) Delegate.CreateDelegate(typeof (Action<TTarget, TParam0>), method);
            return (target, param) => {
                action((TTarget) target, (TParam0) param[0]);
                return null;
            };
        }

        private static Func<object, object[], object> ActionHelper2<TTarget, TParam0, TParam1>(MethodInfo method) where TTarget : class {
            var action = (Action<TTarget, TParam0, TParam1>) Delegate.CreateDelegate(typeof (Action<TTarget, TParam0, TParam1>), method);
            return (target, param) => {
                action((TTarget) target, (TParam0) param[0], (TParam1) param[1]);
                return null;
            };
        }

        private static Func<object, object[], object> ActionHelper3<TTarget, TParam0, TParam1, TParam2>(MethodInfo method) where TTarget : class {
            var action = (Action<TTarget, TParam0, TParam1, TParam2>) Delegate.CreateDelegate(typeof (Action<TTarget, TParam0, TParam1, TParam2>), method);
            return (target, param) => {
                action((TTarget) target, (TParam0) param[0], (TParam1) param[1], (TParam2) param[2]);
                return null;
            };
        }

        private static Func<object, object[], object> ActionHelper4<TTarget, TParam0, TParam1, TParam2, TParam3>(MethodInfo method) where TTarget : class {
            var action = (Action<TTarget, TParam0, TParam1, TParam2, TParam3>) Delegate.CreateDelegate(typeof (Action<TTarget, TParam0, TParam1, TParam2, TParam3>), method);
            return (target, param) => {
                action((TTarget) target, (TParam0) param[0], (TParam1) param[1], (TParam2) param[2], (TParam3) param[3]);
                return null;
            };
        }

        private static Func<object, object[], object> ActionHelper5<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4>(MethodInfo method) where TTarget : class {
            var action = (Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4>) Delegate.CreateDelegate(typeof (Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4>), method);
            return (target, param) => {
                action((TTarget) target, (TParam0) param[0], (TParam1) param[1], (TParam2) param[2], (TParam3) param[3], (TParam4) param[4]);
                return null;
            };
        }

        private static Func<object, object[], object> ActionHelper6<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>(MethodInfo method) where TTarget : class {
            var action = (Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>) Delegate.CreateDelegate(typeof (Action<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5>), method);
            return (target, param) => {
                action((TTarget) target, (TParam0) param[0], (TParam1) param[1], (TParam2) param[2], (TParam3) param[3], (TParam4) param[4], (TParam5) param[5]);
                return null;
            };
        }

        private static Func<object, object[], object> FuncHelper0<TTarget, TReturn>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TReturn>) Delegate.CreateDelegate(typeof (Func<TTarget, TReturn>), method);
            return (target, param) => func((TTarget) target);
        }

        private static Func<object, object[], object> FuncHelper1<TTarget, TParam0, TReturn>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TParam0, TReturn>) Delegate.CreateDelegate(typeof (Func<TTarget, TParam0, TReturn>), method);
            return (target, param) => func((TTarget) target, (TParam0) param[0]);
        }

        private static Func<object, object[], object> FuncHelper2<TTarget, TParam0, TParam1, TReturn>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TParam0, TParam1, TReturn>) Delegate.CreateDelegate(typeof (Func<TTarget, TParam0, TParam1, TReturn>), method);
            return (target, param) => func((TTarget) target, (TParam0) param[0], (TParam1) param[1]);
        }

        private static Func<object, object[], object> FuncHelper3<TTarget, TParam0, TParam1, TParam2, TReturn>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TParam0, TParam1, TParam2, TReturn>) Delegate.CreateDelegate(typeof (Func<TTarget, TParam0, TParam1, TParam2, TReturn>), method);
            return (target, param) => func((TTarget)target, (TParam0)param[0], (TParam1)param[1], (TParam2)param[2]);
        }

        private static Func<object, object[], object> FuncHelper4<TTarget, TParam0, TParam1, TParam2, TParam3, TReturn>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TReturn>) Delegate.CreateDelegate(typeof (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TReturn>), method);
            return (target, param) => func((TTarget) target, (TParam0) param[0], (TParam1) param[1], (TParam2) param[2], (TParam3) param[3]);
        }

        private static Func<object, object[], object> FuncHelper5<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>) Delegate.CreateDelegate(typeof (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TReturn>), method);
            return (target, param) => func((TTarget) target, (TParam0) param[0], (TParam1) param[1], (TParam2) param[2], (TParam3) param[3], (TParam4) param[4]);
        }

        private static Func<object, object[], object> FuncHelper6<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>(MethodInfo method) where TTarget : class {
            var func = (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>) Delegate.CreateDelegate(typeof (Func<TTarget, TParam0, TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>), method);
            return (target, param) => func((TTarget) target, (TParam0) param[0], (TParam1) param[1], (TParam2) param[2], (TParam3) param[3], (TParam4) param[4], (TParam5) param[5]);
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}