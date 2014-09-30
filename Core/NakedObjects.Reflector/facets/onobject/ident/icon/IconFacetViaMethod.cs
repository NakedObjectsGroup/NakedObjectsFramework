// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Icon;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Icon {
    public class IconFacetViaMethod : IconFacetAbstract {
        private readonly string iconName; // iconName from attribute
        private readonly MethodInfo method;

        public IconFacetViaMethod(MethodInfo method, IFacetHolder holder, string iconName)
            : base(holder) {
            this.method = method;
            this.iconName = iconName;
        }

        public override string GetIconName(INakedObject nakedObject) {
            return (string) InvokeUtils.Invoke(method, nakedObject);
        }

        public override string GetIconName() {
            return iconName;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}