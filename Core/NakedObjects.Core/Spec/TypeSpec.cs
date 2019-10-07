// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;

namespace NakedObjects.Core.Spec {
    public abstract class TypeSpec : ITypeSpec {
        private readonly IMetamodelManager metamodelManager;

        private readonly INakedObjectManager nakedObjectManager;

        // cached values 
        private string description;
        private bool? hasNoIdentity;
        private bool? hasSubclasses;
        private ITypeSpec[] interfaces;
        private bool? isAbstract;
        private bool? isAggregated;
        private bool? isASet;
        private bool? isCollection;
        private bool? isEncodeable;
        private bool? isInterface;
        private bool? isParseable;
        private bool? isQueryable;
        private bool? isViewModel;
        private bool? isVoid;
        private IActionSpec[] objectActions;
        private PersistableType? persistable;
        private string pluralName;
        private string shortName;
        private string singularName;
        private ITypeSpec[] subclasses;
        private ITypeSpec superclass;
        private string untitledName;

        protected TypeSpec(SpecFactory memberFactory, IMetamodelManager metamodelManager, INakedObjectManager nakedObjectManager, ITypeSpecImmutable innerSpec) {
            Assert.AssertNotNull(memberFactory);
            Assert.AssertNotNull(metamodelManager);
            Assert.AssertNotNull(nakedObjectManager);
            Assert.AssertNotNull(innerSpec);

            this.MemberFactory = memberFactory;
            this.metamodelManager = metamodelManager;
            this.nakedObjectManager = nakedObjectManager;
            this.InnerSpec = innerSpec;
        }

        private Type Type {
            get { return InnerSpec.Type; }
        }

        protected IActionSpec[] ObjectActions {
            get { return objectActions ?? (objectActions = MemberFactory.CreateActionSpecs(InnerSpec.ObjectActions)); }
        }

        protected SpecFactory MemberFactory { get; }

        #region ITypeSpec Members

        public ITypeSpecImmutable InnerSpec { get; }

        public virtual string FullName {
            get { return InnerSpec.FullName; }
        }

        public virtual object DefaultValue {
            get { return null; }
        }

        public Type[] FacetTypes {
            get { return InnerSpec.FacetTypes; }
        }

        public IIdentifier Identifier {
            get { return InnerSpec.Identifier; }
        }

        public bool ContainsFacet(Type facetType) {
            return InnerSpec.ContainsFacet(facetType);
        }

        public bool ContainsFacet<T>() where T : IFacet {
            return InnerSpec.ContainsFacet<T>();
        }

        public IFacet GetFacet(Type type) {
            return InnerSpec.GetFacet(type);
        }

        public T GetFacet<T>() where T : IFacet {
            return InnerSpec.GetFacet<T>();
        }

        public IEnumerable<IFacet> GetFacets() {
            return InnerSpec.GetFacets();
        }

        public virtual bool IsParseable {
            get {
                if (!isParseable.HasValue) {
                    isParseable = InnerSpec.ContainsFacet(typeof(IParseableFacet));
                }

                return isParseable.Value;
            }
        }

        public virtual bool IsEncodeable {
            get {
                if (!isEncodeable.HasValue) {
                    isEncodeable = InnerSpec.ContainsFacet(typeof(IEncodeableFacet));
                }

                return isEncodeable.Value;
            }
        }

        public virtual bool IsAggregated {
            get {
                if (!isAggregated.HasValue) {
                    isAggregated = InnerSpec.ContainsFacet(typeof(IAggregatedFacet));
                }

                return isAggregated.Value;
            }
        }

        public virtual bool IsCollection {
            get {
                if (!isCollection.HasValue) {
                    isCollection = InnerSpec.ContainsFacet(typeof(ICollectionFacet));
                }

                return isCollection.Value;
            }
        }

        public virtual bool IsViewModel {
            get {
                if (!isViewModel.HasValue) {
                    isViewModel = InnerSpec.ContainsFacet(typeof(IViewModelFacet));
                }

                return isViewModel.Value;
            }
        }

        public virtual bool IsObject {
            get { return !IsCollection; }
        }

        public virtual ITypeSpec Superclass {
            get {
                if (superclass == null && InnerSpec.Superclass != null) {
                    superclass = metamodelManager.GetSpecification(InnerSpec.Superclass);
                }

                return superclass;
            }
        }

        public abstract IActionSpec[] GetActions();

        public IMenuImmutable Menu {
            get { return InnerSpec.ObjectMenu; }
        }

        public bool IsASet {
            get {
                if (!isASet.HasValue) {
                    var collectionFacet = InnerSpec.GetFacet<ICollectionFacet>();
                    isASet = collectionFacet != null && collectionFacet.IsASet;
                }

                return isASet.Value;
            }
        }

        public bool HasSubclasses {
            get {
                if (!hasSubclasses.HasValue) {
                    hasSubclasses = InnerSpec.Subclasses.Any();
                }

                return hasSubclasses.Value;
            }
        }

        public ITypeSpec[] Interfaces {
            get { return interfaces ?? (interfaces = InnerSpec.Interfaces.Select(i => metamodelManager.GetSpecification(i)).ToArray()); }
        }

        public ITypeSpec[] Subclasses {
            get { return subclasses ?? (subclasses = InnerSpec.Subclasses.Select(i => metamodelManager.GetSpecification(i)).ToArray()); }
        }

        public bool IsAbstract {
            get {
                if (!isAbstract.HasValue) {
                    isAbstract = InnerSpec.GetFacet<ITypeIsAbstractFacet>().Flag;
                }

                return isAbstract.Value;
            }
        }

        public bool IsInterface {
            get {
                if (!isInterface.HasValue) {
                    isInterface = InnerSpec.GetFacet<ITypeIsInterfaceFacet>().Flag;
                }

                return isInterface.Value;
            }
        }

        public string ShortName {
            get {
                if (shortName == null) {
                    string postfix = "";
                    if (Type.IsGenericType && !IsCollection) {
                        postfix = Type.GetGenericArguments().Aggregate(string.Empty, (x, y) => x + "-" + metamodelManager.GetSpecification(y).ShortName);
                    }

                    shortName = InnerSpec.ShortName + postfix;
                }

                return shortName;
            }
        }

        public string SingularName {
            get { return singularName ?? (singularName = InnerSpec.GetFacet<INamedFacet>().NaturalName); }
        }

        public string UntitledName {
            get { return untitledName ?? (untitledName = Resources.NakedObjects.Untitled + SingularName); }
        }

        public string PluralName {
            get { return pluralName ?? (pluralName = InnerSpec.GetFacet<IPluralFacet>().Value); }
        }

        public string Description {
            get { return description ?? (description = InnerSpec.GetFacet<IDescribedAsFacet>().Value ?? ""); }
        }

        public bool HasNoIdentity {
            get {
                if (!hasNoIdentity.HasValue) {
                    hasNoIdentity = InnerSpec.GetFacet<ICollectionFacet>() != null || InnerSpec.GetFacet<IParseableFacet>() != null;
                }

                return hasNoIdentity.Value;
            }
        }

        public bool IsQueryable {
            get {
                if (!isQueryable.HasValue) {
                    isQueryable = InnerSpec.IsQueryable;
                }

                return isQueryable.Value;
            }
        }

        public bool IsVoid {
            get {
                if (!isVoid.HasValue) {
                    isVoid = InnerSpec.GetFacet<ITypeIsVoidFacet>().Flag;
                }

                return isVoid.Value;
            }
        }

        public PersistableType Persistable {
            get {
                if (!persistable.HasValue) {
                    persistable = GetPersistable();
                }

                return persistable.Value;
            }
        }

        /// <summary>
        ///     Determines if this class represents the same class, or a subclass, of the specified class.
        /// </summary>
        public bool IsOfType(ITypeSpec spec) {
            return InnerSpec.IsOfType(spec.InnerSpec);
        }

        public string GetTitle(INakedObjectAdapter nakedObjectAdapter) {
            var titleFacet = GetFacet<ITitleFacet>();
            string title = titleFacet == null ? null : titleFacet.GetTitle(nakedObjectAdapter, nakedObjectManager);
            return title ?? DefaultTitle();
        }

        public string GetInvariantString(INakedObjectAdapter nakedObjectAdapter) {
            var parser = GetFacet<IParseableFacet>();
            return parser == null ? null : parser.InvariantString(nakedObjectAdapter);
        }

        public string GetIconName(INakedObjectAdapter forObjectAdapter) {
            return InnerSpec.GetIconName(forObjectAdapter, metamodelManager.Metamodel);
        }

        #endregion

        private string DefaultTitle() {
            return InnerSpec is IServiceSpecImmutable ? SingularName : UntitledName;
        }

        protected abstract PersistableType GetPersistable();

        private string TypeNameFor() {
            return IsCollection ? "Collection" : "Object";
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("class", FullName);
            str.Append("type", TypeNameFor());
            str.Append("persistable", Persistable);
            str.Append("superclass", InnerSpec.Superclass == null ? "object" : InnerSpec.Superclass.FullName);
            return str.ToString();
        }

        protected bool Equals(TypeSpec other) {
            return Equals(InnerSpec, other.InnerSpec);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((TypeSpec) obj);
        }

        public override int GetHashCode() {
            return InnerSpec != null ? InnerSpec.GetHashCode() : 0;
        }
    }
}

// Copyright (c) Naked Objects Group Ltd.