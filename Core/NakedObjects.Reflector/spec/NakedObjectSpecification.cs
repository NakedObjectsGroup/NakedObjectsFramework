// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Objects.NotPersistable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Objects.Value;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Facets.Propcoll.NotPersisted;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.DotNet.Facets.Collections;
using NakedObjects.Reflector.DotNet.Facets.Naming.Named;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Plural;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Actions;
using NakedObjects.Reflector.DotNet.Reflect.Propcoll;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.Spec {
   
    public class NakedObjectSpecification : FacetHolderImpl, IIntrospectableSpecification {
        private static readonly ILog Log = LogManager.GetLogger(typeof (NakedObjectSpecification));
        private readonly DotNetReflector reflector;
        private INakedObjectAction[] contributedActions = new INakedObjectAction[] {};
        private INakedObjectAssociation[] fields;
        private string fullName;
        private IIconFacet iconFacet;
        private IIdentifier identifier;
        private INakedObjectSpecification[] interfaces = new INakedObjectSpecification[] {};
        private DotNetIntrospector introspector;
        private INakedObjectAction[] objectActions = new INakedObjectAction[] {};
        private INakedObjectAction[] relatedActions = new INakedObjectAction[] {};
        private bool service;
        private string shortName;
        private INakedObjectSpecification[] subclasses = new INakedObjectSpecification[] {};
        private INakedObjectSpecification superClassSpecification;
        private ITitleFacet titleFacet;
        private INakedObjectValidation[] validationMethods;
        private bool whetherAbstract;
        private bool whetherInterface;
        private bool whetherSealed;
        private bool whetherVoid;

        public NakedObjectSpecification(Type type, DotNetReflector reflector) {
            this.reflector = reflector;
            introspector = new DotNetIntrospector(type, this, reflector);
            identifier = new IdentifierImpl(reflector, type.FullName);
        }

        public bool IsSealed {
            get { return whetherSealed; }
        }

        public Type Type { get; set; }

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


        public virtual bool IsObject {
            get { return !IsCollection; }
        }


        public virtual INakedObjectSpecification Superclass {
            get { return superClassSpecification; }
        }


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


        public bool IsASet {
            get {
                var collectionFacet = GetFacet<ICollectionFacet>();
                if (collectionFacet != null) {
                    return collectionFacet.IsASet;
                }
                return false;
            }
        }

        public bool HasSubclasses {
            get { return subclasses.Length > 0; }
        }

        public INakedObjectSpecification[] Interfaces {
            get { return interfaces; }
        }

        public INakedObjectSpecification[] Subclasses {
            get { return subclasses; }
        }

        public bool IsAbstract {
            get { return whetherAbstract; }
        }

        public bool IsInterface {
            get { return whetherInterface; }
        }

        public bool IsService {
            get { return service; }
        }

        public string ShortName {
            get { return shortName; }
        }

        public string SingularName {
            get { return GetFacet<INamedFacet>().Value; }
        }

        public string UntitledName {
            get { return Resources.NakedObjects.Untitled + SingularName; }
        }

        public string PluralName {
            get { return GetFacet<IPluralFacet>().Value; }
        }

        public string Description {
            get { return GetFacet<IDescribedAsFacet>().Value ?? ""; }
        }

        public bool HasNoIdentity {
            get {
                // TODO need to tell whether an obj should be treated as a value or not
                return GetFacet<ICollectionFacet>() != null || GetFacet<IParseableFacet>() != null;
            }
        }

        public bool IsQueryable {
            get {
                var collectionFacet = GetFacet<ICollectionFacet>();
                if (collectionFacet != null && collectionFacet.GetType().IsGenericType) {
                    return collectionFacet.GetType().GetGenericTypeDefinition() == typeof (DotNetGenericIQueryableFacet<>);
                }
                return false;
            }
        }

        public bool IsVoid {
            get { return whetherVoid; }
        }

        public PersistableType Persistable {
            get {
                if (service) {
                    return PersistableType.ProgramPersistable;
                }
                if (ContainsFacet<INotPersistedFacet>()) {
                    return PersistableType.Transient;
                }
                if (ContainsFacet<IProgramPersistableOnlyFacet>()) {
                    return PersistableType.ProgramPersistable;
                }
                return PersistableType.UserPersistable;
            }
        }


        public void AddSubclass(INakedObjectSpecification subclass) {
            var subclassList = new List<INakedObjectSpecification>(subclasses) {subclass};
            subclasses = subclassList.ToArray();
        }

        /// <summary>
        ///     Determines if this class represents the same class, or a subclass, of the specified class.
        /// </summary>
        public bool IsOfType(INakedObjectSpecification specification) {
            if (specification == this) {
                return true;
            }
            if (interfaces.Any(interfaceSpec => interfaceSpec.IsOfType(specification))) {
                return true;
            }

            // match covariant generic types 
            if (Type.IsGenericType && IsCollection) {
                Type otherType = ((NakedObjectSpecification) specification).Type;
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

            return superClassSpecification != null && superClassSpecification.IsOfType(specification);
        }

        public void PopulateAssociatedActions(Type[] services) {
            if (string.IsNullOrWhiteSpace(FullName)) {
                string id = (identifier != null ? identifier.ClassName : "unknown") ?? "unknown";
                Log.WarnFormat("Specification with id : {0} as has null or empty name", id);
            }

            if (TypeUtils.IsSystem(FullName) && !IsCollection) {
                return;
            }
            if (TypeUtils.IsNakedObjects(FullName)) {
                return;
            }

            PopulateContributedActions(services);
            PopulateRelatedActions(services);
        }

        public void Introspect(FacetDecoratorSet decorator) {
            if (introspector == null) {
                throw new ReflectionException("Introspection already taken place, cannot introspect again");
            }

            introspector.IntrospectClass();

            Type = introspector.IntrospectedType;
            fullName = introspector.FullName;
            shortName = introspector.ShortName;
            var namedFacet = GetFacet<INamedFacet>();
            if (namedFacet == null) {
                namedFacet = new NamedFacetInferred(NameUtils.NaturalName(shortName), this);
                AddFacet(namedFacet);
            }

            var pluralFacet = GetFacet<IPluralFacet>();
            if (pluralFacet == null) {
                pluralFacet = new PluralFacetInferred(NameUtils.PluralName(namedFacet.Value), this);
                AddFacet(pluralFacet);
            }

            whetherAbstract = introspector.IsAbstract;
            whetherInterface = introspector.IsInterface;
            whetherSealed = introspector.IsSealed;
            whetherVoid = introspector.IsVoid;

            string superclassName = introspector.SuperclassName;
            string[] interfaceNames = introspector.InterfacesNames;


            if (superclassName != null && !TypeUtils.IsSystem(superclassName)) {
                superClassSpecification = reflector.LoadSpecification(superclassName);
                if (superClassSpecification != null) {
                    Log.DebugFormat("Superclass {0}", superclassName);
                    superClassSpecification.AddSubclass(this);
                }
            }
            else if (Type != typeof (object)) {
                // always root in object (unless this is object!) 
                superClassSpecification = reflector.LoadSpecification(typeof (object));
                if (superClassSpecification != null) {
                    Log.DebugFormat("Superclass {0}", typeof (object).Name);
                    superClassSpecification.AddSubclass(this);
                }
            }

            var interfaceList = new List<INakedObjectSpecification>();
            foreach (string interfaceName in interfaceNames) {
                INakedObjectSpecification interfaceSpec = reflector.LoadSpecification(interfaceName);
                interfaceSpec.AddSubclass(this);
                interfaceList.Add(interfaceSpec);
            }

            interfaces = interfaceList.ToArray();

            introspector.IntrospectPropertiesAndCollections();
            fields = OrderFields(introspector.Fields);

            validationMethods = introspector.IntrospectObjectValidationMethods();

            introspector.IntrospectActions();
            objectActions = OrderActions(introspector.ObjectActions);

            introspector = null;

            DecorateAllFacets(decorator);
            iconFacet = GetFacet<IIconFacet>();
        }

        public INakedObjectAssociation GetProperty(string id) {
            try {
                return fields.First(f => f.Id.Equals(id));
            }
            catch (InvalidOperationException) {
                throw new ReflectionException(string.Format("No field called '{0}' in '{1}'", id, SingularName));
            }
        }

        public string GetTitle(INakedObject nakedObject, INakedObjectManager manager) {
            if (titleFacet == null) {
                titleFacet = GetFacet<ITitleFacet>();
            }
            if (titleFacet != null) {
                return titleFacet.GetTitle(nakedObject, manager) ?? DefaultTitle();
            }
            return DefaultTitle();
        }

        public string GetInvariantString(INakedObject nakedObject) {
            var parser = GetFacet<IParseableFacet>();
            if (parser != null) {
                return parser.InvariantString(nakedObject);
            }
            return null;
        }

        public string GetIconName(INakedObject forObject) {
            string iconName = null;
            if (iconFacet != null) {
                iconName = forObject == null ? iconFacet.GetIconName() : iconFacet.GetIconName(forObject);
            }
            else if (IsCollection) {
                iconName = GetFacet<ITypeOfFacet>().ValueSpec.GetIconName(null);
            }

            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        public IConsent ValidToPersist(INakedObject target, ISession session) {
            InteractionContext ic = InteractionContext.PersistingObject(session, false, target);
            IConsent cons = InteractionUtils.IsValid(target.Specification, ic);
            return cons;
        }

        public void MarkAsService() {
            if (Properties.Any(field => field.Id != "Id")) {
                string fieldNames = Properties.Where(field => field.Id != "Id").Aggregate("", (current, field) => current + (current.Length > 0 ? ", " : "") /*+ field.GetName(persistor)*/);
                throw new ModelException(string.Format(Resources.NakedObjects.ServiceObjectWithFieldsError, FullName, fieldNames));
            }
            service = true;
        }

        public IEnumerable GetBoundedSet(ILifecycleManager persistor) {
            if (this.IsBoundedSet()) {
                if (Type.IsInterface) {
                    IList<object> instances = new List<object>();
                    foreach (INakedObjectSpecification spec in GetLeafNodes(this)) {
                        foreach (object instance in persistor.Instances(spec)) {
                            instances.Add(instance);
                        }
                    }
                    return instances;
                }
                return persistor.Instances(this);
            }
            return new object[] {};
        }

        public string UniqueShortName(string sep) {
            string postfix = string.Empty;
            Type type = TypeUtils.GetType(FullName);

            if (type.IsGenericType) {
                postfix = type.GetGenericArguments().Aggregate(string.Empty, (x, y) => x + sep + reflector.LoadSpecification(y).UniqueShortName(sep));
            }

            return ShortName + postfix;
        }

        #endregion

        private void PopulateContributedActions(Type[] services) {
            var serviceActionSets = new List<INakedObjectAction>();

            if (!IsService) {
                foreach (Type serviceType in services) {
                    INakedObjectSpecification specification = reflector.LoadSpecification(serviceType);
                    if (specification != this) {
                        INakedObjectAction[] matchingServiceActions = specification.GetActionLeafNodes().Where(serviceAction => serviceAction.IsContributedTo(this)).ToArray();
                        if (matchingServiceActions.Any()) {
                            var nakedObjectActionSet = new NakedObjectActionSet(specification.Identifier.ClassName,
                                matchingServiceActions);
                            serviceActionSets.Add(nakedObjectActionSet);
                        }
                    }
                }
            }
            contributedActions = serviceActionSets.ToArray();
        }

        private void PopulateRelatedActions(Type[] services) {
            var relatedActionSets = new List<INakedObjectAction>();
            foreach (Type serviceType in services) {
                INakedObjectSpecification specification = reflector.LoadSpecification(serviceType);
                var matchingActions = new List<INakedObjectAction>();
                foreach (INakedObjectAction serviceAction in specification.GetActionLeafNodes().Where(a => a.IsFinderMethod)) {
                    INakedObjectSpecification returnType = serviceAction.ReturnType;
                    if (returnType != null && returnType.IsCollection) {
                        INakedObjectSpecification elementType = returnType.GetFacet<ITypeOfFacet>().ValueSpec;
                        if (elementType.IsOfType(this)) {
                            matchingActions.Add(serviceAction);
                        }
                    }
                    else if (returnType != null && returnType.IsOfType(this)) {
                        matchingActions.Add(serviceAction);
                    }
                }
                if (matchingActions.Count > 0) {
                    var nakedObjectActionSet = new NakedObjectActionSet(specification.Identifier.ClassName,
                        matchingActions.ToArray());
                    relatedActionSets.Add(nakedObjectActionSet);
                }
            }
            relatedActions = relatedActionSets.ToArray();
        }


        private void DecorateAllFacets(FacetDecoratorSet decorator) {
            decorator.DecorateAllHoldersFacets(this);
            foreach (INakedObjectAssociation field in fields) {
                decorator.DecorateAllHoldersFacets(field);
            }
            foreach (INakedObjectAction action in objectActions) {
                DecorateAction(decorator, action);
            }
        }

        private static void DecorateAction(FacetDecoratorSet decorator, INakedObjectAction action) {
            decorator.DecorateAllHoldersFacets(action);
            foreach (INakedObjectActionParameter parm in action.Parameters) {
                decorator.DecorateAllHoldersFacets(parm);
            }
            if (action.ActionType == NakedObjectActionType.Set) {
                action.Actions.ForEach(a => DecorateAction(decorator, a));
            }
        }


        private INakedObjectAssociation[] OrderFields(OrderSet order) {
            var orderedFields = new List<INakedObjectAssociation>();
            foreach (IOrderableElement element in order) {
                if (element is DotNetNakedObjectAssociationPeer) {
                    orderedFields.Add(CreateNakedObjectField((DotNetNakedObjectAssociationPeer) element));
                }
                else if (element is OrderSet) {
                    // Not supported at present
                }
                else {
                    throw new UnknownTypeException(element);
                }
            }
            return orderedFields.ToArray();
        }

        private static INakedObjectAction[] OrderActions(OrderSet order) {
            var actions = new List<INakedObjectAction>();
            foreach (IOrderableElement element in order) {
                if (element is DotNetNakedObjectActionPeer) {
                    actions.Add(CreateNakedObjectAction((DotNetNakedObjectActionPeer) element));
                }
                else if (element is OrderSet) {
                    actions.Add(CreateNakedObjectActionSet((OrderSet) element));
                }
                else {
                    throw new UnknownTypeException(element);
                }
            }

            return actions.ToArray();
        }

        private static NakedObjectActionSet CreateNakedObjectActionSet(OrderSet orderSet) {
            return new NakedObjectActionSet(orderSet.GroupFullName.Replace(" ", ""), orderSet.GroupFullName, OrderActions(orderSet));
        }

        private static NakedObjectActionImpl CreateNakedObjectAction(INakedObjectActionPeer peer) {
            return new NakedObjectActionImpl(peer.Identifier.MemberName, peer);
        }

        private INakedObjectAssociation CreateNakedObjectField(INakedObjectAssociationPeer peer) {
            if (peer.IsOneToOne) {
                return new OneToOneAssociationImpl(reflector, peer);
            }
            if (peer.IsOneToMany) {
                return new OneToManyAssociationImpl(peer);
            }
            throw new ReflectionException("Unknown peer type: " + peer);
        }

        private string DefaultTitle() {
            return IsService ? SingularName : UntitledName;
        }

        private string TypeNameFor() {
            return IsCollection ? "Collection" : "Object";
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("class", fullName);
            str.Append("type", TypeNameFor());
            str.Append("persistable", Persistable);
            str.Append("superclass", superClassSpecification == null ? "object" : superClassSpecification.FullName);
            return str.ToString();
        }


        private static IEnumerable<INakedObjectSpecification> GetLeafNodes(INakedObjectSpecification spec) {
            if ((spec.IsInterface || spec.IsAbstract)) {
                return spec.Subclasses.SelectMany(GetLeafNodes);
            }
            return new[] {spec};
        }
    }
}

// Copyright (c) Naked Objects Group Ltd.