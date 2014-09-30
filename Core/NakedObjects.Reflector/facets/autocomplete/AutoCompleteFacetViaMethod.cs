// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Util;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.AutoComplete {
    public class AutoCompleteFacetViaMethod : AutoCompleteFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        public AutoCompleteFacetViaMethod(MethodInfo autoCompleteMethod, int pageSize, int minLength, IFacetHolder holder)
            : base(holder) {
            method = autoCompleteMethod;
            PageSize = pageSize == 0 ? DefaultPageSize : pageSize;
            MinLength = minLength;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override object[] GetCompletions(INakedObject inObject, string autoCompleteParm) {
            try {
                object autoComplete = InvokeUtils.Invoke(method, inObject.GetDomainObject(), new object[] {autoCompleteParm});
                if (autoComplete is IQueryable) {
                    return ((IQueryable) autoComplete).Take(PageSize).ToArray();
                }
                throw new NakedObjectDomainException("Must return IQueryable from autoComplete method: " + method.Name);
            }
            catch (ArgumentException ae) {
                string msg = string.Format("autoComplete exception: {0} has mismatched parameter type - must be string", method.Name);
                throw new InvokeException(msg, ae);
            }
        }

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}