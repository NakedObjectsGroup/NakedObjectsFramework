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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Spec;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class PropertyChoicesFacet : FacetAbstract, IPropertyChoicesFacet, IImperativeFacet {
        private readonly ILogger<PropertyChoicesFacet> logger;

        private readonly MethodInfo method;

        private readonly string[] parameterNames;

        [field: NonSerialized] private Func<object, object[], object> methodDelegate;

        public PropertyChoicesFacet(MethodInfo optionsMethod, (string name, IObjectSpecImmutable type)[] parameterNamesAndTypes, ISpecification holder, ILogger<PropertyChoicesFacet> logger)
            : base(typeof(IPropertyChoicesFacet), holder) {
            method = optionsMethod;
            this.logger = logger;

            ParameterNamesAndTypes = parameterNamesAndTypes;
            parameterNames = parameterNamesAndTypes.Select(pnt => pnt.name).ToArray();
            methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

        #endregion

        #region IPropertyChoicesFacet Members

        public (string, IObjectSpecImmutable)[] ParameterNamesAndTypes { get; }

        public object[] GetChoices(INakedObjectAdapter inObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            var parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);
            try {
                var options = methodDelegate(inObjectAdapter.GetDomainObject(), parms.Select(p => p.GetDomainObject()).ToArray());
                if (options is IEnumerable enumerable) {
                    return enumerable.Cast<object>().ToArray();
                }

                throw new NakedObjectDomainException($"Must return IEnumerable from choices method: {method.Name}");
            }
            catch (ArgumentException ae) {
                throw new InvokeException($"Choices exception: {method.Name} has mismatched (ie type of parameter does not match type of property) parameter types", ae);
            }
        }

        #endregion

        protected override string ToStringValues() => $"method={method}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    // Copyright (c) Naked Objects Group Ltd.
}