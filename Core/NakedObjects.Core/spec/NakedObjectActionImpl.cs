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
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actions.Contributed;
using NakedObjects.Architecture.Facets.Actions.Executed;
using NakedObjects.Architecture.Facets.Actions.Invoke;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Services;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.spec;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class NakedObjectActionImpl : NakedObjectMemberSessionAware, INakedObjectAction {
        private static readonly ILog Log;

        private readonly MemberFactory memberFactory;
        private readonly IMetamodelManager metamodel;
        private readonly IServicesManager servicesManager;
        private readonly INakedObjectActionPeer nakedObjectActionPeer;

        private INakedObjectActionParameter[] parameters;

        static NakedObjectActionImpl() {
            Log = LogManager.GetLogger(typeof (NakedObjectActionImpl));
        }

        public NakedObjectActionImpl(MemberFactory memberFactory, IMetamodelManager metamodel, ILifecycleManager lifecycleManager, ISession session, IServicesManager servicesManager, INakedObjectActionPeer nakedObjectActionPeer)
            : base(nakedObjectActionPeer.Identifier.MemberName, nakedObjectActionPeer, session, lifecycleManager) {
            Assert.AssertNotNull(metamodel);
            Assert.AssertNotNull(nakedObjectActionPeer);

            this.memberFactory = memberFactory;
            this.metamodel = metamodel;
            this.servicesManager = servicesManager;
            this.nakedObjectActionPeer = nakedObjectActionPeer;
            BuildParameters();
        }

        private IActionInvocationFacet ActionInvocationFacet {
            get { return nakedObjectActionPeer.GetFacet<IActionInvocationFacet>(); }
        }

        #region INakedObjectAction Members

        public virtual INakedObjectSpecification ReturnType {
            get { return metamodel.GetSpecification(ActionInvocationFacet.ReturnType); }
        }

        public virtual INakedObjectSpecification OnType {
            get { return metamodel.GetSpecification(ActionInvocationFacet.OnType); }
        }

        public virtual INakedObjectAction[] Actions {
            get { return new INakedObjectAction[0]; }
        }

        public override Type[] FacetTypes {
            get { return nakedObjectActionPeer.FacetTypes; }
        }

        public override IIdentifier Identifier {
            get { return nakedObjectActionPeer.Identifier; }
        }

        public virtual int ParameterCount {
            get { return nakedObjectActionPeer.Parameters.Length; }
        }

        public virtual Where Target {
            get { return GetFacet<IExecutedFacet>().ExecutedWhere(); }
        }

        public virtual bool IsContributedMethod {
            get { return nakedObjectActionPeer.IsContributedMethod; }
        }

        public bool IsContributedTo(INakedObjectSpecification spec) {
            return IsContributedMethod
                   && Parameters.Any(parm => ContributeTo(parm.Specification, spec))
                   && !(IsCollection(spec) && IsCollection(ReturnType));
        }

        public bool IsFinderMethod {
            get { return HasReturn() && !ContainsFacet(typeof (IExcludeFromFindMenuFacet)); }
        }

        public virtual bool PromptForParameters(INakedObject nakedObject) {
            if (IsContributedMethod && !nakedObject.Specification.IsService) {
                return ParameterCount > 1 || !IsContributedTo(parameters[0].Specification);
            }
            return ParameterCount > 0;
        }

        /// <summary>
        ///     Always returns <c>null</c>
        /// </summary>
        public override INakedObjectSpecification Specification {
            get { return null; }
        }

        public virtual INakedObject Execute(INakedObject nakedObject, INakedObject[] parameterSet) {
            Log.DebugFormat("Execute action {0}.{1}", nakedObject, Id);
            INakedObject[] parms = RealParameters(nakedObject, parameterSet);
            INakedObject target = RealTarget(nakedObject);
            return GetFacet<IActionInvocationFacet>().Invoke(target, parms, LifecycleManager, Session);
        }

        public virtual INakedObject RealTarget(INakedObject target) {
            if (target == null) {
                return FindService();
            }
            if (target.Specification.IsService) {
                return target;
            }
            if (IsContributedMethod) {
                return FindService();
            }
            return target;
        }

        public override bool ContainsFacet(Type facetType) {
            return nakedObjectActionPeer.ContainsFacet(facetType);
        }

        public override IFacet GetFacet(Type type) {
            return nakedObjectActionPeer.GetFacet(type);
        }

        public override IFacet[] GetFacets(IFacetFilter filter) {
            return nakedObjectActionPeer.GetFacets(filter);
        }

        public override void AddFacet(IFacet facet) {
            nakedObjectActionPeer.AddFacet(facet);
        }

        public override void RemoveFacet(IFacet facet) {
            nakedObjectActionPeer.RemoveFacet(facet);
        }

        public override void RemoveFacet(Type facetType) {
            nakedObjectActionPeer.RemoveFacet(facetType);
        }

        public virtual INakedObjectActionParameter[] Parameters {
            get { return parameters; }
        }

        public virtual INakedObjectActionParameter[] GetParameters(INakedObjectActionParameterFilter filter) {
            return Parameters.Where(filter.Accept).ToArray();
        }

        public virtual NakedObjectActionType ActionType {
            get { return NakedObjectActionType.User; }
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
            parameters = nakedObjectActionPeer.Parameters.Select(pp => memberFactory.CreateParameter(pp, this, index++)).ToArray();
        }

        private bool ContributeTo(INakedObjectSpecification parmSpec, INakedObjectSpecification contributeeSpec) {
            //var ncf = GetFacet<INotContributedActionFacet>();

            //if (ncf == null) {
            //    return contributeeSpec.IsOfType(parmSpec);
            //}

            //return contributeeSpec.IsOfType(parmSpec) && !ncf.NotContributedTo(contributeeSpec);
            throw new NotImplementedException();
        }

        private bool IsCollection(INakedObjectSpecification spec) {
            return spec.IsCollection && !spec.IsParseable;
        }

        private bool FindServiceOnSpecOrSpecSuperclass(INakedObjectSpecification spec) {
            if (spec == null) {
                return false;
            }
            return spec.Equals(OnType) || FindServiceOnSpecOrSpecSuperclass(spec.Superclass);
        }

        private INakedObject FindService() {
            foreach (INakedObject serviceAdapter in servicesManager.GetServices(ServiceTypes.Menu | ServiceTypes.Contributor)) {
                if (FindServiceOnSpecOrSpecSuperclass(serviceAdapter.Specification)) {
                    return serviceAdapter;
                }
            }
            throw new FindObjectException("failed to find service for action " + GetName());
        }

        private INakedObjectActionParameter GetParameter(int position) {
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
                sb.Append(Parameters[i].Specification.ShortName);
            }
            sb.Append("}]");
            return sb.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}