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
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Architecture.Facets.Objects.Encodeable;
using NakedObjects.Architecture.Facets.Objects.Ident.Plural;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Facets.Objects.NotPersistable;
using NakedObjects.Architecture.Facets.Objects.Parseable;
using NakedObjects.Architecture.Facets.Objects.Value;
using NakedObjects.Architecture.Facets.Objects.ViewModel;
using NakedObjects.Architecture.Facets.Propcoll.NotPersisted;
using NakedObjects.Architecture.Facets.Types;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.DotNet.Facets.Collections;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.Spec {

    public class NakedObjectSpecification :  INakedObjectSpecification {
       

        private readonly IMetadata metadata;
        private readonly IIntrospectableSpecification innerSpec;
        private static readonly ILog Log = LogManager.GetLogger(typeof (NakedObjectSpecification));
        private INakedObjectAction[] objectActions;
        private INakedObjectAssociation[] objectFields;
        private INakedObjectAction[] contributedActions;
        private INakedObjectAction[] relatedActions;
        private INakedObjectAction[] combinedActions;

        public NakedObjectSpecification(IMetadata metadata, IIntrospectableSpecification innerSpec) {
            this.metadata = metadata;
            this.innerSpec = innerSpec;

            Assert.AssertNotNull(metadata);
            Assert.AssertNotNull(innerSpec);
        }

        public bool IsSealed {
            get { return innerSpec.ContainsFacet(typeof (ISealedFacet)); }
        }

        public Type Type {
            get { return innerSpec.Type; }
        }
    
        public virtual string FullName {
            get { return innerSpec.FullName; }
        }

        public virtual object DefaultValue {
            get { return null; }
        }

        public Type[] FacetTypes { get; private set; }

        public IIdentifier Identifier {
            get { return innerSpec.Identifier; }
        }

        public bool ContainsFacet(Type facetType) {
            return innerSpec.ContainsFacet(facetType);
        }

        public bool ContainsFacet<T>() where T : IFacet {
            return innerSpec.ContainsFacet<T>();
        }

        public IFacet GetFacet(Type type) {
            return innerSpec.GetFacet(type);
        }

        public T GetFacet<T>() where T : IFacet {
            return innerSpec.GetFacet<T>();
        }

        public IFacet[] GetFacets(IFacetFilter filter) {
            return innerSpec.GetFacets(filter);
        }

        public void AddFacet(IFacet facet) {
            throw new NotImplementedException();
        }

        public void AddFacet(IMultiTypedFacet facet) {
            throw new NotImplementedException();
        }

        public void RemoveFacet(IFacet facet) {
            throw new NotImplementedException();
        }

        public void RemoveFacet(Type facetType) {
            throw new NotImplementedException();
        }

        public virtual bool IsParseable {
            get { return innerSpec.ContainsFacet(typeof (IParseableFacet)); }
        }

        public virtual bool IsEncodeable {
            get { return innerSpec.ContainsFacet(typeof (IEncodeableFacet)); }
        }

        public virtual bool IsAggregated {
            get { return innerSpec.ContainsFacet(typeof (IAggregatedFacet)) || innerSpec.ContainsFacet(typeof (IValueFacet)); }
        }

        public virtual bool IsCollection {
            get { return innerSpec.ContainsFacet(typeof (ICollectionFacet)); }
        }

        public virtual bool IsViewModel {
            get { return innerSpec.ContainsFacet(typeof (IViewModelFacet)); }
        }

        public virtual bool IsObject {
            get { return !IsCollection; }
        }

        public virtual INakedObjectSpecification Superclass {
            get { return  innerSpec.Superclass == null ? null : new NakedObjectSpecification(metadata, innerSpec.Superclass); }
        }

        public virtual INakedObjectAssociation[] Properties {
            get {
                if (objectFields == null) {
                    objectFields = OrderFields(innerSpec.Fields);
                }
                return objectFields;
            }
        }

        private  INakedObjectAction[] ObjectActions {
            get {
                if (objectActions == null) {
                    objectActions = OrderActions(innerSpec.ObjectActions);
                }
                return objectActions;
            }
        }

        private INakedObjectAction[] ContributedActions {
            get {
                if (contributedActions == null) {
                    contributedActions = OrderActions(innerSpec.ContributedActions);
                }
                return contributedActions;
            }
        }

        private INakedObjectAction[] RelatedActions {
            get {
                if (relatedActions == null) {
                    relatedActions = OrderActions(innerSpec.RelatedActions);
                }
                return relatedActions;
            }
        }


        public INakedObjectValidation[] ValidateMethods() {
            return innerSpec.ValidationMethods;
        }

        public virtual INakedObjectAction[] GetObjectActions() {
            if (combinedActions == null) {
                var ca = new List<INakedObjectAction>();
                ca.AddRange(ObjectActions);
                ca.AddRange(ContributedActions);
                combinedActions = ca.ToArray();
            }
            return combinedActions;
        }

        public virtual INakedObjectAction[] GetRelatedServiceActions() {
            return RelatedActions;
        }

        public bool IsASet {
            get {
                var collectionFacet = innerSpec.GetFacet<ICollectionFacet>();
                if (collectionFacet != null) {
                    return collectionFacet.IsASet;
                }
                return false;
            }
        }

        public bool HasSubclasses {
            get { return innerSpec.Subclasses.Length > 0; }
        }

        public INakedObjectSpecification[] Interfaces {
            get { return innerSpec.Interfaces.Select(i => new NakedObjectSpecification(metadata, i)).Cast<INakedObjectSpecification>().ToArray(); }
        }

        public INakedObjectSpecification[] Subclasses {
            get { return innerSpec.Subclasses.Select(i => new NakedObjectSpecification(metadata, i)).Cast<INakedObjectSpecification>().ToArray(); }
        }

        public bool IsAbstract {
            get { return innerSpec.ContainsFacet(typeof(IAbstractFacet)); }
        }

        public bool IsInterface {
            get { return innerSpec.ContainsFacet(typeof(IInterfaceFacet)); }
        }

        public bool IsService {
            get { return innerSpec.Service; }
        }

        public string ShortName {
            get { return innerSpec.ShortName; }
        }

        public string SingularName {
            get { return innerSpec.GetFacet<INamedFacet>().Value; }
        }

        public string UntitledName {
            get { return Resources.NakedObjects.Untitled + SingularName; }
        }

        public string PluralName {
            get { return innerSpec.GetFacet<IPluralFacet>().Value; }
        }

        public string Description {
            get { return innerSpec.GetFacet<IDescribedAsFacet>().Value ?? ""; }
        }

        public bool HasNoIdentity {
            get {
                // TODO need to tell whether an obj should be treated as a value or not
                return innerSpec.GetFacet<ICollectionFacet>() != null || innerSpec.GetFacet<IParseableFacet>() != null;
            }
        }

        public bool IsQueryable {
            get {
                var collectionFacet = innerSpec.GetFacet<ICollectionFacet>();
                if (collectionFacet != null && collectionFacet.GetType().IsGenericType) {
                    return collectionFacet.GetType().GetGenericTypeDefinition() == typeof (DotNetGenericIQueryableFacet<>);
                }
                return false;
            }
        }

        public bool IsVoid {
            get { return innerSpec.ContainsFacet(typeof(IVoidFacet)); }
        }

        public PersistableType Persistable {
            get {
                if (IsService) {
                    return PersistableType.ProgramPersistable;
                }
                if (innerSpec.ContainsFacet<INotPersistedFacet>()) {
                    return PersistableType.Transient;
                }
                if (innerSpec.ContainsFacet<IProgramPersistableOnlyFacet>()) {
                    return PersistableType.ProgramPersistable;
                }
                return PersistableType.UserPersistable;
            }
        }

        /// <summary>
        ///     Determines if this class represents the same class, or a subclass, of the specified class.
        /// </summary>
        public bool IsOfType(INakedObjectSpecification specification) {


            if (specification == this) {
                return true;
            }
            if (Interfaces.Any(interfaceSpec => interfaceSpec.IsOfType(specification))) {
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

            return Superclass != null && Superclass.IsOfType(specification);
        }

      
        public INakedObjectAssociation GetProperty(string id) {
            try {
                return Properties.First(f => f.Id.Equals(id));
            }
            catch (InvalidOperationException) {
                throw new ReflectionException(string.Format("No field called '{0}' in '{1}'", id, SingularName));
            }
        }

        public string GetTitle(INakedObject nakedObject, INakedObjectManager manager) {

           
                var titleFacet = GetFacet<ITitleFacet>();
         
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
            return innerSpec.GetIconName(forObject);
         
        }

        public IConsent ValidToPersist(INakedObject target, ISession session) {
            InteractionContext ic = InteractionContext.PersistingObject(session, false, target);
            IConsent cons = InteractionUtils.IsValid(target.Specification, ic);
            return cons;
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
                postfix = type.GetGenericArguments().Aggregate(string.Empty, (x, y) => x + sep + metadata.GetSpecification(y).UniqueShortName(sep));
            }

            return ShortName + postfix;
        }

        private string DefaultTitle() {
            return IsService ? SingularName : UntitledName;
        }

        private string TypeNameFor() {
            return IsCollection ? "Collection" : "Object";
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("class", FullName);
            str.Append("type", TypeNameFor());
            str.Append("persistable", Persistable);
            str.Append("superclass", innerSpec.Superclass  == null ? "object" : innerSpec.Superclass.FullName);
            return str.ToString();
        }


        private static IEnumerable<INakedObjectSpecification> GetLeafNodes(INakedObjectSpecification spec) {
            if ((spec.IsInterface || spec.IsAbstract)) {
                return spec.Subclasses.SelectMany(GetLeafNodes);
            }
            return new[] {spec};
        }

        private INakedObjectAction[] OrderActions(INakedObjectActionPeer[] order) {
            var actions = new List<INakedObjectAction>();
            foreach (var element in order) {
                if (element.Peer != null) {
                    actions.Add(CreateNakedObjectAction(element.Peer));
                }
                else if (element.Set != null) {
                    //actions.Add(CreateNakedObjectActionSet(element.Set));
                }
                else {
                    throw new UnknownTypeException(element);
                }
            }

            return actions.ToArray();
        }

        //private NakedObjectActionSet CreateNakedObjectActionSet(IOrderSet<INakedObjectActionPeer> orderSet) {
        //    return new NakedObjectActionSet(orderSet.GroupFullName.Replace(" ", ""), orderSet.GroupFullName, OrderActions(orderSet));
        //}

        private NakedObjectActionImpl CreateNakedObjectAction(INakedObjectActionPeer peer) {
            return new NakedObjectActionImpl(metadata, peer);
        }

        private INakedObjectAssociation CreateNakedObjectField(INakedObjectAssociationPeer peer) {
            return NakedObjectAssociationAbstract.CreateAssociation(metadata, peer);
        }

        private INakedObjectAssociation[] OrderFields(INakedObjectAssociationPeer[] order) {
            var orderedFields = new List<INakedObjectAssociation>();
            foreach (var element in order) {
                if (element.Peer != null) {
                    orderedFields.Add(CreateNakedObjectField(element.Peer));
                }
                else if (element.Set != null) {
                    // Not supported at present
                }
                else {
                    throw new UnknownTypeException(element);
                }
            }
            return orderedFields.ToArray();
        }

        protected bool Equals(NakedObjectSpecification other) {
            return Equals(innerSpec, other.innerSpec);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NakedObjectSpecification)obj);
        }

        public override int GetHashCode() {
            return (innerSpec != null ? innerSpec.GetHashCode() : 0);
        }

       
    }
}

// Copyright (c) Naked Objects Group Ltd.