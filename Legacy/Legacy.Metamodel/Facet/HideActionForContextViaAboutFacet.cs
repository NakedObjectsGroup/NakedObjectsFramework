// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Legacy.NakedObjects.Reflector.Java.Control;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Interactions;
using NakedFramework.Architecture.Spec;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Facet;

namespace Legacy.Metamodel.Facet {
    [Serializable]
    public sealed class HideActionForContextViaAboutFacet : FacetAbstract, IHideForContextFacet, IImperativeFacet {
        public enum AboutType {
            Action,
            Fields
        }


        private readonly ILogger<HideActionForContextViaAboutFacet> logger;
        private readonly MethodInfo method;
        private readonly AboutType aboutType;

        public HideActionForContextViaAboutFacet(MethodInfo method, ISpecification holder, AboutType aboutType, ILogger<HideActionForContextViaAboutFacet> logger)
            : base(typeof(IHideForContextFacet), holder) {
            this.method = method;
            this.aboutType = aboutType;
            this.logger = logger;
        }

        protected override string ToStringValues() => $"method={method}";


        #region IHideForContextFacet Members

        public string Hides(IInteractionContext ic) => HiddenReason(ic.Target, ic.Framework);

        public Exception CreateExceptionFor(IInteractionContext ic) => new HiddenException(ic, Hides(ic));

        public string HiddenReason(INakedObjectAdapter nakedObjectAdapter, INakedFramework framework) {
            if (nakedObjectAdapter == null) {
                return null;
            }

            bool isHidden;
            if (aboutType is AboutType.Action) {
                var about = new SimpleActionAbout(framework.Session, nakedObjectAdapter.Object, Array.Empty<object>());
                method.Invoke(nakedObjectAdapter.GetDomainObject(), new object[] { about });
                isHidden = about.canAccess().IsVetoed;
            }
            else {
                var about = new SimpleFieldAbout(framework.Session, nakedObjectAdapter.Object);

                var parms = method.GetParameters().Length == 1 ? new object[] { about } : new object[] { about, null };

                method.Invoke(nakedObjectAdapter.GetDomainObject(), parms);
                isHidden = about.canAccess().IsVetoed;
            }

            return isHidden ? global::NakedObjects.Resources.NakedObjects.Hidden : null;
        }

        #endregion

        #region IImperativeFacet Members

        public MethodInfo GetMethod() => method;

        public Func<object, object[], object> GetMethodDelegate() => null;

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}