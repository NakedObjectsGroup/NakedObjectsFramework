// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets.Actcoll.Typeof {
    public abstract class TypeOfFacetAbstract : SingleClassValueFacetAbstract, ITypeOfFacet {
        private readonly bool inferred;

        protected TypeOfFacetAbstract(Type valueType, bool inferred, IFacetHolder holder, INakedObjectReflector reflector)
            : base(Type, holder, valueType, reflector) {
            this.inferred = inferred;
        }

        public static Type Type {
            get { return typeof (ITypeOfFacet); }
        }

        #region ITypeOfFacet Members

        /// <summary>
        ///     Does <b>not</b> correspond to a member in the <see cref="TypeOfAttribute" />
        ///     annotation (or equiv), but indicates that the information provided
        ///     has been inferred rather than explicitly specified.
        /// </summary>
        public virtual bool IsInferred {
            get { return inferred; }
        }

        #endregion

        public override string ToString() {
            return "TypeOf [value=" + Value + ",inferred=" + inferred + "]";
        }
    }
}