// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Objects.NotPersistable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Propcoll.NotPersisted;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Context;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.DotNet.Facets.Collections;
using NakedObjects.Reflector.DotNet.Facets.Naming.Named;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Plural;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.DotNet.Reflect.Actions;
using NakedObjects.Reflector.DotNet.Reflect.Propcoll;
using NakedObjects.Reflector.DotNet.Reflect.Proxies;
using NakedObjects.Reflector.Peer;
using NakedObjects.Reflector.Spec;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Reflect {
    public class DotNetSpecification : NakedObjectSpecificationAbstract {
        private readonly DotNetReflector reflector;
        private static readonly ILog Log = LogManager.GetLogger(typeof (DotNetSpecification));

        private INakedObjectSpecification[] interfaces = new INakedObjectSpecification[] {};
        private INakedObjectSpecification[] subclasses = new INakedObjectSpecification[] {}; 
        
        private IIconFacet iconFacet;
        private DotNetIntrospector introspector;

        private bool service;
        private string shortName;
        private ITitleFacet titleFacet;

        private bool whetherAbstract;
        private bool whetherInterface;
        private bool whetherSealed;
        private bool whetherVoid;

        public DotNetSpecification(Type type, DotNetReflector reflector) {
            this.reflector = reflector;
            introspector = new DotNetIntrospector(type, this, reflector);
            identifier = new IdentifierImpl(reflector, type.FullName);
        }

        public override bool HasSubclasses {
            get { return subclasses.Length > 0; }
        }

        public override INakedObjectSpecification[] Interfaces {
            get { return interfaces; }
        }

        public override INakedObjectSpecification[] Subclasses {
            get { return subclasses; }
        }

        public override bool IsAbstract {
            get { return whetherAbstract; }
        }

        public override bool IsInterface {
            get { return whetherInterface; }
        }

        public override bool IsSealed {
            get { return whetherSealed; }
        }

        public override bool IsService {
            get { return service; }
        }

        public override string ShortName {
            get { return shortName; }
        }

        public override string SingularName {
            get { return GetFacet<INamedFacet>().Value; }
        }

        public override string UntitledName {
            get { return Resources.NakedObjects.Untitled + SingularName; }
        }

        public override string PluralName {
            get { return GetFacet<IPluralFacet>().Value; }
        }

        public override string Description {
            get { return GetFacet<IDescribedAsFacet>().Value ?? ""; }
        }

        public override bool HasNoIdentity {
            get {
                // TODO need to tell whether an obj should be treated as a value or not
                return GetFacet<ICollectionFacet>() != null || GetFacet<IParseableFacet>() != null;
            }
        }

        public override bool IsQueryable {
            get {
                var collectionFacet = GetFacet<ICollectionFacet>();
                if (collectionFacet != null && collectionFacet.GetType().IsGenericType) {
                    return collectionFacet.GetType().GetGenericTypeDefinition() == typeof (DotNetGenericIQueryableFacet<>);
                }
                return false;
            }
        }

        public override bool IsVoid {
            get { return whetherVoid; }
        }

        public override Persistable Persistable {
            get {
                if (service) {
                    return Persistable.PROGRAM_PERSISTABLE;
                }
                if (ContainsFacet<INotPersistedFacet>()) {
                    return Persistable.TRANSIENT;
                }
                if (ContainsFacet<IProgramPersistableOnlyFacet>()) {
                    return Persistable.PROGRAM_PERSISTABLE;
                }
                return Persistable.USER_PERSISTABLE;
            }
        }

        public Type Type { get; set; }


        public override void AddSubclass(INakedObjectSpecification subclass) {
            var subclassList = new List<INakedObjectSpecification>(subclasses) {subclass};
            subclasses = subclassList.ToArray();
        }

        /// <summary>
        ///     Determines if this class represents the same class, or a subclass, of the specified class.
        /// </summary>
        public override bool IsOfType(INakedObjectSpecification specification) {
            if (specification == this) {
                return true;
            }
            if (interfaces.Any(interfaceSpec => interfaceSpec.IsOfType(specification))) {
                return true;
            }

            // match covariant generic types 
            if (Type.IsGenericType && IsCollection) {
                Type otherType = ((DotNetSpecification) specification).Type;
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

        public override void PopulateAssociatedActions(INakedObject[] services) {
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

        private  void PopulateContributedActions(INakedObject[] services) {
            var serviceActionSets = new List<INakedObjectAction>();

            if (!IsService) {
                foreach (INakedObject serviceAdapter in services) {
                    INakedObjectSpecification specification = serviceAdapter.Specification;
                    if (specification != this) {
                        INakedObjectAction[] matchingServiceActions = specification.GetActionLeafNodes().Where(serviceAction => serviceAction.IsContributedTo(this)).ToArray();
                        if (matchingServiceActions.Any()) {
                            var nakedObjectActionSet = new NakedObjectActionSet(serviceAdapter.Specification.Identifier.ClassName,
                                                                                serviceAdapter.TitleString(),
                                                                                matchingServiceActions);
                            serviceActionSets.Add(nakedObjectActionSet);
                        }
                    }
                }
            }
            contributedActions = serviceActionSets.ToArray();
        }

        private void PopulateRelatedActions(INakedObject[] services) {
            var relatedActionSets = new List<INakedObjectAction>();
            foreach (INakedObject serviceAdapter in services) {
                var matchingActions = new List<INakedObjectAction>();
                foreach (INakedObjectAction serviceAction in serviceAdapter.Specification.GetActionLeafNodes().Where(a => a.IsFinderMethod)) {
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
                    var nakedObjectActionSet = new NakedObjectActionSet(serviceAdapter.Specification.Identifier.ClassName,
                                                                        serviceAdapter.TitleString(),
                                                                        matchingActions.ToArray());
                    relatedActionSets.Add(nakedObjectActionSet);
                }
            }
            relatedActions = relatedActionSets.ToArray();
        }


        public override void Introspect(FacetDecoratorSet decorator) {
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


        public override INakedObjectAssociation GetProperty(string id) {
            try {
                return fields.First(f => f.Id.Equals(id));
            }
            catch (InvalidOperationException) {
                throw new ReflectionException(string.Format("No field called '{0}' in '{1}'", id, SingularName));
            }
        }

        private  INakedObjectAssociation[] OrderFields(OrderSet order) {
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

        private  INakedObjectAssociation CreateNakedObjectField(INakedObjectAssociationPeer peer) {
            if (peer.IsOneToOne) {
                return new OneToOneAssociationImpl(reflector, peer);
            }
            if (peer.IsOneToMany) {
                return new OneToManyAssociationImpl(peer);
            }
            throw new ReflectionException("Unknown peer type: " + peer);
        }

        public override string GetTitle(INakedObject nakedObject, INakedObjectManager manager) {
            if (titleFacet == null) {
                titleFacet = GetFacet<ITitleFacet>();
            }
            if (titleFacet != null) {
                return titleFacet.GetTitle(nakedObject, manager) ?? DefaultTitle();
            }
            return DefaultTitle();
        }

        private string DefaultTitle() {
            return IsService ? SingularName : UntitledName;
        }

        public override string GetInvariantString(INakedObject nakedObject) {
            var parser = GetFacet<IParseableFacet>();
            if (parser != null) {
                return parser.InvariantString(nakedObject);
            }
            return null;
        }

        public override string GetIconName(INakedObject forObject) {
            string iconName = null;
            if (iconFacet != null) {
                iconName = forObject == null ? iconFacet.GetIconName() : iconFacet.GetIconName(forObject);
            }
            else if (IsCollection) {
                iconName = GetFacet<ITypeOfFacet>().ValueSpec.GetIconName(null);
            }

            return string.IsNullOrEmpty(iconName) ? "Default" : iconName;
        }

        public override IConsent ValidToPersist(INakedObject target, ISession session) {
            InteractionContext ic = InteractionContext.PersistingObject(session, false, target);
            IConsent cons = InteractionUtils.IsValid(target.Specification, ic);
            return cons;
        }

        public override void MarkAsService() {
            if (Properties.Any(field => field.Id != "Id")) {
                string fieldNames = Properties.Where(field => field.Id != "Id").Aggregate("", (current, field) => current + (current.Length > 0 ? ", " : "") + field.Name);
                throw new ModelException(string.Format(Resources.NakedObjects.ServiceObjectWithFieldsError, FullName, fieldNames));
            }
            service = true;
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

        public override object CreateObject(INakedObjectPersistor persistor) {
            if (Type.IsArray) {
                return Array.CreateInstance(Type.GetElementType(), 0);
            }
            if (Type.IsAbstract) {
                throw new ModelException(string.Format(Resources.NakedObjects.CannotCreateAbstract, Type));
            }
            object domainObject = Activator.CreateInstance(ProxyCreator.CreateProxyType(reflector, Type));
            persistor.InitDomainObject(domainObject);
            return domainObject;
        }

        private static IEnumerable<INakedObjectSpecification> GetLeafNodes(INakedObjectSpecification spec) {
            if ((spec.IsInterface || spec.IsAbstract)) {
                return spec.Subclasses.SelectMany(GetLeafNodes);
            }
            return new[] {spec};
        }

        public override IEnumerable GetBoundedSet(INakedObjectPersistor persistor) {
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
    }

    // Copyright (c) Naked Objects Group Ltd.
}