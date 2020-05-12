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
    public sealed class ActionChoicesFacetViaMethod : ActionChoicesFacetAbstract, IImperativeFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ActionChoicesFacetViaMethod));

        private readonly MethodInfo choicesMethod;
        private readonly Type choicesType;
        private readonly string[] parameterNames;

        [field: NonSerialized] private Func<object, object[], object> choicesDelegate;

        public ActionChoicesFacetViaMethod(MethodInfo choicesMethod, (string, IObjectSpecImmutable)[] parameterNamesAndTypes, Type choicesType, ISpecification holder, bool isMultiple = false)
            : base(holder) {
            this.choicesMethod = choicesMethod;
            this.choicesType = choicesType;
            IsMultiple = isMultiple;
            ParameterNamesAndTypes = parameterNamesAndTypes;
            parameterNames = parameterNamesAndTypes.Select(pnt => pnt.Item1).ToArray();
            choicesDelegate = DelegateUtils.CreateDelegate(choicesMethod);
        }

        public override (string, IObjectSpecImmutable)[] ParameterNamesAndTypes { get; }

        public override bool IsMultiple { get; }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => choicesMethod;

        public Func<object, object[], object> GetMethodDelegate() => choicesDelegate;

        #endregion

        public override object[] GetChoices(INakedObjectAdapter nakedObjectAdapter, IDictionary<string, INakedObjectAdapter> parameterNameValues) {
            var parms = FacetUtils.MatchParameters(parameterNames, parameterNameValues);

            try {
                var options = choicesDelegate(nakedObjectAdapter.GetDomainObject(), parms.Select(p => p.GetDomainObject()).ToArray()) as IEnumerable;
                if (options != null) {
                    return options.Cast<object>().ToArray();
                }

                throw new NakedObjectDomainException(Log.LogAndReturn($"Must return IEnumerable from choices method: {choicesMethod.Name}"));
            }
            catch (ArgumentException ae) {
                throw new InvokeException(Log.LogAndReturn($"Choices exception: {choicesMethod.Name} has mismatched (ie type of choices parameter does not match type of action parameter) parameter types"), ae);
            }
        }

        protected override string ToStringValues() => $"method={choicesMethod},Type={choicesType}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => choicesDelegate = DelegateUtils.CreateDelegate(choicesMethod);
    }

    // Copyright (c) Naked Objects Group Ltd.
}