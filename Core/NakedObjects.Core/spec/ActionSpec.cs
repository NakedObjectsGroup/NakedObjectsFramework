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
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public class ActionSpec : MemberSpecAbstract, IActionSpec {
        private static readonly ILog Log;
        private readonly IActionSpecImmutable actionSpecImmutable;

        private readonly SpecFactory memberFactory;
        private readonly IMetamodelManager metamodel;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IServicesManager servicesManager;
        private readonly ITransactionManager transactionManager;

        private IActionParameterSpec[] parametersSpec;

        static ActionSpec() {
            Log = LogManager.GetLogger(typeof (ActionSpec));
        }

        public ActionSpec(SpecFactory memberFactory, IMetamodelManager metamodel, ILifecycleManager lifecycleManager, ISession session, IServicesManager servicesManager, ITransactionManager transactionManager, INakedObjectManager nakedObjectManager, IActionSpecImmutable actionSpecImmutable)
            : base(actionSpecImmutable.Identifier.MemberName, actionSpecImmutable, session, lifecycleManager) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(actionSpecImmutable);

            this.memberFactory = memberFactory;
            this.metamodel = metamodel;
            this.servicesManager = servicesManager;
            this.transactionManager = transactionManager;
            this.nakedObjectManager = nakedObjectManager;
            this.actionSpecImmutable = actionSpecImmutable;
            BuildParameters();
        }

        private IActionInvocationFacet ActionInvocationFacet {
            get { return actionSpecImmutable.GetFacet<IActionInvocationFacet>(); }
        }

        #region IActionSpec Members

        public virtual IObjectSpec ReturnType {
            get { return metamodel.GetSpecification(ActionInvocationFacet.ReturnType); }
        }

        public virtual IObjectSpec OnType {
            get { return metamodel.GetSpecification(ActionInvocationFacet.OnType); }
        }

        public virtual IActionSpec[] Actions {
            get { return new IActionSpec[0]; }
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

        public virtual Where Target {
            get { return GetFacet<IExecutedFacet>().ExecutedWhere(); }
        }

        public virtual bool IsContributedMethod {
            get { return actionSpecImmutable.IsContributedMethod; }
        }

        public bool IsContributedTo(IObjectSpec spec) {
            return IsContributedMethod
                   && Parameters.Any(parm => ContributeTo(parm.Spec, spec))
                   && !(IsCollection(spec) && IsCollection(ReturnType));
        }

        public bool IsFinderMethod {
            get { return HasReturn() && !ContainsFacet(typeof (IExcludeFromFindMenuFacet)); }
        }

        public virtual bool PromptForParameters(INakedObject nakedObject) {
            if (IsContributedMethod && !nakedObject.Spec.IsService) {
                return ParameterCount > 1 || !IsContributedTo(parametersSpec[0].Spec);
            }
            return ParameterCount > 0;
        }

        /// <summary>
        ///     Always returns <c>null</c>
        /// </summary>
        public override IObjectSpec Spec {
            get { return null; }
        }

        public virtual INakedObject Execute(INakedObject nakedObject, INakedObject[] parameterSet) {
            Log.DebugFormat("Execute action {0}.{1}", nakedObject, Id);
            INakedObject[] parms = RealParameters(nakedObject, parameterSet);
            INakedObject target = RealTarget(nakedObject);
            return GetFacet<IActionInvocationFacet>().Invoke(target, parms, nakedObjectManager, Session, transactionManager);
        }

        public virtual INakedObject RealTarget(INakedObject target) {
            if (target == null) {
                return FindService();
            }
            if (target.Spec.IsService) {
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

        public override void AddFacet(IFacet facet) {
            actionSpecImmutable.AddFacet(facet);
        }

        public override void RemoveFacet(IFacet facet) {
            actionSpecImmutable.RemoveFacet(facet);
        }

        public override void RemoveFacet(Type facetType) {
            actionSpecImmutable.RemoveFacet(facetType);
        }

        public virtual IActionParameterSpec[] Parameters {
            get { return parametersSpec; }
        }

        public virtual ActionType ActionType {
            get { return ActionType.User; }
        }

        /// <summary>
        ///     Returns true if the represented action returns something, else returns false
        /// </summary>
        public virtual bool HasReturn() {
            return ReturnType != null;
        }

        /// <summary>
        ///     Checks declarative constraints, and then checks imperatively.
        /// </summary>
        public virtual IConsent IsParameterSetValid(INakedObject nakedObject, INakedObject[] parameterSet) {
            InteractionContext ic;
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
            InteractionContext ic = InteractionContext.InvokingAction(Session, false, RealTarget(target), Identifier, new[] {target});
            return InteractionUtils.IsUsable(this, ic);
        }

        public override bool IsVisible(INakedObject target) {
            return base.IsVisible(RealTarget(target));
        }

        public INakedObject[] RealParameters(INakedObject target, INakedObject[] parameterSet) {
            return parameterSet ?? (IsContributedMethod ? new[] {target} : new INakedObject[0]);
        }

        #endregion

        private void BuildParameters() {
            int index = 0;
            parametersSpec = actionSpecImmutable.Parameters.Select(pp => memberFactory.CreateParameter(pp, this, index++)).ToArray();
        }

        private bool ContributeTo(IObjectSpec parmSpec, IObjectSpec contributeeSpec) {
            //var ncf = GetFacet<INotContributedActionFacet>();

            //if (ncf == null) {
            //    return contributeeSpec.IsOfType(parmSpec);
            //}

            //return contributeeSpec.IsOfType(parmSpec) && !ncf.NotContributedTo(contributeeSpec);
            throw new NotImplementedException();
        }

        private bool IsCollection(IObjectSpec spec) {
            return spec.IsCollection && !spec.IsParseable;
        }

        private bool FindServiceOnSpecOrSpecSuperclass(IObjectSpec spec) {
            if (spec == null) {
                return false;
            }
            return spec.Equals(OnType) || FindServiceOnSpecOrSpecSuperclass(spec.Superclass);
        }

        private INakedObject FindService() {
            foreach (INakedObject serviceAdapter in servicesManager.GetServices(ServiceType.Menu | ServiceType.Contributor)) {
                if (FindServiceOnSpecOrSpecSuperclass(serviceAdapter.Spec)) {
                    return serviceAdapter;
                }
            }
            throw new FindObjectException("failed to find service for action " + GetName());
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
            sb.Append(",type=");
            sb.Append(ActionType);
            sb.Append(",returns=");
            sb.Append(ReturnType);
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