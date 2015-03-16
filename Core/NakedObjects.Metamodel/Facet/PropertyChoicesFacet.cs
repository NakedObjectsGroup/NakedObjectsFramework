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
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class PropertyChoicesFacetx : FacetAbstract, IPropertyChoicesFacet, IImperativeFacet {
        private readonly MethodInfo method;
        private readonly string[] parameterNames;
        private readonly Tuple<string, IObjectSpecImmutable>[] parameterNamesAndTypes;
        private readonly Func<object, object[], object> methodDelegate;

        public PropertyChoicesFacetx(MethodInfo optionsMethod, Tuple<string, IObjectSpecImmutable>[] parameterNamesAndTypes, ISpecification holder)
            : base(typeof (IPropertyChoicesFacet), holder) {
            method = optionsMethod;

            this.parameterNamesAndTypes = parameterNamesAndTypes;
            parameterNames = parameterNamesAndTypes.Select(pnt => pnt.Item1).ToArray();
            methodDelegate = DelegateUtils.CreateDelegate(method);
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        public Func<object, object[], object> GetMethodDelegate() {
            return methodDelegate;
        }

        #endregion

        #region IPropertyChoicesFacet Members

        public Tuple<string, IObjectSpecImmutable>[] ParameterNamesAndTypes {
            get { return parameterNamesAndTypes; }
        }

        public object[] GetChoices(INakedObjectAdapter inObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            INakedObjectAdapter[] parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);
            try {
                object options = methodDelegate(inObjectAdapter.GetDomainObject(), parms.Select(p => p.GetDomainObject()).ToArray());
                var enumerable = options as IEnumerable;
                if (enumerable != null) {
                    return enumerable.Cast<object>().ToArray();
                }
                throw new NakedObjectDomainException("Must return IEnumerable from choices method: " + method.Name);
            }
            catch (ArgumentException ae) {
                string msg = string.Format("Choices exception: {0} has mismatched (ie type of parameter does not match type of property) parameter types", method.Name);
                throw new InvokeException(msg, ae);
            }
        }

        #endregion

        protected override string ToStringValues() {
            return "method=" + method;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}