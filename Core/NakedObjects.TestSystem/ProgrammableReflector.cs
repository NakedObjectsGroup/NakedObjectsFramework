// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Testing {
    public class ProgrammableReflector : INakedObjectReflector {
        private readonly IDictionary<Type, INakedObjectSpecification> specifications = new Dictionary<Type, INakedObjectSpecification>();
        private readonly ProgrammableTestSystem system;

        public ProgrammableReflector(ProgrammableTestSystem system) {
            this.system = system;
        }

        #region INakedObjectReflector Members

        public void Init() {}

        public INakedObjectSpecification[] AllSpecifications {
            get { return new INakedObjectSpecification[0]; }
        }

        public INakedObject[] NonSystemServices { get; set; }

        public INakedObjectSpecification LoadSpecification(Type type) {
            if (specifications.ContainsKey(type)) {
                return specifications[type];
            }
            else {
                INakedObjectSpecification specification = new ProgrammableSpecification(type, TestSystem);
                specifications[type] = specification;
                return specification;
            }
        }

        public INakedObjectSpecification LoadSpecification(string name) {
            throw new NotImplementedException();

            //TestProxySpecification specification = new TestProxySpecification(name);
            //return specification;
        }

        public void InstallServiceSpecifications(Type[] types) {
            throw new NotImplementedException();
        }

        public void PopulateContributedActions(INakedObject[] services) {
            throw new NotImplementedException();
        }

        public void Shutdown() {}

     

        public bool IgnoreCase {
            get { return false; }
        }

        public ProgrammableTestSystem TestSystem {
            get { return system; }
        }

        public void InstallServiceSpecification(Type type) {
            throw new NotImplementedException();
        }

        #endregion
    }
}