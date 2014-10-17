// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Reflector.Spec {
    public class SimpleSpecificationCache : ISpecificationCache {
        private readonly Dictionary<string, IObjectSpecImmutable> specs = new Dictionary<string, IObjectSpecImmutable>();

        #region ISpecificationCache Members

        public virtual IObjectSpecImmutable GetSpecification(string className) {
            lock (specs) {
                if (specs.ContainsKey(className)) {
                    return specs[className];
                }
                return null;
            }
        }

        public virtual void Cache(string className, IObjectSpecImmutable spec) {
            lock (specs) {
                specs[className] = spec;
            }
        }

        public virtual void Clear() {
            lock (specs) {
                specs.Clear();
            }
        }

        public virtual IObjectSpecImmutable[] AllSpecifications() {
            var returnSpecs = new List<IObjectSpecImmutable>();

            lock (specs) {
                foreach (IObjectSpecImmutable spec in specs.Values) {
                    returnSpecs.Add(spec);
                }
            }

            return returnSpecs.ToArray();
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}