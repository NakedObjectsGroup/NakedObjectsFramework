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
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core;
using NakedObjects.Core.Util;
using NakedObjects.Core.Util.Query;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class AutoCompleteFacet : FacetAbstract, IAutoCompleteFacet, IImperativeFacet {
        private const int DefaultPageSize = 50;
        private readonly ILogger<AutoCompleteFacet> logger;
        private readonly MethodInfo method;
        [field: NonSerialized] private Func<object, object[], object> methodDelegate;

        private AutoCompleteFacet(ISpecification holder)
            : base(Type, holder) { }

        public AutoCompleteFacet(MethodInfo autoCompleteMethod, int pageSize, int minLength, ISpecification holder, ILogger<AutoCompleteFacet> logger)
            : this(holder) {
            method = autoCompleteMethod;
            this.logger = logger;
            PageSize = pageSize == 0 ? DefaultPageSize : pageSize;
            MinLength = minLength;
            methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
        }

        public static Type Type => typeof(IAutoCompleteFacet);

        public int PageSize { get; }

        #region IAutoCompleteFacet Members

        public int MinLength { get; }

        public object[] GetCompletions(INakedObjectAdapter inObjectAdapter, string autoCompleteParm, ISession session, IObjectPersistor persistor) {
            try {
                var autoComplete = methodDelegate(inObjectAdapter.GetDomainObject(), new object[] {autoCompleteParm});
                return autoComplete switch {
                    IQueryable queryable => queryable.Take(PageSize).ToArray(),
                    IEnumerable<string> strings => strings.Cast<object>().ToArray(),
                    _ when !CollectionUtils.IsCollection(autoComplete.GetType()) => new[] {autoComplete},
                    _ => throw new NakedObjectDomainException($"Must return IQueryable or a single object from autoComplete method: {method.Name}")
                };
            }
            catch (ArgumentException ae) {
                throw new InvokeException($"autoComplete exception: {method.Name} has mismatched parameter type - must be string", ae);
            }
        }

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

        #endregion

        protected override string ToStringValues() => $"method={method}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
    }

    // Copyright (c) Naked Objects Group Ltd.
}