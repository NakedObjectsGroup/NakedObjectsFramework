// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public abstract class TypeSpecImmutable : Specification, ITypeSpecBuilder {
        private readonly IIdentifier identifier;
        private ImmutableList<ITypeSpecImmutable> subclasses;

        protected TypeSpecImmutable(Type type) {
            Type = type.IsGenericType && CollectionUtils.IsCollection(type) ? type.GetGenericTypeDefinition() : type;
            identifier = new IdentifierImpl(type.FullName);
            Interfaces = ImmutableList<ITypeSpecImmutable>.Empty;
            subclasses = ImmutableList<ITypeSpecImmutable>.Empty;
            ContributedActions = ImmutableList<IActionSpecImmutable>.Empty;
            CollectionContributedActions = ImmutableList<IActionSpecImmutable>.Empty;
            FinderActions = ImmutableList<IActionSpecImmutable>.Empty;
        }

        #region ITypeSpecBuilder Members

        public void Introspect(IFacetDecoratorSet decorator, IIntrospector introspector) {
            introspector.IntrospectType(Type, this);
            FullName = introspector.FullName;
            ShortName = introspector.ShortName;
            Superclass = introspector.Superclass;
            Interfaces = introspector.Interfaces.Cast<ITypeSpecImmutable>().ToImmutableList();
            Fields = introspector.Fields;
            ObjectActions = introspector.ObjectActions;
            DecorateAllFacets(decorator);
        }

        public ImmutableDictionary<string, ITypeSpecBuilder> Introspect(IFacetDecoratorSet decorator, IIntrospector introspector, ImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
            metamodel = introspector.IntrospectType(Type, this, metamodel);
            FullName = introspector.FullName;
            ShortName = introspector.ShortName;
            Superclass = introspector.Superclass;
            Interfaces = introspector.Interfaces.Cast<ITypeSpecImmutable>().ToImmutableList();
            Fields = introspector.Fields;
            ObjectActions = introspector.ObjectActions;
            DecorateAllFacets(decorator);
            return metamodel;
        }

        public void AddSubclass(ITypeSpecImmutable subclass) {
            subclasses = subclasses.Add(subclass);
        }

        public ITypeSpecImmutable Superclass { get; private set; }

        public override IIdentifier Identifier => identifier;

        public Type Type { get; private set; }
        public string FullName { get; private set; }
        public string ShortName { get; private set; }

        public IMenuImmutable ObjectMenu => GetFacet<IMenuFacet>().GetMenu();

        public IList<IActionSpecImmutable> ObjectActions { get; private set; }
        public IList<IActionSpecImmutable> ContributedActions { get; private set; }
        public IList<IActionSpecImmutable> CollectionContributedActions { get; private set; }
        public IList<IActionSpecImmutable> FinderActions { get; private set; }
        public IList<IAssociationSpecImmutable> Fields { get; private set; }
        public IList<ITypeSpecImmutable> Interfaces { get; private set; }

        public IList<ITypeSpecImmutable> Subclasses => subclasses;

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

            foreach (var interfaceSpec in Interfaces) {
                IFacet interfaceFacet = interfaceSpec.GetFacet(facetType);
                if (FacetUtils.IsNotANoopFacet(interfaceFacet)) {
                    return interfaceFacet;
                }
                if (noopFacet == null) {
                    noopFacet = interfaceFacet;
                }
            }

            return noopFacet;
        }

        public virtual bool IsCollection => ContainsFacet(typeof (ICollectionFacet));

        public bool IsQueryable {
            get {
                var facet = GetFacet<ICollectionFacet>();
                return facet != null && facet.IsQueryable;
            }
        }

        public virtual bool IsParseable => ContainsFacet(typeof (IParseableFacet));

        public virtual bool IsObject => !IsCollection;

        private static bool IsAssignableToGenericType(Type givenType, Type genericType) {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType)) {
                return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType) {
                return true;
            }

            Type baseType = givenType.BaseType;
            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }

        public bool IsOfType(ITypeSpecImmutable otherSpecification) {
            if (otherSpecification == this) {
                return true;
            }

            Type otherType = otherSpecification.Type;

            if (otherType.IsAssignableFrom(Type)) {
                return true;
            }

            // match  generic types 
            if (Type.IsGenericType && IsCollection  && otherType.IsGenericType && otherSpecification.IsCollection) {
                var thisGenericType = Type.GetGenericTypeDefinition();
                var otherGenericType = Type.GetGenericTypeDefinition();
                return thisGenericType == otherGenericType || IsAssignableToGenericType(otherType, thisGenericType);
            }

            return false;
        }

        public string GetIconName(INakedObjectAdapter forObjectAdapter, IMetamodel metamodel) {
            var iconFacet = GetFacet<IIconFacet>();
            string iconName = null;
            if (iconFacet != null) {
                iconName = forObjectAdapter == null ? iconFacet.GetIconName() : iconFacet.GetIconName(forObjectAdapter);
            }
            else if (IsCollection && !IsParseable) {
                iconName = GetFacet<ITypeOfFacet>().GetValueSpec(forObjectAdapter, metamodel).GetIconName(null, metamodel);
            }

            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        #endregion

        public void AddContributedActions(IList<IActionSpecImmutable> contributedActions) {
            ContributedActions = contributedActions.ToImmutableList();
        }

        public void AddCollectionContributedActions(IList<IActionSpecImmutable> collectionContributedActions) {
            CollectionContributedActions = collectionContributedActions.ToImmutableList();
        }

        public void AddFinderActions(IList<IActionSpecImmutable> finderActions) {
            FinderActions = finderActions.ToImmutableList();
        }

        private void DecorateAllFacets(IFacetDecoratorSet decorator) {
            decorator.DecorateAllHoldersFacets(this);
            Fields.ForEach(decorator.DecorateAllHoldersFacets);
            ObjectActions.Where(s => s != null).ForEach(action => DecorateAction(decorator, action));
        }

        private static void DecorateAction(IFacetDecoratorSet decorator, IActionSpecImmutable action) {
            decorator.DecorateAllHoldersFacets(action);
            action.Parameters.ForEach(decorator.DecorateAllHoldersFacets);
        }

        public override string ToString() {
            return $"{GetType().Name} for {Type.Name}";
        }

        #region ISerializable

        private readonly IList<IActionSpecImmutable> tempContributedActions;
        private readonly IList<IActionSpecImmutable> tempCollectionContributedActions;
        private readonly IList<IActionSpecImmutable> tempFinderActions;
        private readonly IList<IAssociationSpecImmutable> tempFields;
        private readonly IList<ITypeSpecImmutable> tempInterfaces;
        private readonly IList<IActionSpecImmutable> tempObjectActions;
        private readonly IList<ITypeSpecImmutable> tempSubclasses;

        // The special constructor is used to deserialize values. 
        protected TypeSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            Type = info.GetValue<Type>("Type");
            FullName = info.GetValue<string>("FullName");
            ShortName = info.GetValue<string>("ShortName");
            identifier = info.GetValue<IIdentifier>("identifier");
            Superclass = info.GetValue<ITypeSpecImmutable>("Superclass");
            tempFields = info.GetValue<IList<IAssociationSpecImmutable>>("Fields");
            tempInterfaces = info.GetValue<IList<ITypeSpecImmutable>>("Interfaces");
            tempSubclasses = info.GetValue<IList<ITypeSpecImmutable>>("subclasses");
            tempObjectActions = info.GetValue<IList<IActionSpecImmutable>>("ObjectActions");
            tempContributedActions = info.GetValue<IList<IActionSpecImmutable>>("ContributedActions");
            tempCollectionContributedActions = info.GetValue<IList<IActionSpecImmutable>>("CollectionContributedActions");
            tempFinderActions = info.GetValue<IList<IActionSpecImmutable>>("FinderActions");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<Type>("Type", Type);
            info.AddValue<string>("FullName", FullName);
            info.AddValue<string>("ShortName", ShortName);
            info.AddValue<IIdentifier>("identifier", identifier);
            info.AddValue<IList<IAssociationSpecImmutable>>("Fields", Fields.ToList());
            info.AddValue<IList<ITypeSpecImmutable>>("Interfaces", Interfaces.ToList());
            info.AddValue<ITypeSpecImmutable>("Superclass", Superclass);
            info.AddValue<IList<ITypeSpecImmutable>>("subclasses", subclasses.ToList());
            info.AddValue<IList<IActionSpecImmutable>>("ObjectActions", ObjectActions.ToList());
            info.AddValue<IList<IActionSpecImmutable>>("ContributedActions", ContributedActions.ToList());
            info.AddValue<IList<IActionSpecImmutable>>("CollectionContributedActions", CollectionContributedActions.ToList());
            info.AddValue<IList<IActionSpecImmutable>>("FinderActions", FinderActions.ToList());
            base.GetObjectData(info, context);
        }

        public override void OnDeserialization(object sender) {
            Fields = tempFields.ToImmutableList();
            Interfaces = tempInterfaces.ToImmutableList();
            subclasses = tempSubclasses.ToImmutableList();
            ObjectActions = tempObjectActions.ToImmutableList();
            ContributedActions = tempContributedActions.ToImmutableList();
            CollectionContributedActions = tempCollectionContributedActions.ToImmutableList();
            FinderActions = tempFinderActions.ToImmutableList();
            base.OnDeserialization(sender);
        }

        #endregion
    }
}