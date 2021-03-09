// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Framework;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Spec;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Error;
using NakedFramework.Core.Util;

namespace NakedFramework.Core.Spec {
    public abstract class TypeSpec : ITypeSpec {
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
        private bool? isStatic;
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

        protected TypeSpec(SpecFactory memberFactory, ITypeSpecImmutable innerSpec, INakedObjectsFramework framework) {
            MemberFactory = memberFactory ?? throw new InitialisationException($"{nameof(memberFactory)} is null");
            InnerSpec = innerSpec ?? throw new InitialisationException($"{nameof(innerSpec)} is null");
            Framework = framework;
        }

        private Type Type => InnerSpec.Type;

        protected IActionSpec[] ObjectActions => objectActions ??= MemberFactory.CreateActionSpecs(InnerSpec.ObjectActions);

        protected SpecFactory MemberFactory { get; }

        private string DefaultTitle() => InnerSpec is IServiceSpecImmutable ? SingularName : UntitledName;

        protected abstract PersistableType GetPersistable();

        private string TypeNameFor() => IsCollection ? "Collection" : "Object";

        public override string ToString() {
            var str = new AsString(this);
            str.Append("class", FullName);
            str.Append("type", TypeNameFor());
            str.Append("persistable", Persistable);
            str.Append("superclass", InnerSpec.Superclass == null ? "object" : InnerSpec.Superclass.FullName);
            return str.ToString();
        }

        protected bool Equals(TypeSpec other) => Equals(InnerSpec, other.InnerSpec);

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((TypeSpec) obj);
        }

        public override int GetHashCode() => InnerSpec != null ? InnerSpec.GetHashCode() : 0;

        #region ITypeSpec Members

        public ITypeSpecImmutable InnerSpec { get; }
        public INakedObjectsFramework Framework { get; }

        public virtual string FullName => InnerSpec.FullName;

        public virtual object DefaultValue => null;

        public Type[] FacetTypes => InnerSpec.FacetTypes;

        public IIdentifier Identifier => InnerSpec.Identifier;

        public bool ContainsFacet(Type facetType) => InnerSpec.ContainsFacet(facetType);

        public bool ContainsFacet<T>() where T : IFacet => InnerSpec.ContainsFacet<T>();

        public IFacet GetFacet(Type type) => InnerSpec.GetFacet(type);

        public T GetFacet<T>() where T : IFacet => InnerSpec.GetFacet<T>();

        public IEnumerable<IFacet> GetFacets() => InnerSpec.GetFacets();

        public virtual bool IsParseable {
            get {
                isParseable ??= InnerSpec.ContainsFacet(typeof(IParseableFacet));

                return isParseable.Value;
            }
        }

        public virtual bool IsEncodeable {
            get {
                isEncodeable ??= InnerSpec.ContainsFacet(typeof(IEncodeableFacet));

                return isEncodeable.Value;
            }
        }

        public virtual bool IsAggregated {
            get {
                isAggregated ??= InnerSpec.ContainsFacet(typeof(IAggregatedFacet));

                return isAggregated.Value;
            }
        }

        public virtual bool IsCollection {
            get {
                isCollection ??= InnerSpec.ContainsFacet(typeof(ICollectionFacet));

                return isCollection.Value;
            }
        }

        public virtual bool IsViewModel {
            get {
                isViewModel ??= InnerSpec.ContainsFacet(typeof(IViewModelFacet));

                return isViewModel.Value;
            }
        }

        public virtual bool IsObject => !IsCollection;

        public virtual ITypeSpec Superclass {
            get {
                if (superclass == null && InnerSpec.Superclass != null) {
                    superclass = Framework.MetamodelManager.GetSpecification(InnerSpec.Superclass);
                }

                return superclass;
            }
        }

        public abstract IActionSpec[] GetActions();

        public IMenuImmutable Menu => InnerSpec.ObjectMenu;

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
                hasSubclasses ??= InnerSpec.Subclasses.Any();

                return hasSubclasses.Value;
            }
        }

        public ITypeSpec[] Interfaces {
            get { return interfaces ??= InnerSpec.Interfaces.Select(i => Framework.MetamodelManager.GetSpecification(i)).ToArray(); }
        }

        public ITypeSpec[] Subclasses {
            get { return subclasses ??= InnerSpec.Subclasses.Select(i => Framework.MetamodelManager.GetSpecification(i)).ToArray(); }
        }

        public bool IsAbstract {
            get {
                isAbstract ??= InnerSpec.GetFacet<ITypeIsAbstractFacet>().Flag;

                return isAbstract.Value;
            }
        }

        public bool IsInterface {
            get {
                isInterface ??= InnerSpec.GetFacet<ITypeIsInterfaceFacet>().Flag;

                return isInterface.Value;
            }
        }

        public string ShortName {
            get {
                if (shortName == null) {
                    var postfix = "";
                    if (Type.IsGenericType && !IsCollection) {
                        postfix = Type.GetGenericArguments().Aggregate(string.Empty, (x, y) => x + "-" + Framework.MetamodelManager.GetSpecification(y).ShortName);
                    }

                    shortName = InnerSpec.ShortName + postfix;
                }

                return shortName;
            }
        }

        public string SingularName => singularName ??= InnerSpec.GetFacet<INamedFacet>().NaturalName;

        public string UntitledName => untitledName ??= NakedObjects.Resources.NakedObjects.Untitled + SingularName;

        public string PluralName => pluralName ??= InnerSpec.GetFacet<IPluralFacet>().Value;

        public string Description => description ??= InnerSpec.GetFacet<IDescribedAsFacet>().Value ?? "";

        public bool HasNoIdentity {
            get {
                hasNoIdentity ??= InnerSpec.GetFacet<ICollectionFacet>() != null || InnerSpec.GetFacet<IParseableFacet>() != null;

                return hasNoIdentity.Value;
            }
        }

        public bool IsQueryable {
            get {
                isQueryable ??= InnerSpec.IsQueryable;

                return isQueryable.Value;
            }
        }

        public bool IsVoid {
            get {
                isVoid ??= InnerSpec.GetFacet<ITypeIsVoidFacet>().Flag;

                return isVoid.Value;
            }
        }

        public bool IsStatic {
            get {
                isStatic ??= InnerSpec.GetFacet<ITypeIsStaticFacet>().Flag;

                return isStatic.Value;
            }
        }

        public PersistableType Persistable {
            get {
                persistable ??= GetPersistable();

                return persistable.Value;
            }
        }

        /// <summary>
        ///     Determines if this class represents the same class, or a subclass, of the specified class.
        /// </summary>
        public bool IsOfType(ITypeSpec spec) => InnerSpec.IsOfType(spec.InnerSpec);

        public string GetTitle(INakedObjectAdapter nakedObjectAdapter) {
            var titleFacet = GetFacet<ITitleFacet>();
            var title = titleFacet == null ? null : titleFacet.GetTitle(nakedObjectAdapter, Framework);
            return title ?? DefaultTitle();
        }

        public string GetInvariantString(INakedObjectAdapter nakedObjectAdapter) {
            var parser = GetFacet<IParseableFacet>();
            return parser == null ? null : parser.InvariantString(nakedObjectAdapter);
        }

        public string GetIconName(INakedObjectAdapter forObjectAdapter) => InnerSpec.GetIconName(forObjectAdapter, Framework.MetamodelManager.Metamodel);

        #endregion
    }
}

// Copyright (c) Naked Objects Group Ltd.