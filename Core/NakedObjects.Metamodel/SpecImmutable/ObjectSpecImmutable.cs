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
using NakedObjects.Architecture;
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
    public class ObjectSpecImmutable : Specification, IObjectSpecBuilder {
        private readonly IIdentifier identifier;
        private ImmutableList<IObjectSpecImmutable> subclasses;

        public ObjectSpecImmutable(Type type, IMetamodel metamodel) {
            Type = type.IsGenericType && CollectionUtils.IsCollection(type) ? type.GetGenericTypeDefinition() : type;
            identifier = new IdentifierImpl(metamodel, type.FullName);
            Interfaces = ImmutableList<IObjectSpecImmutable>.Empty;
            subclasses = ImmutableList<IObjectSpecImmutable>.Empty;
            ContributedActions = ImmutableList<IActionSpecImmutable>.Empty;
            CollectionContributedActions = ImmutableList<IActionSpecImmutable>.Empty;
            FinderActions = ImmutableList<IActionSpecImmutable>.Empty;
        }

        private string SingularName {
            get { return GetFacet<INamedFacet>().Value; }
        }

        private string UntitledName {
            get { return Resources.NakedObjects.Untitled + SingularName; }
        }

        #region IObjectSpecBuilder Members

        public void Introspect(IFacetDecoratorSet decorator, IIntrospector introspector) {
            introspector.IntrospectType(Type, this);
            FullName = introspector.FullName;
            ShortName = introspector.ShortName;
            Superclass = introspector.Superclass;
            Interfaces = introspector.Interfaces.Cast<IObjectSpecImmutable>().ToImmutableList();
            Fields = introspector.Fields;
            ObjectActions = introspector.ObjectActions;
            DecorateAllFacets(decorator);
        }

        public void MarkAsService() {
            if (Fields.Any(field => field.Identifier.MemberName != "Id")) {
                string fieldNames = Fields.Where(field => field.Identifier.MemberName != "Id").Aggregate("", (current, field) => current + (current.Length > 0 ? ", " : "") /*+ field.GetName(persistor)*/);
                throw new ModelException(string.Format(Resources.NakedObjects.ServiceObjectWithFieldsError, FullName, fieldNames));
            }
            Service = true;
        }

        public void AddSubclass(IObjectSpecImmutable subclass) {
            subclasses = subclasses.Add(subclass);
        }

        public void AddContributedActions(IList<IActionSpecImmutable> contributedActions) {
            ContributedActions = contributedActions.ToImmutableList();
        }

        public void AddCollectionContributedActions(IList<IActionSpecImmutable> collectionContributedActions) {
            CollectionContributedActions = collectionContributedActions.ToImmutableList();
        }

        public void AddFinderActions(IList<IActionSpecImmutable> finderActions) {
            FinderActions = finderActions.ToImmutableList();
        }

        #endregion

        #region IObjectSpecImmutable Members

        public IObjectSpecImmutable Superclass { get; private set; }

        public override IIdentifier Identifier {
            get { return identifier; }
        }

        public Type Type { get; private set; }

        public string FullName { get; private set; }

        public string ShortName { get; private set; }

        public IMenuImmutable ObjectMenu {
            get { return GetFacet<IMenuFacet>().GetMenu(); }
        }

        public IList<IActionSpecImmutable> ObjectActions { get; private set; }

        public IList<IActionSpecImmutable> ContributedActions { get; private set; }

        public IList<IActionSpecImmutable> CollectionContributedActions { get; private set; }

        public IList<IActionSpecImmutable> FinderActions { get; private set; }

        public IList<IAssociationSpecImmutable> Fields { get; private set; }

        public IList<IObjectSpecImmutable> Interfaces { get; private set; }

        public IList<IObjectSpecImmutable> Subclasses {
            get { return subclasses; }
        }

        public bool Service { get; private set; }

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

            foreach (IObjectSpecImmutable interfaceSpec in Interfaces) {
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

        public virtual bool IsCollection {
            get { return ContainsFacet(typeof (ICollectionFacet)); }
        }

        public virtual bool IsParseable {
            get { return ContainsFacet(typeof (IParseableFacet)); }
        }

        public virtual bool IsObject {
            get { return !IsCollection; }
        }

        public bool IsOfType(IObjectSpecImmutable specification) {
            if (specification == this) {
                return true;
            }
            if (Interfaces.Any(interfaceSpec => interfaceSpec.IsOfType(specification))) {
                return true;
            }

            // match covariant generic types 
            if (Type.IsGenericType && IsCollection) {
                Type otherType = specification.Type;
                if (otherType.IsGenericType && Type.GetGenericArguments().Count() == 1 && otherType.GetGenericArguments().Count() == 1) {
                    if (Type.GetGenericTypeDefinition() == (typeof (IQueryable<>)) && Type.GetGenericTypeDefinition() == otherType.GetGenericTypeDefinition()) {
                        Type genericArgument = Type.GetGenericArguments().Single();
                        Type otherGenericArgument = otherType.GetGenericArguments().Single();
                        Type otherGenericParameter = otherType.GetGenericTypeDefinition().GetGenericArguments().Single();
                        if ((otherGenericParameter.GenericParameterAttributes & GenericParameterAttributes.Covariant) != 0) {
                            if (otherGenericArgument.IsAssignableFrom(genericArgument)) {
                                return true;
                            }
                        }
                    }
                }
            }

            return Superclass != null && Superclass.IsOfType(specification);
        }

        public string GetIconName(INakedObject forObject, IMetamodel metamodel) {
            var iconFacet = GetFacet<IIconFacet>();
            string iconName = null;
            if (iconFacet != null) {
                iconName = forObject == null ? iconFacet.GetIconName() : iconFacet.GetIconName(forObject);
            }
            else if (IsCollection && !IsParseable) {
                iconName = GetFacet<ITypeOfFacet>().GetValueSpec(forObject, metamodel).GetIconName(null, metamodel);
            }

            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        #endregion

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
            return string.Format("{0} for {1}", GetType().Name, Type.Name);
        }

        #region ISerializable

        private readonly IList<IActionSpecImmutable> tempContributedActions;
        private readonly IList<IActionSpecImmutable> tempCollectionContributedActions;
        private readonly IList<IActionSpecImmutable> tempFinderActions;
        private readonly IList<IAssociationSpecImmutable> tempFields;
        private readonly IList<IObjectSpecImmutable> tempInterfaces;
        private readonly IList<IActionSpecImmutable> tempObjectActions;
        private readonly IList<IObjectSpecImmutable> tempSubclasses;

        // The special constructor is used to deserialize values. 
        public ObjectSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {
            Type = info.GetValue<Type>("Type");
            FullName = info.GetValue<string>("FullName");
            ShortName = info.GetValue<string>("ShortName");
            identifier = info.GetValue<IIdentifier>("identifier");
            Superclass = info.GetValue<IObjectSpecImmutable>("Superclass");
            Service = info.GetValue<bool>("Service");
            tempFields = info.GetValue<IList<IAssociationSpecImmutable>>("Fields");
            tempInterfaces = info.GetValue<IList<IObjectSpecImmutable>>("Interfaces");
            tempSubclasses = info.GetValue<IList<IObjectSpecImmutable>>("subclasses");
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
            info.AddValue<bool>("Service", Service);
            info.AddValue<IList<IAssociationSpecImmutable>>("Fields", Fields.ToList());
            info.AddValue<IList<IObjectSpecImmutable>>("Interfaces", Interfaces.ToList());
            info.AddValue<IObjectSpecImmutable>("Superclass", Superclass);
            info.AddValue<IList<IObjectSpecImmutable>>("subclasses", subclasses.ToList());
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