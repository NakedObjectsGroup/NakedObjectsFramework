// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Core.Util.Query;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class AutoCompleteFacet : FacetAbstract, IAutoCompleteFacet, IImperativeFacet {
        private const int DefaultPageSize = 50;
        private readonly MethodInfo method;
        private readonly Func<object, object[], object> methodDelegate;

        private AutoCompleteFacet(ISpecification holder)
            : base(Type, holder) {}

        public AutoCompleteFacet(MethodInfo autoCompleteMethod, int pageSize, int minLength, ISpecification holder)
            : this(holder) {
            method = autoCompleteMethod;
            PageSize = pageSize == 0 ? DefaultPageSize : pageSize;
            MinLength = minLength;
            methodDelegate = DelegateUtils.CreateDelegate(method);
        }

        public static Type Type {
            get { return typeof (IAutoCompleteFacet); }
        }

        public int PageSize { get; private set; }

        #region IAutoCompleteFacet Members

        public int MinLength { get; private set; }

        public object[] GetCompletions(INakedObject inObject, string autoCompleteParm) {
            try {
                object autoComplete = methodDelegate(inObject.GetDomainObject(), new object[] { autoCompleteParm });

                var complete = autoComplete as IQueryable;
                if (complete != null) {
                    return complete.Take(PageSize).ToArray();
                }
                throw new NakedObjectDomainException("Must return IQueryable from autoComplete method: " + method.Name);
            }
            catch (ArgumentException ae) {
                string msg = string.Format("autoComplete exception: {0} has mismatched parameter type - must be string", method.Name);
                throw new InvokeException(msg, ae);
            }
        }

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        public Func<object, object[], object> GetMethodDelegate() {
            return methodDelegate;
        }

        #endregion

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}