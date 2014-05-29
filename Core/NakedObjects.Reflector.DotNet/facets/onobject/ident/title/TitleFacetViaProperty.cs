// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Core.Persist;
using NakedObjects.Reflector.DotNet.Reflect.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title {
    public class TitleFacetViaProperty : TitleFacetAbstract, IImperativeFacet {
        private readonly MethodInfo method;

        public TitleFacetViaProperty(MethodInfo method, IFacetHolder holder)
            : base(holder) {
            this.method = method;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override string GetTitle(INakedObject nakedObject) {
            object obj = InvokeUtils.Invoke(method, nakedObject);
            return obj == null ? null : PersistorUtils.CreateAdapter(obj).TitleString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}