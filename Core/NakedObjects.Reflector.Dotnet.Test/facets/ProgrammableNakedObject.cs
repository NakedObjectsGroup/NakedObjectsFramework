// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.DotNet.Facets {
    internal class ProgrammableNakedObject : INakedObject {
        private readonly object poco;
        private readonly INakedObjectSpecification specification;

        public ProgrammableNakedObject(object poco, INakedObjectSpecification specification) {
            this.poco = poco;
            this.specification = specification;
        }

        #region INakedObject Members

        public object Object {
            get { return poco; }
        }

        public INakedObjectSpecification Specification {
            get { return specification; }
        }

        public IOid Oid {
            get { throw new NotImplementedException(); }
        }

        public ResolveStateMachine ResolveState {
            get { throw new NotImplementedException(); }
        }

        public IVersion Version {
            get { throw new NotImplementedException(); }
        }

        public IVersion OptimisticLock {
            set { throw new NotImplementedException(); }
        }

        public ITypeOfFacet TypeOfFacet {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string IconName() {
            throw new NotImplementedException();
        }

        public string TitleString() {
            throw new NotImplementedException();
        }

        public string InvariantString() {
            throw new NotImplementedException();
        }

        public void CheckLock(IVersion otherVersion) {
            throw new NotImplementedException();
        }

        public void ReplacePoco(object poco) {
            throw new NotImplementedException();
        }

        public string ValidToPersist() {
            throw new NotImplementedException();
        }

       

        public void SetATransientOid(IOid oid) {
            throw new NotImplementedException();
        }

        #endregion

        public void FireChangedEvent() {
            throw new NotImplementedException();
        }
    }
}