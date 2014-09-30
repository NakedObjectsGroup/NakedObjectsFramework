// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.Named;
using NakedObjects.Util;

namespace NakedObjects.Reflector.I18n {
    public class NamedFacetDynamicWrapI18n : FacetAbstract, INamedFacet {
        private readonly IIdentifier identifier;
        private readonly int index;
        private readonly II18nManager manager;
        private readonly INamedFacet namedFacet;


        public NamedFacetDynamicWrapI18n(II18nManager manager, IFacetHolder holder, IIdentifier identifier, INamedFacet namedFacet, int index = -1)
            : base(Type, holder) {
            this.manager = manager;
            this.identifier = identifier;
            this.namedFacet = namedFacet;

            this.index = index;
        }

        public static Type Type {
            get { return typeof (INamedFacet); }
        }

        #region INamedFacet Members

        public string Value {
            get {
                if (index >= 0) {
                    return manager.GetParameterName(identifier, index, null) ?? namedFacet.Value ?? NameUtils.NaturalName(identifier.MemberName);
                }
                return manager.GetName(identifier, null) ?? namedFacet.Value ?? NameUtils.NaturalName(identifier.MemberName);
            }
        }

        #endregion
    }


    // Copyright (c) Naked Objects Group Ltd.
}