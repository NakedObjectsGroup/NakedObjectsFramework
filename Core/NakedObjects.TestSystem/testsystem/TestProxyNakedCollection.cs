// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Facets.Collections.Modify;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NUnit.Framework;

namespace NakedObjects.TestSystem {
    public class TestProxyNakedCollection : INakedObject {
        private readonly IList collection = new ArrayList();
        private readonly ResolveStateMachine resolveState;
        private string addValidMessgae;
        private IOid oid;
        private string removeValidMessage;
        private INakedObjectSpecification specification;
        private ITypeOfFacet typeOfFacet;
        private IVersion version;

        public TestProxyNakedCollection() {
            resolveState = new ResolveStateMachine(this);
        }

        public TestProxyNakedCollection(object wrappedCollection) {
            if (wrappedCollection is ICollection) {
                collection = new ArrayList((ICollection) wrappedCollection);
            }
        }

        #region INakedObject Members

        public virtual ITypeOfFacet TypeOfFacet {
            get {
                if (typeOfFacet == null) {
                    return Specification.GetFacet<ITypeOfFacet>();
                }
                return typeOfFacet;
            }

            set { typeOfFacet = value; }
        }

        public void CheckLock(IVersion otherVersion) {}

        public string IconName() {
            return Specification.GetIconName(null);
        }

        public object Object {
            get { return collection; }
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

        public void ReplacePoco(object pojo) {
            throw new NotImplementedException();
        }

        public IVersion OptimisticLock {
            set { }
        }

        public string TitleString() {
            return "title";
        }

        public void FireChangedEvent() {}
        public string ValidToPersist() {
            throw new System.NotImplementedException();
        }

        public void SetATransientOid(IOid oid) {
            throw new NotImplementedException();
        }

        #endregion

        public INakedObject FirstElement() {
            if (collection.Count == 0) {
                return null;
            }
            return (INakedObject) collection[0];
        }

        public INakedObjectSpecification ElementSpecification() {
            return null;
        }

        public void Init(INakedObject[] initElements) {
            Assert.AreEqual(0, collection.Count, "Collection not empty");
            foreach (INakedObject nakedObject in initElements) {
                collection.Add(nakedObject);
            }
        }

        public void SetupSpecification(INakedObjectSpecification spec) {
            specification = spec;
        }

        public void SetupOid(IOid newOid) {
            oid = newOid;
        }

        public void SetupElement(INakedObject element) {
            collection.Add(element);
        }

        public int Size() {
            return collection.Count;
        }

        public void SetupVersion(IVersion newVersion) {
            version = newVersion;
        }

        public void Add(INakedObject element) {
            collection.Add(element);
        }

        public void Clear() {
            collection.Clear();
        }

        public string IsAddValid(INakedObject element) {
            return addValidMessgae;
        }

        public string IsRemoveValid(INakedObject element) {
            return removeValidMessage;
        }

        public void Remove(INakedObject element) {
            collection.Remove(element);
        }

        public void SetupAddValidMessage(string addValidMessage) {
            addValidMessgae = addValidMessage;
        }

        public void SetupRemoveValidMessage(string message) {
            removeValidMessage = message;
        }     

       

      
     
      

      

    }

    internal class TestProxyCollectionFacet : CollectionFacetAbstract {
        public TestProxyCollectionFacet(IFacetHolder holder)
            : base(holder) {}

        public override IFacetHolder FacetHolder {
            set { }
            get { return null; }
        }

        public override bool CanAlwaysReplace {
            get { return false; }
        }

        public override bool IsNoOp {
            get { return false; }
        }

        private static TestProxyNakedCollection AsCollection(INakedObject collection) {
            var coll = (TestProxyNakedCollection) collection;
            return coll;
        }

        public override bool Contains(INakedObject collection, INakedObject element) {
            //  return AsCollection(collection).Contains(element);
            return false;
        }

       

        public override void Init(INakedObject collection, INakedObject[] initData) {}
        public override INakedObject Page(int page, int size, INakedObject collection, bool forceEnumerable) {
            throw new NotImplementedException();
        }

        public override IEnumerable<INakedObject> AsEnumerable(INakedObject collection) {
            throw new NotImplementedException();
        }

        public override IQueryable AsQueryable(INakedObject collection) {
            throw new NotImplementedException();
        }
    }
}