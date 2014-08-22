// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.TestSystem {
    public class TestProxySpecification : NakedObjectSpecificationAbstract {
        private readonly string name;
        private INakedObjectAction action;
        public INakedObjectAssociation[] assocs = new INakedObjectAssociation[0];
        private bool hasNoIdentity;
        private bool isEncodeable;
        private Persistable persistable;
        private INakedObjectSpecification[] subclasses = new INakedObjectSpecification[0];
        private string title;

        public TestProxySpecification(Type type) : this(type.FullName) {
            persistable = Persistable.USER_PERSISTABLE;
        }

        public TestProxySpecification(string name) {
            this.name = name;
            title = "";
        }

        public override bool IsAbstract {
            get { return false; }
        }

        public override bool IsService {
            get { return false; }
        }

        public override INakedObjectAssociation[] Properties {
            get { return assocs; }
        }

        public override string FullName {
            get { return name; }
        }

        public override void PopulateAssociatedActions(INakedObject[] services) {
            throw new NotImplementedException();
        }

        public override string PluralName {
            get { return null; }
        }

        public override string ShortName {
            get { return name.Substring(name.LastIndexOf('.') + 1); }
        }

        public override string SingularName {
            get { return name + " (singular)"; }
        }

        public override string UntitledName {
            get { return Resources.NakedObjects.Untitled + SingularName; }
        }

        public override string Description {
            get { return SingularName; }
        }      

        public override bool HasSubclasses {
            get { return false; }
        }

        public override INakedObjectSpecification[] Interfaces {
            get { return new INakedObjectSpecification[0]; }
        }

        public override bool IsEncodeable {
            get { return isEncodeable; }
        }

        public override bool IsParseable {
            get { return false; }
        }

        public override INakedObjectSpecification[] Subclasses {
            get { return subclasses; }
        }

        public override INakedObjectSpecification Superclass {
            get { return null; }
        }

        public override object DefaultValue {
            get { return null; }
        }

        public override IIdentifier Identifier {
            get { return new TestProxyIdentifier(name); }
        }

        public string DebugTitle {
            get { return ""; }
        }

        public override bool HasNoIdentity {
            get { return hasNoIdentity; }
        }

        public override bool IsQueryable {
            get { return false; }
        }

        public override bool IsVoid {
            get { return false; }
        }

        public override void AddSubclass(INakedObjectSpecification specification) {}


        public override INakedObjectAction[] GetRelatedServiceActions() {
            return null;
        }

        public int GetFeatures() {
            return 0;
        }

        public override INakedObjectAssociation GetProperty(string name) {
            for (int i = 0; i < assocs.Length; i++) {
                if (assocs[i].Id.Equals(name)) {
                    return assocs[i];
                }
            }
            throw new Exception("Field not found: " + name);
        }

        public override string GetInvariantString(INakedObject nakedObject) {
            throw new NotImplementedException();
        }

        public object GetFieldExtension(string name, Type type) {
            return null;
        }

        public Type[] GetFieldExtensions(string name) {
            return new Type[0];
        }

        public override string GetIconName(INakedObject forObject) {
            return ShortName;
        }

        public override INakedObjectAction[] GetObjectActions() {
            return null;
        }

        public override string GetTitle(INakedObject nakedObject, INakedObjectManager manager) {
            return title;
        }


        public void Introspect() {}


        public override bool IsOfType(INakedObjectSpecification specification) {
            return specification == this;
        }


        public Persistable IsPersistable() {
            return persistable;
        }

        public void SetupAction(INakedObjectAction action) {
            this.action = action;
        }

        public void SetupFields(INakedObjectAssociation[] fields) {
            assocs = fields;
        }

        public void SetupIsCollection() {}

        public void SetupIsObject() {}

        public void SetupIsEncodeable() {
            isEncodeable = true;
        }

        public void SetupSubclasses(INakedObjectSpecification[] subclasses) {
            this.subclasses = subclasses;
        }

        public void SetupHasNoIdentity(bool hasNoIdentity) {
            this.hasNoIdentity = hasNoIdentity;
        }

        public void SetupTitle(string title) {
            this.title = title;
        }

        public override string ToString() {
            return FullName;
        }

        public override IConsent ValidToPersist(INakedObject transientObject, ISession session) {
            return null;
        }

        public override object CreateObject(INakedObjectPersistor persistor) {
            Type type = Type.GetType(name);
            return type.GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        public override IEnumerable GetBoundedSet(INakedObjectPersistor persistor) {
            throw new NotImplementedException();
        }

        public void SetupPersistable(Persistable persistable) {
            this.persistable = persistable;
        }

        public override void Introspect(FacetDecoratorSet decorator) {
            // do nothing
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}