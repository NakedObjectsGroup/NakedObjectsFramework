// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Reflection;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Ident.Title;
using NakedObjects.Architecture.Persist;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Ident.Title {
    public class TitleFacetViaToStringMethod : TitleFacetAbstract, IImperativeFacet {
        private readonly MethodInfo maskMethod;
        private readonly MethodInfo method;

        public TitleFacetViaToStringMethod(MethodInfo method, IFacetHolder holder)
            : this(method, null, holder) {}

        public TitleFacetViaToStringMethod(MethodInfo method, MethodInfo maskMethod, IFacetHolder holder)
            : base(holder) {
            this.method = method;
            this.maskMethod = maskMethod;
        }

        #region IImperativeFacet Members

        public MethodInfo GetMethod() {
            return method;
        }

        #endregion

        public override string GetTitle(INakedObject nakedObject, INakedObjectManager manager) {
            return nakedObject.Object.ToString();
        }

        public override string GetTitleWithMask(string mask, INakedObject nakedObject, INakedObjectManager manager) {
            if (maskMethod == null) {
                return GetTitle(nakedObject, manager);
            }
            return (string) maskMethod.Invoke(nakedObject.Object, new[] {mask});
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}