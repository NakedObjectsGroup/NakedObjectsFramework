// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core.Util;
using NakedObjects.Util;

namespace NakedObjects.Core.Spec {
    public class ObjectSpec : IObjectSpec {
        private static readonly ILog Log = LogManager.GetLogger(typeof (ObjectSpec));
        private readonly IObjectSpecImmutable innerSpec;
        private readonly SpecFactory memberFactory;
        private readonly IMetamodelManager metamodelManager;
        private IActionSpec[] combinedActions;
        private IActionSpec[] contributedActions;
        private IActionSpec[] objectActions;
        private IAssociationSpec[] objectFields;
        private IActionSpec[] relatedActions;

        public ObjectSpec(SpecFactory memberFactory, IMetamodelManager metamodelManager, IObjectSpecImmutable innerSpec) {
            this.memberFactory = memberFactory;
            this.metamodelManager = metamodelManager;
            this.innerSpec = innerSpec;

            Assert.AssertNotNull(metamodelManager);
            Assert.AssertNotNull(innerSpec);
        }

        public bool IsSealed {
            get { return innerSpec.ContainsFacet(typeof (ISealedFacet)); }
        }

        public Type Type {
            get { return innerSpec.Type; }
        }

        private IActionSpec[] ObjectActions {
            get {
                if (objectActions == null) {
                    objectActions = memberFactory.OrderActions(innerSpec.ObjectActions);
                }
                return objectActions;
            }
        }

        private IActionSpec[] ContributedActions {
            get {
                if (contributedActions == null) {
                    contributedActions = memberFactory.OrderActions(innerSpec.ContributedActions);
                }
                return contributedActions;
            }
        }

        private IActionSpec[] RelatedActions {
            get {
                if (relatedActions == null) {
                    relatedActions = memberFactory.OrderActions(innerSpec.RelatedActions);
                }
                return relatedActions;
            }
        }

        #region IObjectSpec Members

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

        public IEnumerable<IFacet> GetFacets() {
            return innerSpec.GetFacets();
        }

        public virtual bool IsParseable {
            get { return innerSpec.ContainsFacet(typeof (IParseableFacet)); }
        }

        public virtual bool IsEncodeable {
            get { return innerSpec.ContainsFacet(typeof (IEncodeableFacet)); }
        }

        public virtual bool IsAggregated {
            get { return innerSpec.ContainsFacet(typeof (IAggregatedFacet)); }
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

        public virtual IObjectSpec Superclass {
            get { return innerSpec.Superclass == null ? null : metamodelManager.GetSpecification(innerSpec.Superclass); }
        }

        public virtual IAssociationSpec[] Properties {
            get {
                if (objectFields == null) {
                    objectFields = OrderFields(innerSpec.Fields);
                }
                return objectFields;
            }
        }

        public virtual IActionSpec[] GetAllActions() {
            if (combinedActions == null) {
                var ca = new List<IActionSpec>();
                ca.AddRange(ObjectActions);
                ca.AddRange(ContributedActions);
                combinedActions = ca.ToArray();
            }
            return combinedActions;
        }

        public virtual IActionSpec[] GetRelatedServiceActions() {
            return RelatedActions;
        }

        public IMenuImmutable ObjectMenu {
            get { return innerSpec.ObjectMenu; }
        }

        public bool IsASet {
            get {
                var collectionFacet = innerSpec.GetFacet<ICollectionFacet>();
                return collectionFacet != null && collectionFacet.IsASet;
            }
        }

        public bool HasSubclasses {
            get { return innerSpec.Subclasses.Count > 0; }
        }

        public IObjectSpec[] Interfaces {
            get { return innerSpec.Interfaces.Select(i => metamodelManager.GetSpecification(i)).ToArray(); }
        }

        public IObjectSpec[] Subclasses {
            get { return innerSpec.Subclasses.Select(i => metamodelManager.GetSpecification(i)).ToArray(); }
        }

        public bool IsAbstract {
            get { return innerSpec.ContainsFacet(typeof (IAbstractFacet)); }
        }

        public bool IsInterface {
            get { return innerSpec.ContainsFacet(typeof (IInterfaceFacet)); }
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
                return collectionFacet != null && collectionFacet.IsQueryable;
            }
        }

        public bool IsVoid {
            get { return innerSpec.ContainsFacet(typeof (IVoidFacet)); }
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
        public bool IsOfType(IObjectSpec spec) {
            if (spec.Equals(this)) {
                return true;
            }
            if (Interfaces.Any(interfaceSpec => interfaceSpec.IsOfType(spec))) {
                return true;
            }

            // match covariant generic types 
            if (Type.IsGenericType && IsCollection) {
                Type otherType = ((ObjectSpec) spec).Type;
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

            return Superclass != null && Superclass.IsOfType(spec);
        }


        public IAssociationSpec GetProperty(string id) {
            try {
                return Properties.First(f => f.Id.Equals(id));
            }
            catch (InvalidOperationException) {
                throw new ReflectionException(string.Format("No field called '{0}' in '{1}'", id, SingularName));
            }
        }

        public string GetTitle(INakedObject nakedObject) {
            return innerSpec.GetTitle(nakedObject);
        }

        public string GetInvariantString(INakedObject nakedObject) {
            var parser = GetFacet<IParseableFacet>();
            return parser == null ? null : parser.InvariantString(nakedObject);
        }

        public string GetIconName(INakedObject forObject) {
            return innerSpec.GetIconName(forObject, metamodelManager.Metamodel);
        }

        public IConsent ValidToPersist(INakedObject target, ISession session) {
            InteractionContext ic = InteractionContext.PersistingObject(session, false, target);
            IConsent cons = InteractionUtils.IsValid(target.Spec, ic);
            return cons;
        }


        public string UniqueShortName(string sep) {
            string postfix = string.Empty;
            Type type = TypeUtils.GetType(FullName);

            if (type.IsGenericType) {
                postfix = type.GetGenericArguments().Aggregate(string.Empty, (x, y) => x + sep + metamodelManager.GetSpecification(y).UniqueShortName(sep));
            }

            return ShortName + postfix;
        }

        #endregion

        private string TypeNameFor() {
            return IsCollection ? "Collection" : "Object";
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append("class", FullName);
            str.Append("type", TypeNameFor());
            str.Append("persistable", Persistable);
            str.Append("superclass", innerSpec.Superclass == null ? "object" : innerSpec.Superclass.FullName);
            return str.ToString();
        }


        private IAssociationSpec[] OrderFields(IList<IOrderableElement<IAssociationSpecImmutable>> order) {
            var orderedFields = new List<IAssociationSpec>();
            foreach (var element in order) {
                if (element.Spec != null) {
                    orderedFields.Add(memberFactory.CreateAssociationSpec(element.Spec));
                }
                else if (element.Set != null) {
                    throw new NotImplementedException();
                }
                else {
                    throw new UnknownTypeException(element);
                }
            }
            return orderedFields.ToArray();
        }

        protected bool Equals(ObjectSpec other) {
            return Equals(innerSpec, other.innerSpec);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ObjectSpec) obj);
        }

        public override int GetHashCode() {
            return (innerSpec != null ? innerSpec.GetHashCode() : 0);
        }
    }
}

// Copyright (c) Naked Objects Group Ltd.