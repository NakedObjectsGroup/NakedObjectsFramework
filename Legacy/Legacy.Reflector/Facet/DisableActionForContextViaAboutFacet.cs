// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Legacy.NakedObjects.Object.Control;
using Legacy.NakedObjects.Reflector.Java.Control;
using Legacy.Reflector.Component;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;

namespace Legacy.Reflector.Facet {
    [Serializable]
    public sealed class DisableActionForContextViaAboutFacet : FacetAbstract, IDisableForContextFacet, IImperativeFacet {
        public enum AboutType {
            Action,
            Fields
        }

        private readonly AboutType aboutType;

        private readonly ILogger<DisableActionForContextViaAboutFacet> logger;
        private readonly MethodInfo method;

        public DisableActionForContextViaAboutFacet(MethodInfo method, ISpecification holder, AboutType aboutType, ILogger<DisableActionForContextViaAboutFacet> logger)
            : base(typeof(IDisableForContextFacet), holder) {
            this.method = method;
            this.aboutType = aboutType;
            this.logger = logger;
        }

        protected override string ToStringValues() => $"method={method}";

        public string Disables(IInteractionContext ic) => DisabledReason(ic.Target, ic.Framework);

        public Exception CreateExceptionFor(IInteractionContext ic) => new DisabledException(ic, Disables(ic));

        public string DisabledReason(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
            if (nakedObjectAdapter == null) {
                return null;
            }

            bool isDisabled;
            if (aboutType is AboutType.Action) {
                var about =  (Hint)LegacyAboutCache.GetActionAbout(framework, method, nakedObjectAdapter.Object);
                isDisabled = about.canUse().IsVetoed;
            }
            else {
                var about = new SimpleFieldAbout(framework.Session, nakedObjectAdapter.Object);

                var parms = method.GetParameters().Length == 1 ? new object[] { about } : new object[] { about, null };

                method.Invoke(nakedObjectAdapter.GetDomainObject(), parms);
                isDisabled = about.canUse().IsVetoed;
            }

            return isDisabled ? global::NakedObjects.Resources.NakedObjects.Disabled : null;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => null;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}