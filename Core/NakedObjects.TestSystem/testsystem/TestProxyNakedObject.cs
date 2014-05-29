// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;

namespace NakedObjects.TestSystem {
    public class TestProxyNakedObject : INakedObject {
        private static int next;
        private readonly IDictionary<string, INakedObject> fieldContents = new Dictionary<string, INakedObject>();
        private readonly int id = next++;
        private readonly ResolveStateMachine resolveState;
        private object adaptedObject;
        private string iconName;
        private IOid oid;
        private INakedObjectSpecification specification;
        private string titleString = "default title string";
        private ITypeOfFacet typeOfFacet;
        private IVersion version;

        public TestProxyNakedObject() {
            resolveState = new ResolveStateMachine(this);
        }

        #region INakedObject Members

        public string TitleString() {
            return titleString;
        }

        public virtual ITypeOfFacet TypeOfFacet {
            get {
                if (typeOfFacet == null) {
                    return Specification.GetFacet<ITypeOfFacet>();
                }
                return typeOfFacet;
            }

            set { typeOfFacet = value; }
        }

        public void CheckLock(IVersion otherVersion) {
            if (version.IsDifferent(otherVersion)) {
                throw new ConcurrencyException("", Oid);
            }
        }

        public string IconName() {
            return iconName;
        }

        public object Object {
            get { return adaptedObject; }
        }

        public IOid Oid {
            get { return oid; }
        }

        public ResolveStateMachine ResolveState {
            get { return resolveState; }
        }

        public INakedObjectSpecification Specification {
            get { return specification; }
        }

        public IVersion Version {
            get { return version; }
        }

        public IVersion OptimisticLock {
            set { version = value; }
        }

        public void ReplacePoco(object pojo) {
            throw new NotImplementedException();
        }

        public void FireChangedEvent() {
            // do nothing
        }

        public string ValidToPersist() {
            throw new System.NotImplementedException();
        }

        public void SetATransientOid(IOid oid) {
            throw new NotImplementedException();
        }

        #endregion

        public INakedObject GetField(INakedObjectAssociation field) {
            return fieldContents[field.Id];
        }

        public void SetupFieldValue(string name, INakedObject field) {
            fieldContents[name] = field;
        }

        public void SetupIconName(string newIconName) {
            iconName = newIconName;
        }

        public void SetupObject(object obj) {
            if (obj is INakedObject) {
                throw new Exception("can't create a naked object for a naked object: " + obj);
            }
            adaptedObject = obj;
        }

        public void SetupOid(IOid newOid) {
            oid = newOid;
        }

     

        public void SetupSpecification(INakedObjectSpecification spec) {
            specification = spec;
        }

        public void SetupTitleString(string titleString) {
            this.titleString = titleString;
        }

        public void SetupVersion(IVersion newVersion) {
            version = newVersion;
        }

        public void SetValue(IOneToOneAssociation field, object obj) {}

        public override string ToString() {
            var str = new AsString(this, id);
            str.Append("title", titleString);
            str.Append("oid", oid);
            str.Append("pojo", adaptedObject);
            return str.ToString();
        }

      
   

       

       
    }
}