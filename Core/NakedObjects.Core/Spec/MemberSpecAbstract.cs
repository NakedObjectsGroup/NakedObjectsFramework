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
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Architecture.Reflect {
    public abstract class MemberSpecAbstract : IMemberSpec {
        private readonly string defaultName;
        private readonly string id;
        private readonly ILifecycleManager lifecycleManager;
        private readonly IMetamodelManager metamodelManager;
        private readonly ISession session;
        private readonly IMemberSpecImmutable memberSpecImmutable;


        protected internal MemberSpecAbstract(string id, IMemberSpecImmutable memberSpec, ISession session, ILifecycleManager lifecycleManager, IMetamodelManager metamodelManager) {
            AssertArgNotNull(id, Resources.NakedObjects.NameNotSetMessage);
            AssertArgNotNull(memberSpec);
            AssertArgNotNull(session);
            AssertArgNotNull(lifecycleManager);

            this.id = id;
            defaultName = NameUtils.NaturalName(id);
            memberSpecImmutable = memberSpec;
            this.session = session;
            this.lifecycleManager = lifecycleManager;
            this.metamodelManager = metamodelManager;
        }

        public ISession Session {
            get { return session; }
        }

        public ILifecycleManager LifecycleManager {
            get { return lifecycleManager; }
        }

        protected IMetamodelManager MetamodelManager {
            get { return metamodelManager; }
        }

        #region IMemberSpec Members

        public virtual string Id {
            get { return id; }
        }

        public virtual IIdentifier Identifier {
            get { return memberSpecImmutable.Identifier; }
        }

        public virtual Type[] FacetTypes {
            get { return memberSpecImmutable.FacetTypes; }
        }

        /// <summary>
        ///     Return the default label for this member. This is based on the name of this member.
        /// </summary>
        /// <seealso cref="Id()" />
        public virtual string GetName() {
            return memberSpecImmutable.GetName();
        }

        public virtual string Description {
            get { return memberSpecImmutable.Description; }
        }

        public abstract IObjectSpec Spec { get; }
        public abstract IObjectSpec ElementSpec { get; }


        public virtual bool ContainsFacet(Type facetType) {
            return memberSpecImmutable.ContainsFacet(facetType);
        }

        public virtual bool ContainsFacet<T>() where T : IFacet {
            return memberSpecImmutable.ContainsFacet<T>();
        }

        public virtual IFacet GetFacet(Type type) {
            return memberSpecImmutable.GetFacet(type);
        }

        public virtual T GetFacet<T>() where T : IFacet {
            return memberSpecImmutable.GetFacet<T>();
        }

        public virtual IEnumerable<IFacet> GetFacets() {
            return memberSpecImmutable.GetFacets();
        }


        /// <summary>
        ///     Loops over all <see cref="IHidingInteractionAdvisor" /> <see cref="IFacet" />s and
        ///     returns <c>true</c> only if none hide the member.
        /// </summary>
        public virtual bool IsVisible(INakedObject target) {
            InteractionContext ic = InteractionContext.AccessMember(Session, false, target, Identifier);
            return InteractionUtils.IsVisible(this, ic, LifecycleManager, metamodelManager);
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
            get { return memberSpecImmutable.ContainsFacet(typeof (INullableFacet)); }
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