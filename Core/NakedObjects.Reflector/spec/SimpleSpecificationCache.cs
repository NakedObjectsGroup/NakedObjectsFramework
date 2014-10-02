// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;

namespace NakedObjects.Reflector.Spec {
    public class SimpleSpecificationCache : ISpecificationCache {
        private readonly Dictionary<string, IIntrospectableSpecification> specs = new Dictionary<string, IIntrospectableSpecification>();

        #region ISpecificationCache Members

        public virtual IIntrospectableSpecification GetSpecification(string className) {
            lock (specs) {
                if (specs.ContainsKey(className)) {
                    return specs[className];
                }
                return null;
            }
        }

        public virtual void Cache(string className, IIntrospectableSpecification spec) {
            lock (specs) {
                specs[className] = spec;
            }
        }

        public virtual void Clear() {
            lock (specs) {
                specs.Clear();
            }
        }

        public virtual IIntrospectableSpecification[] AllSpecifications() {
            var returnSpecs = new List<IIntrospectableSpecification>();

            lock (specs) {
                foreach (IIntrospectableSpecification spec in specs.Values) {
                    returnSpecs.Add(spec);
                }
            }

            return returnSpecs.ToArray();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}