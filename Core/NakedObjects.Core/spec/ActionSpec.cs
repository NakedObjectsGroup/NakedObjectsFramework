// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Interactions;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    internal class ActionSpec : MemberSpecAbstract, IActionSpec {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ActionSpec));
        private readonly IActionSpecImmutable actionSpecImmutable;
        private readonly SpecFactory memberFactory;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IActionParameterSpec[] parametersSpec;
        private readonly IServicesManager servicesManager;
        private IObjectSpec elementSpec;
        private Where? executedWhere;
        private bool? hasReturn;
        private bool? isFinderMethod;
        private ITypeSpec onSpec;
        // cached values     
        private IObjectSpec returnSpec;

        public ActionSpec(SpecFactory memberFactory, IMetamodelManager metamodel, ILifecycleManager lifecycleManager, ISession session, IServicesManager servicesManager, INakedObjectManager nakedObjectManager, IActionSpecImmutable actionSpecImmutable)
            : base(actionSpecImmutable.Identifier.MemberName, actionSpecImmutable, session, lifecycleManager, metamodel) {
            Assert.AssertNotNull(memberFactory);
            Assert.AssertNotNull(servicesManager);
            Assert.AssertNotNull(nakedObjectManager);
            Assert.AssertNotNull(actionSpecImmutable);

            this.memberFactory = memberFactory;
            this.servicesManager = servicesManager;
            this.nakedObjectManager = nakedObjectManager;
            this.actionSpecImmutable = actionSpecImmutable;
            int index = 0;
            parametersSpec = this.actionSpecImmutable.Parameters.Select(pp => this.memberFactory.CreateParameter(pp, this, index++)).ToArray();
        }

        private IActionInvocationFacet ActionInvocationFacet {
            get { return actionSpecImmutable.GetFacet<IActionInvocationFacet>(); }
        }

        public virtual IActionSpec[] Actions {
            get { return new IActionSpec[0]; }
        }

        #region IActionSpec Members

        public override IObjectSpec ReturnSpec {
            get { return returnSpec ?? (returnSpec = MetamodelManager.GetSpecification(actionSpecImmutable.ReturnSpec)); }
        }

        public override IObjectSpec ElementSpec {
            get { return elementSpec ?? (elementSpec = MetamodelManager.GetSpecification(actionSpecImmutable.ElementSpec)); }
        }

        public virtual ITypeSpec OnSpec {
            get { return onSpec ?? (onSpec = MetamodelManager.GetSpecification(ActionInvocationFacet.OnType)); }
        }

        public override Type[] FacetTypes {
            get { return actionSpecImmutable.FacetTypes; }
        }

        public override IIdentifier Identifier {
            get { return actionSpecImmutable.Identifier; }
        }

        public virtual int ParameterCount {
            get { return actionSpecImmutable.Parameters.Length; }
        }

        public virtual Where ExecutedWhere {
            get {
                if (!executedWhere.HasValue) {
                    executedWhere = GetFacet<IExecutedFacet>().ExecutedWhere();
                }
                return executedWhere.Value;
            }
        }

        public virtual bool IsContributedMethod {
            get { return actionSpecImmutable.IsContributedMethod; }
        }

        public bool IsFinderMethod {
            get {
                if (!isFinderMethod.HasValue) {
                    isFinderMethod = actionSpecImmutable.IsFinderMethod;
                }
                return isFinderMethod.Value;
            }
        }

        public virtual INakedObject Execute(INakedObject nakedObject, INakedObject[] parameterSet) {
            Log.DebugFormat("Execute action {0}.{1}", nakedObject, Id);
            INakedObject[] parms = RealParameters(nakedObject, parameterSet);
            INakedObject target = RealTarget(nakedObject);
            var result = ActionInvocationFacet.Invoke(target, parms, LifecycleManager, MetamodelManager, Session, nakedObjectManager);

            if (result != null && result.Oid == null) {
                result.SetATransientOid(new CollectionMemento(LifecycleManager, nakedObjectManager, MetamodelManager, nakedObject, this, parameterSet));
            }
            return result;
        }

        public virtual INakedObject RealTarget(INakedObject target) {
            if (target == null) {
                return FindService();
            }
            if (target.Spec is IServiceSpec) {
                return target;
            }
            if (IsContributedMethod) {
                return FindService();
            }
            return target;
        }

        public override bool ContainsFacet(Type facetType) {
            return actionSpecImmutable.ContainsFacet(facetType);
        }

        public override IFacet GetFacet(Type type) {
            return actionSpecImmutable.GetFacet(type);
        }

        public override IEnumerable<IFacet> GetFacets() {
            return actionSpecImmutable.GetFacets();
        }

        public virtual IActionParameterSpec[] Parameters {
            get { return parametersSpec; }
        }

        /// <summary>
        ///     Returns true if the represented action returns something, else returns false
        /// </summary>
        public virtual bool HasReturn {
            get {
                if (!hasReturn.HasValue) {
                    hasReturn = ReturnSpec != null;
                }

                return hasReturn.Value;
            }
        }

        /// <summary>
        ///     Checks declarative constraints, and then checks imperatively.
        /// </summary>
        public virtual IConsent IsParameterSetValid(INakedObject nakedObject, INakedObject[] parameterSet) {
            IInteractionContext ic;
            var buf = new InteractionBuffer();
            if (parameterSet != null) {
                INakedObject[] parms = RealParameters(nakedObject, parameterSet);
                for (int i = 0; i < parms.Length; i++) {
                    ic = InteractionContext.ModifyingPropParam(Session, false, RealTarget(nakedObject), Identifier, parameterSet[i]);
                    InteractionUtils.IsValid(GetParameter(i), ic, buf);
                }
            }
            INakedObject target = RealTarget(nakedObject);
            ic = InteractionContext.InvokingAction(Session, false, target, Identifier, parameterSet);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public override IConsent IsUsable(INakedObject target) {
            IInteractionContext ic = InteractionContext.InvokingAction(Session, false, RealTarget(target), Identifier, new[] {target});
            return InteractionUtils.IsUsable(this, ic);
        }

        public override bool IsVisible(INakedObject target) {
            return base.IsVisible(RealTarget(target));
        }

        public INakedObject[] RealParameters(INakedObject target, INakedObject[] parameterSet) {
            return parameterSet ?? (IsContributedMethod ? new[] {target} : new INakedObject[0]);
        }

        #endregion

        private bool FindServiceOnSpecOrSpecSuperclass(ITypeSpec spec) {
            return spec != null && (spec.Equals(OnSpec) || FindServiceOnSpecOrSpecSuperclass(spec.Superclass));
        }

        private INakedObject FindService() {
            foreach (INakedObject serviceAdapter in servicesManager.GetServices()) {
                if (FindServiceOnSpecOrSpecSuperclass(serviceAdapter.Spec)) {
                    return serviceAdapter;
                }
            }
            throw new FindObjectException("failed to find service for action " + Name);
        }

        private IActionParameterSpec GetParameter(int position) {
            if (position >= Parameters.Length) {
                throw new ArgumentException("GetParameter(int): only " + Parameters.Length + " parameters, position=" + position);
            }
            return Parameters[position];
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.Append("Action [");
            sb.Append(base.ToString());
            sb.Append(",returns=");
            sb.Append(ReturnSpec);
            sb.Append(",parameters={");
            for (int i = 0; i < ParameterCount; i++) {
                if (i > 0) {
                    sb.Append(",");
                }
                sb.Append(Parameters[i].Spec.ShortName);
            }
            sb.Append("}]");
            return sb.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}