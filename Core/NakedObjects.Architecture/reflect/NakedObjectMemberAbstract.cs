// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Propparam.Modify;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Util;

namespace NakedObjects.Architecture.Reflect {
    public abstract class NakedObjectMemberAbstract : INakedObjectMember {
        private readonly IFacetHolder facetHolder;
        private readonly string id;
        protected internal string defaultName;

        protected internal NakedObjectMemberAbstract(string id, IFacetHolder facetHolder) {
            if (id == null) {
                throw new ArgumentException(Resources.NakedObjects.NameNotSetMessage);
            }
            this.id = id;
            defaultName = NameUtils.NaturalName(id);
            this.facetHolder = facetHolder;
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
        public virtual string Name {
            get { return GetFacet<INamedFacet>().Value ?? defaultName; }
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

        public virtual IFacet[] GetFacets(IFacetFilter filter) {
            return facetHolder.GetFacets(filter);
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
        public virtual bool IsVisible(ISession session, INakedObject target) {
            InteractionContext ic = InteractionContext.AccessMember(session, false, target, Identifier);
            return InteractionUtils.IsVisible(this, ic);
        }


        /// <summary>
        ///     Loops over all <see cref="IDisablingInteractionAdvisor" /> <see cref="IFacet" />s and
        ///     returns <c>true</c> only if none disables the member.
        /// </summary>
        public virtual IConsent IsUsable(ISession session, INakedObject target) {
            InteractionContext ic = InteractionContext.AccessMember(session, false, target, Identifier);
            return InteractionUtils.IsUsable(this, ic);
        }

        public bool IsNullable {
            get { return facetHolder.ContainsFacet(typeof (INullableFacet)); }
        }

        #endregion

        public override string ToString() {
            return "id=" + Id + ",name='" + Name + "'";
        }

        protected internal virtual IConsent GetConsent(string message) {
            if (message == null) {
                return Allow.Default;
            }
            return new Veto(message);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}