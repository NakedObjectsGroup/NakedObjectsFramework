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
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Facet;
using NakedFramework.Architecture.Menu;
using NakedFramework.Architecture.Reflect;
using NakedFramework.Architecture.SpecImmutable;
using NakedFramework.Core.Util;
using NakedFramework.Metamodel.Serialization;
using NakedFramework.Metamodel.Spec;
using NakedFramework.Metamodel.Utils;

namespace NakedFramework.Metamodel.SpecImmutable;

[Serializable]
public abstract class TypeSpecImmutable : Specification, ITypeSpecBuilder {
    private IIdentifier identifier;
    private ImmutableListSerializationWrapper<ITypeSpecImmutable> interfaces;
    private ImmutableListSerializationWrapper<IActionSpecImmutable> orderedCollectionContributedActions;
    private ImmutableListSerializationWrapper<IActionSpecImmutable> orderedContributedActions;
    private ImmutableListSerializationWrapper<IAssociationSpecImmutable> orderedFields;
    private ImmutableListSerializationWrapper<IActionSpecImmutable> orderedFinderActions;

    private ImmutableListSerializationWrapper<IActionSpecImmutable> orderedObjectActions;

    [NonSerialized]
    private Type[] services;

    private ImmutableListSerializationWrapper<ITypeSpecImmutable> subclasses;
    private TypeSerializationWrapper typeWrapper;

    [NonSerialized]
    private List<IActionSpecImmutable> unorderedCollectionContributedActions = new();

    [NonSerialized]
    protected List<IActionSpecImmutable> unorderedContributedActions = new();

    [NonSerialized]
    private List<IAssociationSpecImmutable> unorderedFields;

    [NonSerialized]
    private List<IActionSpecImmutable> unorderedFinderActions = new();

    [NonSerialized]
    private List<IActionSpecImmutable> unorderedObjectActions;

    [NonSerialized]
    private List<ITypeSpecImmutable> unorderedSubclasses = new();

    protected TypeSpecImmutable(Type type, bool isRecognized) : base(null) {
        typeWrapper = new TypeSerializationWrapper(type.IsGenericType && CollectionUtils.IsCollection(type) ? type.GetGenericTypeDefinition() : type);
        ReflectionStatus = isRecognized ? ReflectionStatus.PlaceHolder : ReflectionStatus.PendingIntrospection;
    }

    private ReflectionStatus ReflectionStatus { get; set; }

    public Type Type => typeWrapper.Type;

    public string FullName { get; private set; }
    public string ShortName { get; private set; }
    public IMenuImmutable ObjectMenu => GetFacet<IMenuFacet>()?.GetMenu();

    public IReadOnlyList<IActionSpecImmutable> OrderedObjectActions => orderedObjectActions.ImmutableList;

    public IReadOnlyList<IActionSpecImmutable> OrderedContributedActions => orderedContributedActions.ImmutableList;

    public IReadOnlyList<IActionSpecImmutable> OrderedCollectionContributedActions => orderedCollectionContributedActions.ImmutableList;

    public IReadOnlyList<IActionSpecImmutable> OrderedFinderActions => orderedFinderActions.ImmutableList;

    public IReadOnlyList<IAssociationSpecImmutable> OrderedFields => orderedFields.ImmutableList;

    public IReadOnlyList<ITypeSpecImmutable> Interfaces => interfaces?.ImmutableList ?? ImmutableList<ITypeSpecImmutable>.Empty;

    public IReadOnlyList<ITypeSpecImmutable> Subclasses => subclasses.ImmutableList;

    public IList<IAssociationSpecImmutable> UnorderedFields => unorderedFields;

    public IList<IActionSpecImmutable> UnorderedObjectActions => unorderedObjectActions;

    public ITypeSpecImmutable Superclass { get; private set; }

    public override IIdentifier Identifier => identifier;

    public void AddContributedFunctions(IList<IActionSpecImmutable> contributedFunctions) => unorderedContributedActions.AddRange(contributedFunctions);

    public void AddContributedFields(IList<IAssociationSpecImmutable> addedFields) => unorderedFields.AddRange(addedFields);

    public bool IsPlaceHolder => ReflectionStatus == ReflectionStatus.PlaceHolder;

    public bool IsPendingIntrospection => ReflectionStatus == ReflectionStatus.PendingIntrospection;

    public virtual bool IsCollection => ContainsFacet(typeof(ICollectionFacet));

    public bool IsQueryable => GetFacet<ICollectionFacet>()?.IsQueryable == true;

    public virtual bool IsParseable => ContainsFacet(typeof(IParseableFacet));

    public virtual bool IsObject => !IsCollection;

    public void RemoveAction(IActionSpecImmutable action, ILogger logger) {
        if (!UnorderedObjectActions.Remove(action)) {
            logger.LogWarning($"Failed to find and remove {action} from {identifier}");
        }
    }

    public void CompleteIntegration() {
        orderedFields = new ImmutableListSerializationWrapper<IAssociationSpecImmutable>(CreateOrderedImmutableList(UnorderedFields));
        orderedFields.ImmutableList.Select(a => new FacetUtils.ActionHolder(a)).ToList().ErrorOnDuplicates();
        orderedObjectActions = new ImmutableListSerializationWrapper<IActionSpecImmutable>(CreateOrderedImmutableList(UnorderedObjectActions));
        orderedContributedActions = new ImmutableListSerializationWrapper<IActionSpecImmutable>(CreateOrderedContributedActions());
        orderedCollectionContributedActions = new ImmutableListSerializationWrapper<IActionSpecImmutable>(CreateOrderedImmutableList(unorderedCollectionContributedActions));
        orderedFinderActions = new ImmutableListSerializationWrapper<IActionSpecImmutable>(CreateOrderedImmutableList(unorderedFinderActions));
        subclasses = new ImmutableListSerializationWrapper<ITypeSpecImmutable>(unorderedSubclasses.ToImmutableList());

        ClearUnorderedCollections();
    }

    public IImmutableDictionary<string, ITypeSpecBuilder> Introspect(IFacetDecoratorSet decorator, IIntrospector introspector, IImmutableDictionary<string, ITypeSpecBuilder> metamodel) {
        metamodel = introspector.IntrospectType(Type, this, metamodel);
        identifier = introspector.Identifier;
        FullName = introspector.FullName;
        ShortName = introspector.ShortName;
        Superclass = introspector.Superclass;
        interfaces = new ImmutableListSerializationWrapper<ITypeSpecImmutable>(introspector.Interfaces.Cast<ITypeSpecImmutable>().ToImmutableList());
        unorderedFields = introspector.UnorderedFields.ToList();
        unorderedObjectActions = introspector.UnorderedObjectActions.ToList();
        DecorateAllFacets(decorator);
        typeWrapper = new TypeSerializationWrapper(introspector.SpecificationType);
        ReflectionStatus = ReflectionStatus.Introspected;
        return metamodel;
    }

    public void AddSubclass(ITypeSpecImmutable subclass) {
        lock (unorderedSubclasses) {
            unorderedSubclasses.Add(subclass);
        }
    }

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

    public string[] GetLocallyContributedActionNames(string id) {
        return OrderedFields.OfType<IOneToManyAssociationSpecImmutable>().SingleOrDefault(a => a.Identifier.MemberName == id)?.ContributedActionNames ?? Array.Empty<string>();
    }

    private ImmutableList<IActionSpecImmutable> CreateOrderedContributedActions() {
        return Order(unorderedContributedActions).GroupBy(i => i.OwnerSpec.Type, i => i, (service, actions) => new { service, actions }).OrderBy(a => Array.IndexOf(services, a.service)).SelectMany(a => a.actions).ToImmutableList();
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

    private static ImmutableList<T> CreateOrderedImmutableList<T>(IEnumerable<T> members) where T : IMemberSpecImmutable => Order(members).ToImmutableList();

    private static IEnumerable<T> Order<T>(IEnumerable<T> members) where T : IMemberSpecImmutable => members.OrderBy(m => m, new MemberOrderComparator<T>());

    private void ClearUnorderedCollections() {
        unorderedFields = null;
        unorderedObjectActions = null;
        unorderedContributedActions = null;
        unorderedCollectionContributedActions = null;
        unorderedFinderActions = null;
        services = null;
        unorderedSubclasses = null;
    }
}