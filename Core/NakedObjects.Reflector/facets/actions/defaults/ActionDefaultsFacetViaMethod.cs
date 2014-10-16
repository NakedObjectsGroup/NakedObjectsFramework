// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Defaults;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.DotNet.Facets.Actions.Invoke;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Defaults {
    public class ActionDefaultsFacetViaMethod : ActionDefaultsFacetAbstract, IImperativeFacet {
        private readonly MethodInfo actionMethod;
        private readonly MethodInfo method;

        public ActionDefaultsFacetViaMethod(MethodInfo method, ISpecification holder)
            : base(holder) {
            this.method = method;
            var actionInvocationFacet = holder.GetFacet<IActionInvocationFacet>();
            if (actionInvocationFacet is ActionInvocationFacetViaMethod) {
                var facetViaMethod = (ActionInvocationFacetViaMethod) actionInvocationFacet;
                actionMethod = facetViaMethod.GetMethod();
            }
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override Tuple<object, TypeOfDefaultValue> GetDefault(INakedObject nakedObject) {
            // type safety is given by the reflector only identifying methods that match the 
            // parameter type
            return new Tuple<object, TypeOfDefaultValue>(InvokeUtils.Invoke(method, nakedObject), TypeOfDefaultValue.Explicit);
        }

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}