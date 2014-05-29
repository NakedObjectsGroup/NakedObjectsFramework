// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Objects.Value;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.Reflector.Spec {
    // TODO work through all subclasses and remove duplicated overridden methods - this 
    // hierarchy is badly structured move common, and default, functionality to this class from subclasses.
    public abstract class NakedObjectSpecificationAbstract : FacetHolderImpl, IIntrospectableSpecification {
        protected internal INakedObjectAssociation[] fields;
        protected internal string fullName;
        protected internal IIdentifier identifier;
        protected internal INakedObjectAction[] objectActions = new INakedObjectAction[] { };
        protected internal INakedObjectAction[] contributedActions = new INakedObjectAction[] {};
        protected internal INakedObjectAction[] relatedActions = new INakedObjectAction[] { };
        protected internal INakedObjectSpecification superClassSpecification;
        protected internal INakedObjectValidation[] validationMethods;

        /// <summary>
        ///     Whether or not this specification's type is marked as sealed,
        ///     that is it may not have subclasses, and hence methods that could be overridden.
        /// </summary>
        /// <para>
        ///     Not used at present.
        /// </para>
        public virtual bool IsSealed {
            get { return false; }
        }

        #region IIntrospectableSpecification Members

        public virtual string FullName {
            get { return fullName; }
        }

        public virtual object DefaultValue {
            get { return null; }
        }

        public override IIdentifier Identifier {
            get { return identifier; }
        }

        public virtual bool IsAbstract {
            get { return false; }
        }

        public virtual bool IsInterface {
            get { return false; }
        }

        public virtual bool IsService {
            get { return false; }
        }

        public virtual bool IsParseable {
            get { return ContainsFacet(typeof (IParseableFacet)); }
        }

        public virtual bool IsEncodeable {
            get { return ContainsFacet(typeof (IEncodeableFacet)); }
        }

        public virtual bool IsAggregated {
            get { return ContainsFacet(typeof (IAggregatedFacet)) || ContainsFacet(typeof (IValueFacet)); }
        }

        public virtual bool IsCollection {
            get { return ContainsFacet(typeof (ICollectionFacet)); }
        }

        public virtual bool IsViewModel {
            get { return ContainsFacet(typeof (IViewModelFacet)); }
        }

        public abstract bool IsQueryable { get; }

        public abstract bool IsVoid { get; }

        public virtual bool IsObject {
            get { return !IsCollection; }
        }

        public virtual string GetIconName(INakedObject forObject) {
            return null;
        }

        public virtual bool HasSubclasses {
            get { return false; }
        }

        public virtual INakedObjectSpecification[] Interfaces {
            get { return new INakedObjectSpecification[0]; }
        }

        public virtual INakedObjectSpecification[] Subclasses {
            get { return new INakedObjectSpecification[0]; }
        }

        public virtual INakedObjectSpecification Superclass {
            get { return superClassSpecification; }
        }

        public virtual bool IsOfType(INakedObjectSpecification specification) {
            return specification == this;
        }

        public virtual void AddSubclass(INakedObjectSpecification specification) {}

        public override IFacet GetFacet(Type facetType) {
            IFacet facet = base.GetFacet(facetType);
            if (FacetUtils.IsNotANoopFacet(facet)) {
                return facet;
            }

            IFacet noopFacet = facet;

            if (Superclass != null) {
                IFacet superClassFacet = Superclass.GetFacet(facetType);
                if (FacetUtils.IsNotANoopFacet(superClassFacet)) {
                    return superClassFacet;
                }
                if (noopFacet == null) {
                    noopFacet = superClassFacet;
                }
            }
            if (Interfaces != null) {
                INakedObjectSpecification[] interfaceSpecs = Interfaces;
                foreach (INakedObjectSpecification interfaceSpec in interfaceSpecs) {
                    IFacet interfaceFacet = interfaceSpec.GetFacet(facetType);
                    if (FacetUtils.IsNotANoopFacet(interfaceFacet)) {
                        return interfaceFacet;
                    }
                    if (noopFacet == null) {
                        noopFacet = interfaceFacet;
                    }
                }
            }
            return noopFacet;
        }

        public virtual INakedObjectAssociation[] Properties {
            get { return fields; }
        }

        public INakedObjectValidation[] ValidateMethods() {
            return validationMethods;
        }

        public virtual INakedObjectAction[] GetObjectActions() {
            var combinedActions = new List<INakedObjectAction>();
            combinedActions.AddRange(objectActions);
            combinedActions.AddRange(contributedActions);
            return combinedActions.ToArray();
        }

        
        public virtual INakedObjectAction[] GetRelatedServiceActions() {
            return relatedActions;
        }

        public virtual IConsent ValidToPersist(INakedObject transientObject) {
            return Allow.Default;
        }

        public virtual bool HasNoIdentity {
            get { return false; }
        }

        public virtual Persistable Persistable {
            get { return Persistable.USER_PERSISTABLE; }
        }

        public bool IsASet {
            get {
                var collectionFacet = GetFacet<ICollectionFacet>();
                if (collectionFacet != null) {
                    return collectionFacet.IsASet;
                }
                return false;
            }
        }

        public abstract object CreateObject();
        public abstract IEnumerable GetBoundedSet();
        public abstract void Introspect(FacetDecoratorSet decoratorSet);
        public abstract void PopulateAssociatedActions(INakedObject[] services);
        public abstract string PluralName { get; }
        public abstract string ShortName { get; }
        public abstract string Description { get; }
        public abstract string SingularName { get; }
        public abstract string UntitledName { get; }
        public abstract string GetTitle(INakedObject nakedObject);
        public abstract INakedObjectAssociation GetProperty(string id);

        #endregion

        public virtual void MarkAsService() {}

        public override string ToString() {
            var str = new AsString(this);
            str.Append("class", fullName);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}