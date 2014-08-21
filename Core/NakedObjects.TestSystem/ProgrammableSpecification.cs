// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Testing {
    public class ProgrammableSpecification : INakedObjectSpecification {
        private readonly IDictionary<Type, IFacet> facets = new Dictionary<Type, IFacet>();
        private readonly IDictionary<string, Association> properties = new Dictionary<string, Association>();
        private readonly Type type;
        private Persistable persistable = Persistable.USER_PERSISTABLE;

        public ProgrammableSpecification(Type type, ProgrammableTestSystem system) {
            this.type = type;

            foreach (PropertyInfo property in type.GetProperties()) {
                properties[property.Name] = new Association(property, system);
            }
            foreach (MethodInfo method in type.GetMethods()) {}
        }

        #region INakedObjectSpecification Members

        public INakedObjectAction[] GetRelatedServiceActions() {
            throw new NotImplementedException();
        }

        public INakedObjectAction[] GetObjectActions() {
            throw new NotImplementedException();
        }

        public INakedObjectAssociation[] Properties {
            get {
                var array = new Association[properties.Count];
                properties.Values.CopyTo(array, 0);
                return array;
            }
        }

        public INakedObjectAssociation GetProperty(string id) {
            return properties[id];
        }

        public INakedObjectValidation[] ValidateMethods() {
            throw new NotImplementedException();
        }

        public Type[] FacetTypes {
            get { throw new NotImplementedException(); }
        }

        public IIdentifier Identifier {
            get { throw new NotImplementedException(); }
        }

        public bool ContainsFacet(Type facetType) {
            return false;
        }

        public bool ContainsFacet<T>() where T : IFacet {
            return false;
        }

        public IFacet GetFacet(Type type) {
            throw new NotImplementedException();
        }

        public T GetFacet<T>() where T : IFacet {
            return (T) facets[typeof (T)];
        }

        public IFacet[] GetFacets(IFacetFilter filter) {
            throw new NotImplementedException();
        }

        public void AddFacet(IFacet facet) {
            Type facetType = facet.FacetType;
            facets[facetType] = facet;
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

        public bool HasSubclasses {
            get { throw new NotImplementedException(); }
        }

        public INakedObjectSpecification[] Interfaces {
            get { throw new NotImplementedException(); }
        }

        public INakedObjectSpecification[] Subclasses {
            get { throw new NotImplementedException(); }
        }

        public INakedObjectSpecification Superclass {
            get { throw new NotImplementedException(); }
        }

        public void AddSubclass(INakedObjectSpecification specification) {
            throw new NotImplementedException();
        }

        public bool IsOfType(INakedObjectSpecification specification) {
            return this == specification;
        }

        public string FullName {
            get { return type.FullName; }
        }

        public string PluralName {
            get { return string.Empty; }
        }

        public string ShortName {
            get { return string.Empty; }
        }

        public string Description {
            get { return string.Empty; }
        }

        public string Help {
            get { return string.Empty; }
        }

        public string SingularName {
            get { return string.Empty; }
        }

        public string UntitledName {
            get { return Resources.NakedObjects.Untitled + SingularName; }
        }

        public bool IsParseable {
            get { throw new NotImplementedException(); }
        }

        public bool IsEncodeable {
            get { return false; }
        }

        public bool IsAggregated {
            get { return false; }
        }

        public bool IsCollection {
            get { return type is IList; }
        }

        public bool IsObject {
            get { return true; }
        }

        public bool IsAbstract {
            get { throw new NotImplementedException(); }
        }

        public bool IsInterface {
            get { throw new NotImplementedException(); }
        }

        public bool IsService {
            get { throw new NotImplementedException(); }
        }

        public bool HasNoIdentity {
            get { return true; }
        }

        public bool IsQueryable {
            get { return false; }
        }

        public bool IsVoid {
            get { return false; }
        }

        public string GetIconName(INakedObject forObject) {
            throw new NotImplementedException();
        }

        public string GetTitle(INakedObject nakedObject) {
            throw new NotImplementedException();
        }

        public IConsent ValidToPersist(INakedObject transientObject) {
            throw new NotImplementedException();
        }

        public Persistable Persistable {
            get { return persistable; }
        }

        public bool IsASet {
            get { return false; }
        }

        public bool IsViewModel {
            get { return false; }
        }

        public object CreateObject() {
            throw new NotImplementedException();
        }

        public IEnumerable GetBoundedSet(INakedObjectPersistor persistor) {
            throw new NotImplementedException();
        }

        public void MarkAsService() {
            throw new NotImplementedException();
        }

        public string GetInvariantString(INakedObject nakedObject) {
            throw new NotImplementedException();
        }

        public object DefaultValue {
            get { throw new NotImplementedException(); }
        }

        #endregion

        public void SetUpPersistable(Persistable persistable) {
            this.persistable = persistable;
        }

        public object CreateInlineObject(object root) {
            throw new NotImplementedException();
        }
    }

    public class Association : INakedObjectAssociation {
        private readonly PropertyInfo property;
        private readonly INakedObjectSpecification specification;
        private readonly ProgrammableTestSystem system;

        public Association(PropertyInfo property, ProgrammableTestSystem system) {
            this.property = property;
            this.system = system;
            specification = system.SpecificationFor(property.PropertyType);
        }

        #region INakedObjectAssociation Members

        public Type[] FacetTypes {
            get { throw new NotImplementedException(); }
        }

        public IIdentifier Identifier {
            get { throw new NotImplementedException(); }
        }

        public bool ContainsFacet(Type facetType) {
            throw new NotImplementedException();
        }

        public bool ContainsFacet<T>() where T : IFacet {
            throw new NotImplementedException();
        }

        public IFacet GetFacet(Type type) {
            throw new NotImplementedException();
        }

        public T GetFacet<T>() where T : IFacet {
            throw new NotImplementedException();
        }

        public IFacet[] GetFacets(IFacetFilter filter) {
            throw new NotImplementedException();
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

        public string Name {
            get { throw new NotImplementedException(); }
        }

        public string Description {
            get { throw new NotImplementedException(); }
        }

        public string Id {
            get { return property.Name; }
        }

        public string Help {
            get { throw new NotImplementedException(); }
        }

        public string DebugData {
            get { throw new NotImplementedException(); }
        }

        public bool IsVisible(ISession session, INakedObject target, INakedObjectPersistor persistor) {
            throw new NotImplementedException();
        }

        public bool IsObject {
            get { return true; }
        }

        public bool IsCollection {
            get { return false; }
        }

        public bool IsASet {
            get { return false; }
        }

        public bool IsPersisted {
            get { return true; }
        }

        public bool IsMandatory {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly {
            get { throw new NotImplementedException(); }
        }

        public INakedObject GetNakedObject(INakedObject target, INakedObjectManager manager) {
            object value = property.GetValue(target.Object, null);
            if (value == null) {
                return null;
            }
            return system.AdapterFor(value);
        }

        public INakedObject GetDefault(INakedObject nakedObject) {
            throw new NotImplementedException();
        }

        public TypeOfDefaultValue GetDefaultType(INakedObject nakedObject) {
            throw new NotImplementedException();
        }

        public void ToDefault(INakedObject target) {
            throw new NotImplementedException();
        }

        public bool IsEmpty(INakedObject target, INakedObjectPersistor persistor) {
            throw new NotImplementedException();
        }

        public bool IsInline {
            get { return false; }
        }

        public INakedObjectSpecification Specification {
            get { return specification; }
        }

        public IConsent IsUsable(ISession session, INakedObject target, INakedObjectPersistor persistor) {
            throw new NotImplementedException();
        }

        public bool IsNullable {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}