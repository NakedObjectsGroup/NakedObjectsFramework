// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class NakedObjectActionImpl : NakedObjectMemberSessionAware, INakedObjectAction {
        private static readonly ILog Log;
        private readonly IMetamodelManager metamodel;
        private readonly INakedObjectActionPeer nakedObjectActionPeer;
        private INakedObjectActionParameter[] parameters;

        static NakedObjectActionImpl() {
            Log = LogManager.GetLogger(typeof (NakedObjectActionImpl));
        }

        public NakedObjectActionImpl(IMetamodelManager metamodel, INakedObjectActionPeer nakedObjectActionPeer)
            : base(nakedObjectActionPeer.Identifier.MemberName, nakedObjectActionPeer) {
            this.metamodel = metamodel;
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
            get {
                return GetFacet<IExecutedFacet>().ExecutedWhere();
            }
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

        public virtual INakedObject Execute(INakedObject nakedObject, INakedObject[] parameterSet, ILifecycleManager persistor, ISession session) {
            Log.DebugFormat("Execute action {0}.{1}", nakedObject, Id);
            INakedObject[] parms = RealParameters(nakedObject, parameterSet);
            INakedObject target = RealTarget(nakedObject, persistor);
            return GetFacet<IActionInvocationFacet>().Invoke(target, parms, persistor, session);
        }

        public virtual INakedObject RealTarget(INakedObject target, ILifecycleManager persistor) {
            if (target == null) {
                return FindService(persistor);
            }
            if (target.Specification.IsService) {
                return target;
            }
            if (IsContributedMethod) {
                return FindService(persistor);
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
            get {              
                return parameters;
            }
        }

        private void BuildParameters() {
            var list = new List<INakedObjectActionParameter>();
            INakedObjectActionParamPeer[] paramPeers = nakedObjectActionPeer.Parameters;
            for (int i = 0; i < paramPeers.Length; i++) {
                var specification = paramPeers[i].Specification;

                if (specification.IsParseable) {
                    list.Add(new NakedObjectActionParameterParseable(metamodel, i, this, paramPeers[i]));
                }
                else if (specification.IsObject) {
                    list.Add(new OneToOneActionParameterImpl(metamodel, i, this, paramPeers[i]));
                }
                else if (specification.IsCollection) {
                    list.Add(new OneToManyActionParameterImpl(metamodel, i, this, paramPeers[i]));
                }
                else {
                    throw new UnknownTypeException(specification);
                }
            }
            parameters = list.ToArray();
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
        public virtual IConsent IsParameterSetValid(ISession session, INakedObject nakedObject, INakedObject[] parameterSet, ILifecycleManager persistor) {
            InteractionContext ic;
            var buf = new InteractionBuffer();
            if (parameterSet != null) {
                INakedObject[] parms = RealParameters(nakedObject, parameterSet);
                for (int i = 0; i < parms.Length; i++) {
                    ic = InteractionContext.ModifyingPropParam(session, false, RealTarget(nakedObject, persistor), Identifier, parameterSet[i]);
                    InteractionUtils.IsValid(GetParameter(i), ic, buf);
                }
            }
            INakedObject target = RealTarget(nakedObject, persistor);
            ic = InteractionContext.InvokingAction(session, false, target, Identifier, parameterSet);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public override IConsent IsUsable(ISession session, INakedObject target, ILifecycleManager persistor) {
            InteractionContext ic = InteractionContext.InvokingAction(session, false, RealTarget(target, persistor), Identifier, new[] {target});
            return InteractionUtils.IsUsable(this, ic);
        }

        public override bool IsVisible(ISession session, INakedObject target, ILifecycleManager persistor) {
            return base.IsVisible(session, RealTarget(target, persistor), persistor);
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

        #endregion

        public INakedObject[] RealParameters(INakedObject target, INakedObject[] parameterSet) {
            return parameterSet ?? (IsContributedMethod ? new[] {target} : new INakedObject[0]);
        }

        private bool FindServiceOnSpecOrSpecSuperclass(INakedObjectSpecification spec) {
            if (spec == null) {
                return false;
            }
            if (spec.Equals(OnType)) {
                return true;
            }
            return FindServiceOnSpecOrSpecSuperclass(spec.Superclass);
        }

        private INakedObject FindService(ILifecycleManager persistor) {
            foreach (INakedObject serviceAdapter in persistor.GetServices(ServiceTypes.Menu | ServiceTypes.Contributor)) {
                if (FindServiceOnSpecOrSpecSuperclass(serviceAdapter.Specification)) {
                    return serviceAdapter;
                }
            }
            throw new FindObjectException("failed to find service for action " + GetName(persistor));
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