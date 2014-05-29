// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.TestSystem {
    public class TestProxyReflector : NakedObjectReflectorAbstract {
        private readonly IDictionary<string, INakedObjectSpecification> specs = new Dictionary<string, INakedObjectSpecification>();


        public override INakedObjectSpecification[] AllSpecifications {
            get {
                var specsArray = new INakedObjectSpecification[specs.Count];
                int i = 0;

                foreach (INakedObjectSpecification spec in specs.Values) {
                    specsArray[i++] = spec;
                }
                return specsArray;
            }
        }


        public override void Init() {}

        public override void InstallServiceSpecification(Type type) {}

        public override INakedObjectSpecification LoadSpecification(Type type) {
            return LoadSpecification(type.FullName);
        }

        public new INakedObjectSpecification LoadSpecification(string name) {
            if (specs.ContainsKey(name)) {
                return specs[name];
            }
            var specification = new TestProxySpecification(name);
            specs[specification.FullName] = specification;
            return specification;
        }

        public override void Shutdown() {}

        public INakedObject CreateCollectionAdapter(object collection, INakedObjectSpecification elementSpecification) {
            return null;
        }

        public override IContainerInjector CreateContainerInjector(object[] container) {
            return new TestProxyContainerInjector();
        }

        public void AddSpecification(INakedObjectSpecification specification) {
            specs[specification.FullName] = specification;
        }

        protected override NakedObjectSpecificationAbstract Install(Type type) {
            throw new NotImplementedException();
        }

        protected override INakedObjectSpecification CreateSpecification(Type type) {
            return LoadSpecification(type);
        }
    }
}