// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.DotNet.Facets.Naming.Named;
using NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Plural;
using NakedObjects.Reflector.DotNet.Facets.Ordering;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.DotNet.Reflect.Actions;
using NakedObjects.Reflector.DotNet.Reflect.Propcoll;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.Spec {





    public class IntrospectableSpecification : FacetHolderImpl, IIntrospectableSpecification {
        private static readonly ILog Log = LogManager.GetLogger(typeof (IntrospectableSpecification));

        private readonly IdentifierImpl identifier;
        private readonly INakedObjectReflector reflector;
        private INakedObjectAction[] contributedActions;
        private INakedObjectAssociation[] fields;
        private string fullName;
        private IIconFacet iconFacet;
        private IIntrospectableSpecification[] interfaces;
        private DotNetIntrospector introspector;
        private INakedObjectAction[] objectActions = {};
        private INakedObjectAction[] relatedActions;
        private bool service;
        private string shortName;
        private IIntrospectableSpecification[] subclasses = {};
        private IIntrospectableSpecification superClassSpecification;
        private INakedObjectValidation[] validationMethods;
        private bool whetherAbstract;
        private bool whetherInterface;
        private bool whetherSealed;
        private bool whetherVoid;

        public IntrospectableSpecification(Type type, INakedObjectReflector reflector) {
            this.reflector = reflector;
            introspector = new DotNetIntrospector(type, this, reflector);
            identifier = new IdentifierImpl((IMetadata) reflector, type.FullName);
        }

        private bool IsCollection {
            get { return ContainsFacet(typeof (ICollectionFacet)); }
        }

        private IIntrospectableSpecification Superclass {
            get { return superClassSpecification; }
        }

        public Type Type { get; set; }

        #region IIntrospectableSpecification Members

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
            if (interfaces != null) {
                var interfaceSpecs = interfaces;
                foreach (var interfaceSpec in interfaceSpecs) {
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

        public void Introspect(IFacetDecoratorSet decorator) {
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

            var interfaceList = new List<IIntrospectableSpecification>();
            foreach (string interfaceName in interfaceNames) {
                var interfaceSpec = reflector.LoadSpecification(interfaceName);
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

        public void PopulateAssociatedActions(Type[] services) {
            if (string.IsNullOrWhiteSpace(fullName)) {
                string id = (identifier != null ? identifier.ClassName : "unknown") ?? "unknown";
                Log.WarnFormat("Specification with id : {0} as has null or empty name", id);
            }

            if (TypeUtils.IsSystem(fullName) && !IsCollection) {
                return;
            }
            if (TypeUtils.IsNakedObjects(fullName)) {
                return;
            }

            PopulateContributedActions(services);
            PopulateRelatedActions(services);
        }

        public void MarkAsService() {
            if (fields.Any(field => field.Id != "Id")) {
                string fieldNames = fields.Where(field => field.Id != "Id").Aggregate("", (current, field) => current + (current.Length > 0 ? ", " : "") /*+ field.GetName(persistor)*/);
                throw new ModelException(string.Format(Resources.NakedObjects.ServiceObjectWithFieldsError, fullName, fieldNames));
            }
            service = true;
        }

        public void AddSubclass(IIntrospectableSpecification subclass) {
            var subclassList = new List<IIntrospectableSpecification>(subclasses) {subclass};
            subclasses = subclassList.ToArray();
        }

        #endregion

        private void DecorateAllFacets(IFacetDecoratorSet decorator) {
            decorator.DecorateAllHoldersFacets(this);
            foreach (INakedObjectAssociation field in fields) {
                decorator.DecorateAllHoldersFacets(field);
            }
            foreach (INakedObjectAction action in objectActions) {
                DecorateAction(decorator, action);
            }
        }

        private static void DecorateAction(IFacetDecoratorSet decorator, INakedObjectAction action) {
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
                return new OneToOneAssociationImpl((IMetadata) reflector, peer);
            }
            if (peer.IsOneToMany) {
                return new OneToManyAssociationImpl(peer);
            }
            throw new ReflectionException("Unknown peer type: " + peer);
        }

        private void PopulateContributedActions(Type[] services) {
            var serviceActionSets = new List<INakedObjectAction>();

            if (!service) {
                foreach (Type serviceType in services) {
                    var serviceSpecification = ((IMetadata) reflector).GetSpecification(serviceType);
                    if (serviceType != Type) {
                        var thisSpecification = ((IMetadata) reflector).GetSpecification(Type);
                        INakedObjectAction[] matchingServiceActions = serviceSpecification.GetActionLeafNodes().Where(serviceAction => serviceAction.IsContributedTo(thisSpecification)).ToArray();
                        if (matchingServiceActions.Any()) {
                            var nakedObjectActionSet = new NakedObjectActionSet(serviceSpecification.Identifier.ClassName,
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
                var thisSpecification = new NakedObjectSpecification(this);
                var serviceSpecification = ((IMetadata) reflector).GetSpecification(serviceType);
                var matchingActions = new List<INakedObjectAction>();
                foreach (INakedObjectAction serviceAction in serviceSpecification.GetActionLeafNodes().Where(a => a.IsFinderMethod)) {
                    INakedObjectSpecification returnType = serviceAction.ReturnType;
                    if (returnType != null && returnType.IsCollection) {
                        INakedObjectSpecification elementType = returnType.GetFacet<ITypeOfFacet>().ValueSpec;
                        if (elementType.IsOfType(thisSpecification)) {
                            matchingActions.Add(serviceAction);
                        }
                    }
                    else if (returnType != null && returnType.IsOfType(thisSpecification)) {
                        matchingActions.Add(serviceAction);
                    }
                }
                if (matchingActions.Count > 0) {
                    var nakedObjectActionSet = new NakedObjectActionSet(serviceSpecification.Identifier.ClassName,
                        matchingActions.ToArray());
                    relatedActionSets.Add(nakedObjectActionSet);
                }
            }
            relatedActions = relatedActionSets.ToArray();
        }
    }
}