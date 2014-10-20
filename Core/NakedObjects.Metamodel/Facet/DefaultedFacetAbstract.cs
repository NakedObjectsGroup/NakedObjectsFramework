// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;

namespace NakedObjects.Metamodel.Facet {
    public abstract class DefaultedFacetAbstract<T> : FacetAbstract, IDefaultedFacet {
        // to delegate to
        private readonly DefaultedFacetUsingDefaultsProvider<T> defaultedFacetUsingDefaultsProvider;
        private readonly Type defaultsProviderClass;

        protected DefaultedFacetAbstract(string candidateProviderName,
                                         Type candidateProviderClass,
                                         ISpecification holder)
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