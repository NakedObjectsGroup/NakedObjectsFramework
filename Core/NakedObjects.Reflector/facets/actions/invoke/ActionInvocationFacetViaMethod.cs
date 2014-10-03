// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.DotNet.Reflect.Util;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Invoke {
    public class ActionInvocationFacetViaMethod : ActionInvocationFacetAbstract, IImperativeFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ActionInvocationFacetViaMethod));

        private readonly MethodInfo actionMethod;
        private readonly IIntrospectableSpecification onType;
        private readonly int paramCount;
        private readonly IIntrospectableSpecification returnType;

        public ActionInvocationFacetViaMethod(MethodInfo method, IIntrospectableSpecification onType, IIntrospectableSpecification returnType, IFacetHolder holder)
            : base(holder) {
            actionMethod = method;
            paramCount = method.GetParameters().Length;
            this.onType = onType;
            this.returnType = returnType;
        }

        public override IIntrospectableSpecification ReturnType {
            get { return returnType; }
        }

        public override IIntrospectableSpecification OnType {
            get { return onType; }
        }

        #region IImperativeFacet Members

        /// <summary>
        ///     See <see cref="IImperativeFacet" />
        /// </summary>
        public MethodInfo GetMethod() {
            return actionMethod;
        }

        #endregion

        public override INakedObject Invoke(INakedObject inObject, INakedObject[] parameters, ILifecycleManager persistor, ISession session) {
            if (parameters.Length != paramCount) {
                Log.Error(actionMethod + " requires " + paramCount + " parameters, not " + parameters.Length);
            }

            object result = InvokeUtils.Invoke(actionMethod, inObject, parameters);
            INakedObject adaptedResult = persistor.CreateAdapter(result, null, null);

            Log.DebugFormat("Action result {0}", adaptedResult);
            return adaptedResult;
        }

        public override INakedObject Invoke(INakedObject nakedObject, INakedObject[] parameters, int resultPage, ILifecycleManager persistor, ISession session) {
            return Invoke(nakedObject, parameters, persistor, session);
        }

        protected override string ToStringValues() {
            return "method=" + actionMethod;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}