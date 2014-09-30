// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.Defaults;
using NakedObjects.Capabilities;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Defaults {
    public abstract class DefaultedFacetAbstract<T> : FacetAbstract, IDefaultedFacet {
        // to delegate to
        private readonly DefaultedFacetUsingDefaultsProvider<T> defaultedFacetUsingDefaultsProvider;
        private readonly Type defaultsProviderClass;

        protected DefaultedFacetAbstract(string candidateProviderName,
                                         Type candidateProviderClass,
                                         IFacetHolder holder)
            : base(typeof (IDefaultedFacet), holder) {
            defaultsProviderClass = DefaultsProviderUtils.DefaultsProviderOrNull<T>(candidateProviderClass, candidateProviderName);
            defaultedFacetUsingDefaultsProvider = IsValid ? new DefaultedFacetUsingDefaultsProvider<T>((IDefaultsProvider<T>) TypeUtils.NewInstance(defaultsProviderClass), holder) : null;
        }

        #region IDefaultedFacet Members

        public object Default {
            get { return defaultedFacetUsingDefaultsProvider.Default; }
        }

        /// <summary>
        ///     Discover whether either of the candidate defaults provider name or class is valid.
        /// </summary>
        public bool IsValid {
            get { return defaultsProviderClass != null; }
        }

        #endregion

        /// <summary>
        ///     Guaranteed to implement the <see cref="IEncoderDecoder{T}" /> class, thanks to
        ///     generics in the applib.
        /// </summary>
        public Type GetDefaultsProviderClass() {
            return defaultsProviderClass;
        }

        protected override string ToStringValues() {
            return defaultsProviderClass.FullName;
        }
    }
}