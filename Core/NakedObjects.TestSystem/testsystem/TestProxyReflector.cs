// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Reflector.Spec;

namespace NakedObjects.TestSystem {
    public class TestProxyReflector : INakedObjectReflector {
        // abstract


        private readonly IDictionary<string, INakedObjectSpecification> specs = new Dictionary<string, INakedObjectSpecification>();
        private ISpecificationCache cache = new SimpleSpecificationCache();
        private FacetDecoratorSet facetDecorator;

        private bool linked;


        public virtual FacetDecoratorSet FacetDecorator {
            set { facetDecorator = value; }
        }

        public virtual ISpecificationCache Cache {
            get { return cache; }
            set { cache = value; }
        }

        #region INakedObjectReflector Members

        public INakedObject[] NonSystemServices { get; set; }
        public IClassStrategy ClassStrategy { get; private set; }
        public IFacetFactorySet FacetFactorySet { get; private set; }

        public virtual void InstallServiceSpecifications(Type[] type) {
            type.ForEach(InstallServiceSpecification);
        }


        public virtual void PopulateContributedActions(INakedObject[] services) {
            try {
                if (!linked) {
                    AllSpecifications.OfType<IIntrospectableSpecification>().ForEach(s => s.PopulateAssociatedActions(services));
                }
            }
            finally {
                linked = true;
            }
        }


        public bool IgnoreCase { get; set; }

        // end abastract


        public INakedObjectSpecification[] AllSpecifications {
            get {
                var specsArray = new INakedObjectSpecification[specs.Count];
                int i = 0;

                foreach (INakedObjectSpecification spec in specs.Values) {
                    specsArray[i++] = spec;
                }
                return specsArray;
            }
        }


        public void Init() {}

        public INakedObjectSpecification LoadSpecification(Type type) {
            return LoadSpecification(type.FullName);
        }

        public INakedObjectSpecification LoadSpecification(string name) {
            if (specs.ContainsKey(name)) {
                return specs[name];
            }
            var specification = new TestProxySpecification(name);
            specs[specification.FullName] = specification;
            return specification;
        }

        public void Shutdown() {}
        public void LoadSpecificationForReturnTypes(IList<PropertyInfo> properties, Type classToIgnore) {
            throw new NotImplementedException();
        }

        #endregion

        public void InstallServiceSpecification(Type type) {}

        public INakedObject CreateCollectionAdapter(object collection, INakedObjectSpecification elementSpecification) {
            return null;
        }

        public void AddSpecification(INakedObjectSpecification specification) {
            specs[specification.FullName] = specification;
        }

        protected NakedObjectSpecificationAbstract Install(Type type) {
            throw new NotImplementedException();
        }

        protected INakedObjectSpecification CreateSpecification(Type type) {
            return LoadSpecification(type);
        }
    }
}