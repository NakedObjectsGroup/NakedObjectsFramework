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
using NakedObjects.Core.Util;
using NakedObjects.Reflector.DotNet.Facets.Collections;
using NakedObjects.Reflector.DotNet.Reflect;
using NakedObjects.Reflector.Peer;
using NakedObjects.Util;

namespace NakedObjects.Reflector.Spec {

    public class NakedObjectSpecification : FacetHolderImpl, INakedObjectSpecification {
        private readonly IIntrospectableSpecification innerSpec;
        private static readonly ILog Log = LogManager.GetLogger(typeof (NakedObjectSpecification));
     
        private INakedObjectAction[] contributedActions = {};
        private INakedObjectAssociation[] fields;
        private string fullName;
        private IIconFacet iconFacet;
        private IIdentifier identifier;
        private INakedObjectSpecification[] interfaces = {};
        private DotNetIntrospector introspector;
        private INakedObjectAction[] objectActions = {};
        private INakedObjectAction[] relatedActions = {};
        private bool service;
        private string shortName;
        private INakedObjectSpecification[] subclasses = {};
        private INakedObjectSpecification superClassSpecification;
        private ITitleFacet titleFacet;
        private INakedObjectValidation[] validationMethods;
        private bool whetherAbstract;
        private bool whetherInterface;
        private bool whetherSealed;
        private bool whetherVoid;

        public NakedObjectSpecification(IIntrospectableSpecification innerSpec) {
            this.innerSpec = innerSpec;
        }

        public bool IsSealed {
            get { return whetherSealed; }
        }

        public Type Type { get; set; }

      
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
            get { return superClassSpecification; }
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
                var collectionFacet = innerSpec.GetFacet<ICollectionFacet>();
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
            get { return whetherVoid; }
        }

        public PersistableType Persistable {
            get {
                if (service) {
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

            //if (type.IsGenericType) {
            //    postfix = type.GetGenericArguments().Aggregate(string.Empty, (x, y) => x + sep + reflector.LoadSpecification(y).UniqueShortName(sep));
            //}
            throw new NotImplementedException();

            //return ShortName + postfix;
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