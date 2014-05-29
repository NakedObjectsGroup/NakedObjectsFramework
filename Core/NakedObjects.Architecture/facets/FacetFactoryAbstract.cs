// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets {
    public abstract class FacetFactoryAbstract : IFacetFactory, INakedObjectReflectorAware {
        private readonly NakedObjectFeatureType[] featureTypes;

        protected FacetFactoryAbstract(NakedObjectFeatureType[] featureTypes) {
            this.featureTypes = featureTypes;
        }

        #region IFacetFactory Members

        public virtual NakedObjectFeatureType[] FeatureTypes {
            get { return featureTypes; }
        }

        public virtual bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            return false;
        }

        public virtual bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return false;
        }

        public virtual bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return false;
        }

        public virtual bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            return false;
        }

        public virtual void FindCollectionProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {}

        public virtual void FindProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {}

        #endregion

        #region INakedObjectReflectorAware Members

        /// <summary>
        ///     Injected
        /// </summary>
        public virtual INakedObjectReflector Reflector { protected get; set; }

        #endregion
    }
}