// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Propparam.Modify;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Architecture.Reflect {
    public abstract class NakedObjectMemberAbstract : INakedObjectMember {
        private readonly string defaultName;
        private readonly IFacetHolder facetHolder;
        private readonly string id;
        private readonly ILifecycleManager lifecycleManager;
        private readonly ISession session;


        protected internal NakedObjectMemberAbstract(string id, IFacetHolder facetHolder, ISession session, ILifecycleManager lifecycleManager) {
            AssertArgNotNull(id, Resources.NakedObjects.NameNotSetMessage);
            AssertArgNotNull(facetHolder);
            AssertArgNotNull(session);
            AssertArgNotNull(lifecycleManager);

            this.id = id;
            defaultName = NameUtils.NaturalName(id);
            this.facetHolder = facetHolder;
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
            get { return facetHolder.Identifier; }
        }

        public virtual Type[] FacetTypes {
            get { return facetHolder.FacetTypes; }
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

        public abstract INakedObjectSpecification Specification { get; }


        public virtual bool ContainsFacet(Type facetType) {
            return facetHolder.ContainsFacet(facetType);
        }

        public virtual bool ContainsFacet<T>() where T : IFacet {
            return facetHolder.ContainsFacet<T>();
        }

        public virtual IFacet GetFacet(Type type) {
            return facetHolder.GetFacet(type);
        }

        public virtual T GetFacet<T>() where T : IFacet {
            return facetHolder.GetFacet<T>();
        }

        public virtual IEnumerable<IFacet> GetFacets() {
            return facetHolder.GetFacets();
        }

        public virtual void AddFacet(IFacet facet) {
            facetHolder.AddFacet(facet);
        }

        public virtual void AddFacet(IMultiTypedFacet facet) {
            facetHolder.AddFacet(facet);
        }

        public virtual void RemoveFacet(IFacet facet) {
            facetHolder.RemoveFacet(facet);
        }

        public virtual void RemoveFacet(Type facetType) {
            facetHolder.RemoveFacet(facetType);
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
            get { return facetHolder.ContainsFacet(typeof (INullableFacet)); }
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