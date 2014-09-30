// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Validate;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Actions.Validate {
    public class ActionValidationFacetViaMethod : ActionValidationFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        public ActionValidationFacetViaMethod(MethodInfo method, IFacetHolder holder)
            : base(holder) {
            this.method = method;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override string InvalidReason(INakedObject target, INakedObject[] proposedArguments) {
            return (string) InvokeUtils.Invoke(method, target, proposedArguments);
        }


        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}