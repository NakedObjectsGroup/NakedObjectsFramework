// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Icon {
    public class IconFacetAnnotation : IconFacetAbstract {
        private readonly string iconName;

        public IconFacetAnnotation(string iconName, IFacetHolder holder)
            : base(holder) {
            this.iconName = iconName;
        }

        public override string GetIconName() {
            return iconName;
        }

        public override string GetIconName(INakedObject nakedObject) {
            return iconName;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}