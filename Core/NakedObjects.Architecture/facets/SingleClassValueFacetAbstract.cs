// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Architecture.Facets {
    public abstract class SingleClassValueFacetAbstract : FacetAbstract, ISingleClassValueFacet {
        private readonly INakedObjectReflector reflector;
        private readonly Type valueType;

        protected SingleClassValueFacetAbstract(Type facetType, IFacetHolder holder, Type valueType, INakedObjectReflector reflector)
            : base(facetType, holder) {
            this.valueType = valueType;
            this.reflector = reflector;
        }

        private INakedObjectReflector Reflector {
            get { return reflector; }
        }

        #region ISingleClassValueFacet Members

        public virtual Type Value {
            get { return valueType; }
        }

        /// <summary>
        ///     The <see cref="INakedObjectSpecification" /> of the <see cref="Value" />
        /// </summary>
        public virtual INakedObjectSpecification ValueSpec {
            get { return Value != null ? Reflector.LoadSpecification(Value) : null; }
        }

        #endregion
    }
}