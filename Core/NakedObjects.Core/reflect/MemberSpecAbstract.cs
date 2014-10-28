// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Architecture.Reflect {
    public abstract class MemberSpecAbstract : IMemberSpec {
        private readonly string defaultName;
        private readonly ISpecificationBuilder specification;
        private readonly string id;
        private readonly ILifecycleManager lifecycleManager;
        private readonly ISession session;


        protected internal MemberSpecAbstract(string id, ISpecificationBuilder specification, ISession session, ILifecycleManager lifecycleManager) {
            AssertArgNotNull(id, Resources.NakedObjects.NameNotSetMessage);
            AssertArgNotNull(specification);
            AssertArgNotNull(session);
            AssertArgNotNull(lifecycleManager);

            this.id = id;
            defaultName = NameUtils.NaturalName(id);
            this.specification = specification;
            this.session = session;
            this.lifecycleManager = lifecycleManager;
        }

        public ISession Session {
            get { return session; }
        }

        public ILifecycleManager LifecycleManager {
            get { return lifecycleManager; }
        }

        #region INakedObjectMember Members

        public virtual string Id {
            get { return id; }
        }

        public virtual IIdentifier Identifier {
            get { return specification.Identifier; }
        }

        public virtual Type[] FacetTypes {
            get { return specification.FacetTypes; }
        }

        /// <summary>
        ///     Return the default label for this member. This is based on the name of this member.
        /// </summary>
        /// <seealso cref="Id()" />
        public virtual string GetName() {
            return GetFacet<INamedFacet>().Value ?? defaultName;
        }

        public virtual string Description {
            get { return GetFacet<IDescribedAsFacet>().Value; }
        }

        public abstract IObjectSpec Spec { get; }
        public abstract IObjectSpec ElementSpec { get; }


        public virtual bool ContainsFacet(Type facetType) {
            return specification.ContainsFacet(facetType);
        }

        public virtual bool ContainsFacet<T>() where T : IFacet {
            return specification.ContainsFacet<T>();
        }

        public virtual IFacet GetFacet(Type type) {
            return specification.GetFacet(type);
        }

        public virtual T GetFacet<T>() where T : IFacet {
            return specification.GetFacet<T>();
        }

        public virtual IEnumerable<IFacet> GetFacets() {
            return specification.GetFacets();
        }

        public virtual void AddFacet(IFacet facet) {
            specification.AddFacet(facet);
        }

        public virtual void AddFacet(IMultiTypedFacet facet) {
            specification.AddFacet(facet);
        }

        public virtual void RemoveFacet(IFacet facet) {
            specification.RemoveFacet(facet);
        }

        public virtual void RemoveFacet(Type facetType) {
            specification.RemoveFacet(facetType);
        }


        /// <summary>
        ///     Loops over all <see cref="IHidingInteractionAdvisor" /> <see cref="IFacet" />s and
        ///     returns <c>true</c> only if none hide the member.
        /// </summary>
        public virtual bool IsVisible(INakedObject target) {
            InteractionContext ic = InteractionContext.AccessMember(Session, false, target, Identifier);
            return InteractionUtils.IsVisible(this, ic, LifecycleManager);
        }

        /// <summary>
        ///     Loops over all <see cref="IDisablingInteractionAdvisor" /> <see cref="IFacet" />s and
        ///     returns <c>true</c> only if none disables the member.
        /// </summary>
        public virtual IConsent IsUsable(INakedObject target) {
            InteractionContext ic = InteractionContext.AccessMember(Session, false, target, Identifier);
            return InteractionUtils.IsUsable(this, ic);
        }

        public bool IsNullable {
            get { return specification.ContainsFacet(typeof (INullableFacet)); }
        }

        #endregion

        private static void AssertArgNotNull(object arg, string msg = null) {
            if (arg == null) {
                throw new ArgumentException(msg ?? "");
            }
        }

        //public override string ToString() {
        //    return "id=" + Id + ",name='" + GetName() + "'";
        //}

        protected internal virtual IConsent GetConsent(string message) {
            return message == null ? (IConsent) Allow.Default : new Veto(message);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}