// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using Common.Logging;
using NakedObjects.Util;

namespace NakedObjects.Architecture.Facets.Objects.Facets {
    public abstract class FacetsFacetAbstract : FacetAbstract, IFacetsFacet {
        private static readonly ILog Log = LogManager.GetLogger(typeof (FacetsFacetAbstract));

        private readonly Type[] facetFactories;

        protected FacetsFacetAbstract(IEnumerable<string> names, IEnumerable<Type> types, IFacetHolder holder)
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