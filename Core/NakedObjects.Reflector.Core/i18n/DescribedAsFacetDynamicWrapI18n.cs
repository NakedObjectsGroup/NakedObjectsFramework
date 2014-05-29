// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Naming.DescribedAs;

namespace NakedObjects.Reflector.I18n {
    public class DescribedAsFacetDynamicWrapI18n : FacetAbstract, IDescribedAsFacet {
        private readonly IDescribedAsFacet describedAsFacet;
        private readonly IIdentifier identifier;
        private readonly int index;
        private readonly II18nManager manager;


        public DescribedAsFacetDynamicWrapI18n(II18nManager manager, IFacetHolder holder, IIdentifier identifier, IDescribedAsFacet describedAsFacet, int index = -1)
            : base(Type, holder) {
            this.manager = manager;
            this.identifier = identifier;
            this.describedAsFacet = describedAsFacet;
            this.index = index;
        }

        public static Type Type {
            get { return typeof (IDescribedAsFacet); }
        }

        #region IDescribedAsFacet Members

        public string Value {
            get {
                if (index >= 0) {
                    return manager.GetParameterDescription(identifier, index, null) ?? describedAsFacet.Value;
                }
                return manager.GetDescription(identifier, null) ?? describedAsFacet.Value;
            }
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}