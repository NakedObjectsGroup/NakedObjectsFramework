// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Architecture.Facets {
    public abstract class FacetAbstract : IFacet {
        private readonly Type facetType;
        private IFacetHolder holder;

        protected FacetAbstract(Type facetType, IFacetHolder holder) {
            this.facetType = facetType;
            this.holder = holder;
        }

        /// <summary>
        ///     Convenience for creating exceptions by those facets that implement
        ///     <see cref="IValidatingInteractionAdvisor" />, <see cref="IHidingInteractionAdvisor" /> or
        ///     <see cref="IDisablingInteractionAdvisor" />
        /// </summary>
        protected internal virtual string FacetHolderId {
            get { return FacetHolder.Identifier.ToIdentityString(IdentifierDepth.Class); }
        }

        #region IFacet Members

        public virtual IFacetHolder FacetHolder {
            get { return holder; }
            set { holder = value; }
        }

        /// <summary>
        ///     Assume implementation is <i>not</i> a no-op.
        /// </summary>
        /// <para>
        ///     No-op implementations should override and return <c>true</c>.
        /// </para>
        public virtual bool IsNoOp {
            get { return false; }
        }

        public Type FacetType {
            get { return facetType; }
        }


        /// <summary>
        ///     Default implementation of this method that returns <c>true</c>, ie
        ///     should replace non-<see cref="IsNoOp" /> implementations.
        /// </summary>
        /// <para>
        ///     Implementations that don't wish to replace non-<see cref="IsNoOp" /> implementations
        ///     should override and return <c>false</c>.
        /// </para>
        public virtual bool CanAlwaysReplace {
            get { return true; }
        }

        #endregion

        public virtual void Reparent(IFacetHolder newFacetHolder) {
            IFacetHolder oldFacetHolder = FacetHolder;

            oldFacetHolder.RemoveFacet(this);

            newFacetHolder.AddFacet(this);
            if (FacetHolder != newFacetHolder) {
                FacetHolder = newFacetHolder;
            }
        }

        public override string ToString() {
            string details = "";
            if (typeof (IValidatingInteractionAdvisor).IsAssignableFrom(GetType())) {
                details += "Validating";
            }
            if (typeof (IDisablingInteractionAdvisor).IsAssignableFrom(GetType())) {
                details += (details.Length > 0 ? ";" : "") + "Disabling";
            }
            if (typeof (IHidingInteractionAdvisor).IsAssignableFrom(GetType())) {
                details += (details.Length > 0 ? ";" : "") + "Hiding";
            }
            if (!"".Equals(details)) {
                details = "interaction=" + details + ",";
            }
            if (GetType() != FacetType) {
                string sFacetType = FacetType.FullName;
                details += "type=" + sFacetType.Substring(sFacetType.LastIndexOf('.') + 1);
            }
            string stringValues = ToStringValues();
            if (!"".Equals(stringValues)) {
                details += ",";
            }
            string typeName = GetType().FullName;
            int last = typeName.IndexOf('`');
            if (last == -1) {
                last = typeName.Length - 1;
            }
            return typeName.Substring(typeName.LastIndexOf('.', last) + 1) + "[" + details + stringValues + "]";
        }


        /// <summary>
        ///     For convenience of subclass facets that implement
        ///     <see cref="IValidatingInteractionAdvisor" />, <see cref="IHidingInteractionAdvisor" /> or
        ///     <see cref="IDisablingInteractionAdvisor" />
        /// </summary>
        protected internal virtual string UnwrapString(INakedObject nakedObject) {
            object obj = nakedObject.GetDomainObject();
            if (obj == null) {
                return null;
            }
            if (!(obj is string)) {
                return null;
            }
            return (string) obj;
        }

        protected virtual string ToStringValues() {
            return "";
        }
    }
}