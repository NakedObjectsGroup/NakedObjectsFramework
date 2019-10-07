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
using System.Runtime.Serialization;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class PropertyChoicesFacet : FacetAbstract, IPropertyChoicesFacet, IImperativeFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PropertyChoicesFacet));

        private readonly MethodInfo method;

        [field: NonSerialized] private Func<object, object[], object> methodDelegate;

        private readonly string[] parameterNames;

        public PropertyChoicesFacet(MethodInfo optionsMethod, Tuple<string, IObjectSpecImmutable>[] parameterNamesAndTypes, ISpecification holder)
            : base(typeof(IPropertyChoicesFacet), holder) {
            method = optionsMethod;

            ParameterNamesAndTypes = parameterNamesAndTypes;
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

        public Tuple<string, IObjectSpecImmutable>[] ParameterNamesAndTypes { get; }

        public object[] GetChoices(INakedObjectAdapter inObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            INakedObjectAdapter[] parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);
            try {
                object options = methodDelegate(inObjectAdapter.GetDomainObject(), parms.Select(p => p.GetDomainObject()).ToArray());
                var enumerable = options as IEnumerable;
                if (enumerable != null) {
                    return enumerable.Cast<object>().ToArray();
                }

                throw new NakedObjectDomainException(Log.LogAndReturn($"Must return IEnumerable from choices method: {method.Name}"));
            }
            catch (ArgumentException ae) {
                throw new InvokeException(Log.LogAndReturn($"Choices exception: {method.Name} has mismatched (ie type of parameter does not match type of property) parameter types"), ae);
            }
        }

        #endregion

        protected override string ToStringValues() {
            return "method=" + method;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            methodDelegate = DelegateUtils.CreateDelegate(method);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}