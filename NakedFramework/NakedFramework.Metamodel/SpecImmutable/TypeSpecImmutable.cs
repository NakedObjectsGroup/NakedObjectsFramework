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
using System.Runtime.Serialization;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Spec;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public abstract class TypeSpecImmutable : Specification, ITypeSpecBuilder {
    private IIdentifier identifier;
    private Type[] services;
    private ImmutableList<ITypeSpecImmutable> subclasses;
    private List<IActionSpecImmutable> unorderedCollectionContributedActions = new();
    private List<IActionSpecImmutable> unorderedContributedActions = new();
    private List<IAssociationSpecImmutable> unorderedFields;
    private List<IActionSpecImmutable> unorderedFinderActions = new();
    private List<IActionSpecImmutable> unorderedObjectActions;

    protected TypeSpecImmutable(Type type, bool isRecognized) {
        Type = type.IsGenericType && CollectionUtils.IsCollection(type) ? type.GetGenericTypeDefinition() : type;
        Interfaces = ImmutableList<ITypeSpecImmutable>.Empty;
        subclasses = ImmutableList<ITypeSpecImmutable>.Empty;
        ReflectionStatus = isRecognized ? ReflectionStatus.PlaceHolder : ReflectionStatus.PendingIntrospection;
    }

    private ReflectionStatus ReflectionStatus { get; set; }

    public void AddContributedFunctions(IList<IActionSpecImmutable> contributedFunctions) => unorderedContributedActions.AddRange(contributedFunctions);

    public void AddContributedFields(IList<IAssociationSpecImmutable> addedFields) => unorderedFields.AddRange(addedFields);

    public bool IsPlaceHolder => ReflectionStatus == ReflectionStatus.PlaceHolder;

    public bool IsPendingIntrospection => ReflectionStatus == ReflectionStatus.PendingIntrospection;

    public void RemoveAction(IActionSpecImmutable action) {
        if (UnorderedObjectActions.Contains(action)) {
            UnorderedObjectActions.Remove(action);
        }
    }

    private static bool IsAssignableToGenericType(Type givenType, Type genericType) {
        var interfaceTypes = givenType.GetInterfaces();

        if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType)) {
            return true;
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType) {
            return true;
        }

        var baseType = givenType.BaseType;
        return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }

    public void AddContributedActions(IList<IActionSpecImmutable> contributedActions, Type[] services) {
        unorderedContributedActions = contributedActions.ToList();
        this.services = services;
    }

    public void AddCollectionContributedActions(IList<IActionSpecImmutable> collectionContributedActions) => unorderedCollectionContributedActions.AddRange(collectionContributedActions);

    public void AddFinderActions(IList<IActionSpecImmutable> finderActions) => unorderedFinderActions.AddRange(finderActions);

    private void DecorateAllFacets(IFacetDecoratorSet decorator) {
        decorator.DecorateAllHoldersFacets(this);
        UnorderedFields.ForEach(decorator.DecorateAllHoldersFacets);
        UnorderedObjectActions.Where(s => s != null).ForEach(action => DecorateAction(decorator, action));
    }

    private static void DecorateAction(IFacetDecoratorSet decorator, IActionSpecImmutable action) {
        decorator.DecorateAllHoldersFacets(action);
        action.Parameters.ForEach(decorator.DecorateAllHoldersFacets);
    }

    public override string ToString() => $"{GetType().Name} for {Type.Name}";

    #region ITypeSpecBuilder Members

    public IImmutableDictionary<string, ITypeSpecBuilder> Introspect(IFacetDecoratorSet decorator, IIntrospector introspector, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        metamodel = introspector.IntrospectType(Type, this, metamodel);
        identifier = introspector.Identifier;
        FullName = introspector.FullName;
        ShortName = introspector.ShortName;
        Superclass = introspector.Superclass;
        Interfaces = introspector.Interfaces.Cast<ITypeSpecImmutable>().ToImmutableList();
        unorderedFields = introspector.UnorderedFields.ToList();
        unorderedObjectActions = introspector.UnorderedObjectActions.ToList();
        DecorateAllFacets(decorator);
        Type = introspector.SpecificationType;
        ReflectionStatus = ReflectionStatus.Introspected;
        return metamodel;
    }

    public void AddSubclass(ITypeSpecImmutable subclass) {
        lock (subclasses) {
            subclasses = subclasses.Add(subclass);
        }
    }

    public ITypeSpecImmutable Superclass { get; private set; }

    public override IIdentifier Identifier => identifier;

    public Type Type { get; private set; }
    public string FullName { get; private set; }
    public string ShortName { get; private set; }
    public IMenuImmutable ObjectMenu => GetFacet<IMenuFacet>()?.GetMenu();

    public IReadOnlyList<IActionSpecImmutable> OrderedObjectActions { get; private set; }
    public IReadOnlyList<IActionSpecImmutable> OrderedContributedActions { get; private set; }
    public IReadOnlyList<IActionSpecImmutable> OrderedCollectionContributedActions { get; private set; }
    public IReadOnlyList<IActionSpecImmutable> OrderedFinderActions { get; private set; }
    public IReadOnlyList<IAssociationSpecImmutable> OrderedFields { get; private set; }

    public IReadOnlyList<ITypeSpecImmutable> Interfaces { get; private set; }

    public IReadOnlyList<ITypeSpecImmutable> Subclasses => subclasses;

    public override IFacet GetFacet(Type facetType) {
        var facet = base.GetFacet(facetType);
        if (FacetUtils.IsNotANoopFacet(facet)) {
            return facet;
        }

        var noopFacet = facet;

        if (Superclass != null) {
            var superClassFacet = Superclass.GetFacet(facetType);
            if (FacetUtils.IsNotANoopFacet(superClassFacet)) {
                return superClassFacet;
            }

            noopFacet ??= superClassFacet;
        }

        foreach (var interfaceSpec in Interfaces) {
            var interfaceFacet = interfaceSpec.GetFacet(facetType);
            if (FacetUtils.IsNotANoopFacet(interfaceFacet)) {
                return interfaceFacet;
            }

            noopFacet ??= interfaceFacet;
        }

        return noopFacet;
    }

    public virtual bool IsCollection => ContainsFacet(typeof(ICollectionFacet));

    public bool IsQueryable => GetFacet<ICollectionFacet>()?.IsQueryable == true;

    public virtual bool IsParseable => ContainsFacet(typeof(IParseableFacet));

    public virtual bool IsObject => !IsCollection;

    public IList<IAssociationSpecImmutable> UnorderedFields => unorderedFields;

    public IList<IActionSpecImmutable> UnorderedObjectActions => unorderedObjectActions;

    public bool IsOfType(ITypeSpecImmutable otherSpecification) {
        if (otherSpecification == this) {
            return true;
        }

        var otherType = otherSpecification.Type;

        if (otherType.IsAssignableFrom(Type)) {
            return true;
        }

        // match  generic types 
        if (Type.IsGenericType && IsCollection && otherType.IsGenericType && otherSpecification.IsCollection) {
            var thisGenericType = Type.GetGenericTypeDefinition();
            var otherGenericType = Type.GetGenericTypeDefinition();
            return thisGenericType == otherGenericType || IsAssignableToGenericType(otherType, thisGenericType);
        }

        return false;
    }

    #endregion

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
        tempContributedActions = info.GetValue<IList<IActionSpecImmutable>>("OrderedContributedActions");
        tempCollectionContributedActions = info.GetValue<IList<IActionSpecImmutable>>("OrderedCollectionContributedActions");
        tempFinderActions = info.GetValue<IList<IActionSpecImmutable>>("OrderedFinderActions");
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
        info.AddValue<Type>("Type", Type);
        info.AddValue<string>("FullName", FullName);
        info.AddValue<string>("ShortName", ShortName);
        info.AddValue<IIdentifier>("identifier", identifier);
        info.AddValue<IList<IAssociationSpecImmutable>>("Fields", OrderedFields.ToList());
        info.AddValue<IList<ITypeSpecImmutable>>("Interfaces", Interfaces.ToList());
        info.AddValue<ITypeSpecImmutable>("Superclass", Superclass);
        info.AddValue<IList<ITypeSpecImmutable>>("subclasses", subclasses.ToList());
        info.AddValue<IList<IActionSpecImmutable>>("ObjectActions", OrderedObjectActions.ToList());
        info.AddValue<IList<IActionSpecImmutable>>("OrderedContributedActions", OrderedContributedActions.ToList());
        info.AddValue<IList<IActionSpecImmutable>>("OrderedCollectionContributedActions", OrderedCollectionContributedActions.ToList());
        info.AddValue<IList<IActionSpecImmutable>>("OrderedFinderActions", OrderedFinderActions.ToList());
        base.GetObjectData(info, context);
    }

    public override void OnDeserialization(object sender) {
        OrderedFields = tempFields.ToImmutableList();
        Interfaces = tempInterfaces.ToImmutableList();
        subclasses = tempSubclasses.ToImmutableList();
        OrderedObjectActions = tempObjectActions.ToImmutableList();
        OrderedContributedActions = tempContributedActions.ToImmutableList();
        OrderedCollectionContributedActions = tempCollectionContributedActions.ToImmutableList();
        OrderedFinderActions = tempFinderActions.ToImmutableList();
        base.OnDeserialization(sender);
    }

    private static IReadOnlyList<T> CreateOrderedImmutableList<T>(IEnumerable<T> members) where T : IMemberSpecImmutable => Order(members).ToImmutableList();

    private static IEnumerable<T> Order<T>(IEnumerable<T> members) where T : IMemberSpecImmutable => members.OrderBy(m => m, new MemberOrderComparator<T>());

    private void ClearUnorderedCollections() {
        unorderedFields = null;
        unorderedObjectActions = null;
        unorderedContributedActions = null;
        unorderedCollectionContributedActions = null;
        unorderedFinderActions = null;
    }

    public void CompleteIntegration() {
        OrderedFields = CreateOrderedImmutableList(UnorderedFields);
        OrderedFields.Select(a => new FacetUtils.ActionHolder(a)).ToList().ErrorOnDuplicates();
        OrderedObjectActions = CreateOrderedImmutableList(UnorderedObjectActions);
        OrderedContributedActions = Order(unorderedContributedActions).GroupBy(i => i.OwnerSpec.Type, i => i, (service, actions) => new { service, actions }).OrderBy(a => Array.IndexOf(services, a.service)).SelectMany(a => a.actions).ToImmutableList();
        OrderedCollectionContributedActions = CreateOrderedImmutableList(unorderedCollectionContributedActions);
        OrderedFinderActions = CreateOrderedImmutableList(unorderedFinderActions);

        ClearUnorderedCollections();
    }

    #endregion
}