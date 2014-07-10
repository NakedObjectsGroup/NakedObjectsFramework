// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.Actcoll.Typeof;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.DotNet.Facets {
    class ProgrammableNakedObject : INakedObject {
        private readonly object poco;
        private readonly INakedObjectSpecification specification;

        public ProgrammableNakedObject(object  poco, INakedObjectSpecification specification) {
            this.poco = poco;
            this.specification = specification;
        }

        public object Object {
            get { return poco; }
        }

        public INakedObjectSpecification Specification {
            get { return specification; }
        }

        public IOid Oid {
            get { throw new System.NotImplementedException(); }
        }

        public ResolveStateMachine ResolveState {
            get { throw new System.NotImplementedException(); }
        }

        public IVersion Version {
            get { throw new System.NotImplementedException(); }
        }

        public IVersion OptimisticLock {
            set { throw new System.NotImplementedException(); }
        }

        public ITypeOfFacet TypeOfFacet {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public string IconName() {
            throw new System.NotImplementedException();
        }

        public string TitleString() {
            throw new System.NotImplementedException();
        }

        public string InvariantString() {
            throw new NotImplementedException();
        }

        public void CheckLock(IVersion otherVersion) {
            throw new System.NotImplementedException();
        }

        public void ReplacePoco(object poco) {
            throw new System.NotImplementedException();
        }

        public void FireChangedEvent() {
            throw new System.NotImplementedException();
        }

        public string ValidToPersist() {
            throw new System.NotImplementedException();
        }

        public void SetATransientOid(IOid oid) {
            throw new NotImplementedException();
        }
    }
}
