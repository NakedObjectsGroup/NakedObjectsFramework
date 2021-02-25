// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class ActionParameterValidation : FacetAbstract, IActionParameterValidationFacet, IImperativeFacet {
        private readonly ILogger<ActionParameterValidation> logger;
        private readonly MethodInfo method;

        [field: NonSerialized] private Func<object, object[], object> methodDelegate;

        public ActionParameterValidation(MethodInfo method, ISpecification holder, ILogger<ActionParameterValidation> logger)
            : base(typeof(IActionParameterValidationFacet), holder) {
            this.method = method;
            this.logger = logger;
            methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);
        }

        protected override string ToStringValues() => $"method={method}";

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) => methodDelegate = LogNull(DelegateUtils.CreateDelegate(method), logger);

        #region IActionParameterValidationFacet Members

        public string Invalidates(IInteractionContext ic) => InvalidReason(ic.Target, ic.Framework, ic.ProposedArgument);

        public Exception CreateExceptionFor(IInteractionContext ic) => new ActionArgumentsInvalidException(ic, Invalidates(ic));

        public string InvalidReason(INakedObjectAdapter target, INakedObjectsFramework framework, INakedObjectAdapter proposedArgument) =>
            (string) methodDelegate(target.GetDomainObject(), new[] {proposedArgument.GetDomainObject()});

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => methodDelegate;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}