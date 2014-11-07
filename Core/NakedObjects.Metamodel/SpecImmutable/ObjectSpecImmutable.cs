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
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Meta.Adapter;
using NakedObjects.Meta.Spec;
using NakedObjects.Meta.Utils;
using NakedObjects.Architecture.Menu;
using NakedObjects.Meta.Menus;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public class ObjectSpecImmutable : Specification, IObjectSpecImmutable, IObjectSpecBuilder {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ObjectSpecImmutable));
        private readonly IMetamodel metamodel;
        private readonly IIdentifier identifier;
        private ImmutableList<IObjectSpecImmutable> subclasses;

        public ObjectSpecImmutable(Type type, IMetamodel metamodel) {
            this.metamodel = metamodel;
            Type = type.IsGenericType && CollectionUtils.IsCollection(type) ? type.GetGenericTypeDefinition() : type;
            identifier = new IdentifierImpl(metamodel, type.FullName);
            Interfaces = ImmutableList<IObjectSpecImmutable>.Empty;
            subclasses = ImmutableList<IObjectSpecImmutable>.Empty;
            ContributedActions =  ImmutableList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>>.Empty;
            RelatedActions = ImmutableList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>>.Empty;
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
            if (Fields.Any(field => field.Spec.Identifier.MemberName != "Id")) {
                string fieldNames = Fields.Where(field => field.Spec.Identifier.MemberName != "Id").Aggregate("", (current, field) => current + (current.Length > 0 ? ", " : "") /*+ field.GetName(persistor)*/);
                throw new ModelException(string.Format(Resources.NakedObjects.ServiceObjectWithFieldsError, FullName, fieldNames));
            }
            Service = true;
        }

        public void AddSubclass(IObjectSpecImmutable subclass) {
            subclasses = subclasses.Add(subclass);
        }

        public void AddContributedActions(IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>> contributedActions) {
            ContributedActions = contributedActions.ToImmutableList();
            CreateObjectMenu();
        }

        private void CreateObjectMenu() {
            Menu menu = new Menu(metamodel, "Object Actions");
            //First add the native actions
            menu.AddOrderableElementsToMenu(ObjectActions, menu);
            //Then add the contributed actions
            foreach (var ca in ContributedActions) {
                Menu sub = new Menu(metamodel, ca.Item1); //Item 1 should be friendly name of the contributing service
                //Item2 is contributing service class name, not used.
                sub.AddOrderableElementsToMenu(ca.Item3, sub); //Item 3 should be the actions
                menu.AddAsSubMenu(sub);
            }
            ObjectMenu = menu;
        }

        public void AddRelatedActions(IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>> relatedActions) {
            RelatedActions = relatedActions.ToImmutableList();
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

        public IMenu ObjectMenu { get; private set; }

        public IList< IOrderableElement<IActionSpecImmutable>> ObjectActions { get; private set; }

        public IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>> ContributedActions { get; private set; }

        public IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>> RelatedActions { get; private set; }

        public IList<IOrderableElement<IAssociationSpecImmutable>> Fields { get; private set; }

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

        public string GetTitle(INakedObject nakedObject) {
            var titleFacet = GetFacet<ITitleFacet>();
            string title = titleFacet == null ? null : titleFacet.GetTitle(nakedObject);
            return title ?? DefaultTitle();
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
            else if (IsCollection) {
                iconName = GetFacet<ITypeOfFacet>().GetValueSpec(forObject, metamodel).GetIconName(null, metamodel);
            }

            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        #endregion

        private void DecorateAllFacets(IFacetDecoratorSet decorator) {
            decorator.DecorateAllHoldersFacets(this);
            foreach (var field in Fields) {
                decorator.DecorateAllHoldersFacets(field.Spec);
            }
            foreach (var action in ObjectActions.Select(oa => oa.Spec).Where(s => s != null)) {
                DecorateAction(decorator, action);
            }
        }

        private static void DecorateAction(IFacetDecoratorSet decorator, IActionSpecImmutable action) {
            decorator.DecorateAllHoldersFacets(action);
            foreach (var parm in action.Parameters) {
                decorator.DecorateAllHoldersFacets(parm);
            }
        }

        private string DefaultTitle() {
            return Service ? SingularName : UntitledName;
        }

        #region ISerializable

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("Type", Type);
            info.AddValue("FullName", FullName);
            info.AddValue("ShortName", ShortName);
            info.AddValue("identifier", identifier);
            info.AddValue("Fields", Fields.ToList());
            info.AddValue("Interfaces", Interfaces.ToList());
            info.AddValue("subclasses", subclasses.ToList());
            info.AddValue("ObjectActions", ObjectActions.ToList());
            info.AddValue("ContributedActions", ContributedActions.ToList());
            info.AddValue("RelatedActions", RelatedActions.ToList());
            base.GetObjectData(info, context);
        }

         // The special constructor is used to deserialize values. 
        public ObjectSpecImmutable(SerializationInfo info, StreamingContext context) :base(info, context)  {
            Type = (Type) info.GetValue("Type", typeof (Type));
            FullName = (string)info.GetValue("FullName", typeof(string));
            ShortName = (string)info.GetValue("ShortName", typeof(string));
            identifier = (IIdentifier)info.GetValue("identifier", typeof(IIdentifier));
            Fields = ((IList<IOrderableElement<IAssociationSpecImmutable>>)info.GetValue("Fields", typeof(IList<IOrderableElement<IAssociationSpecImmutable>>))).ToImmutableList();
            Interfaces = ((IList<IObjectSpecImmutable>)info.GetValue("Interfaces", typeof(IList<IObjectSpecImmutable>))).ToImmutableList();
            subclasses = ((IList<IObjectSpecImmutable>)info.GetValue("subclasses", typeof(IList<IObjectSpecImmutable>))).ToImmutableList();
            ObjectActions = ((IList<IOrderableElement<IActionSpecImmutable>>)info.GetValue("ObjectActions", typeof(IList<IOrderableElement<IActionSpecImmutable>>))).ToImmutableList();
            ContributedActions = ((IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>>)info.GetValue("ContributedActions", typeof(IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>>))).ToImmutableList();
            RelatedActions = ((IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>>)info.GetValue("RelatedActions", typeof(IList<Tuple<string, string, IList<IOrderableElement<IActionSpecImmutable>>>>))).ToImmutableList();
        }


        #endregion


        public override string ToString() {
            return string.Format("{0} for {1}", GetType().Name, Type.Name);
        }
    }
}