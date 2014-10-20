// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Metamodel.Facet {
    public abstract class FacetsFacetAbstract : FacetAbstract, IFacetsFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof (FacetsFacetAbstract));

        private readonly Type[] facetFactories;

        protected FacetsFacetAbstract(IEnumerable<string> names, IEnumerable<Type> types, ISpecification holder)
            : base(Type, holder) {
            var factories = new List<Type>();
            foreach (string name in names) {
                Type facetFactory = FacetFactoryOrNull(name);
                if (facetFactory != null) {
                    factories.Add(facetFactory);
                }
            }
            foreach (Type type in types) {
                Type facetFactory = FacetFactoryOrNull(type);
                if (facetFactory != null) {
                    factories.Add(facetFactory);
                }
            }
            facetFactories = factories.ToArray();
        }

        public static Type Type {
            get { return typeof (IFacetsFacet); }
        }

        #region IFacetsFacet Members

        public virtual Type[] FacetFactories {
            get { return facetFactories; }
        }

        #endregion

        private static Type FacetFactoryOrNull(string typeCandidateName) {
            if (typeCandidateName == null) {
                Log.Warn("typeCandidateName is null");
                return null;
            }
            try {
                Type classCandidate = TypeUtils.GetType(typeCandidateName);
                return FacetFactoryOrNull(classCandidate);
            }
            catch {
                Log.Warn("Exception getting type for typeCandidateName");
            }
            return null;
        }

        private static Type FacetFactoryOrNull(Type typeCandidate) {
            if (typeCandidate == null) {
                Log.Warn("typeCandidate is null");
                return null;
            }
            Type facetFactory = typeof (IFacetFactory).IsAssignableFrom(typeCandidate) ? typeCandidate : null;
            if (facetFactory == null) {
                Log.WarnFormat("FacetFactory type : {0} is not assignable from typeCandidate type : {1}", typeof (IFacetFactory), typeCandidate);
            }
            return facetFactory;
        }
    }
}