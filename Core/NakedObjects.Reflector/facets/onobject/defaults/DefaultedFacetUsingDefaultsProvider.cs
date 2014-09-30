// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Defaults;
using NakedObjects.Capabilities;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public class DefaultedFacetUsingDefaultsProvider<T> : FacetAbstract, IDefaultedFacet {
        private readonly IDefaultsProvider<T> defaultsProvider;

        public DefaultedFacetUsingDefaultsProvider(IDefaultsProvider<T> parser, IFacetHolder holder)
            : base(typeof (IDefaultedFacet), holder) {
            defaultsProvider = parser;
        }

        #region IDefaultedFacet Members

        public object Default {
            get { return defaultsProvider.DefaultValue; }
        }

        public bool IsValid {
            get { return defaultsProvider != null; }
        }

        #endregion

        protected override string ToStringValues() {
            return defaultsProvider.ToString();
        }
    }
}