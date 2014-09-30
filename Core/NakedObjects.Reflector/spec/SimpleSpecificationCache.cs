// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflector.Spec {
    public class SimpleSpecificationCache : ISpecificationCache {
        private readonly Dictionary<string, INakedObjectSpecification> specs = new Dictionary<string, INakedObjectSpecification>();

        #region ISpecificationCache Members

        public virtual INakedObjectSpecification GetSpecification(string className) {
            lock (specs) {
                if (specs.ContainsKey(className)) {
                    return specs[className];
                }
                return null;
            }
        }

        public virtual void Cache(string className, INakedObjectSpecification spec) {
            lock (specs) {
                specs[className] = spec;
            }
        }

        public virtual void Clear() {
            lock (specs) {
                specs.Clear();
            }
        }

        public virtual INakedObjectSpecification[] AllSpecifications() {
            var returnSpecs = new List<INakedObjectSpecification>();

            lock (specs) {
                foreach (INakedObjectSpecification spec in specs.Values) {
                    returnSpecs.Add(spec);
                }
            }

            return returnSpecs.ToArray();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}