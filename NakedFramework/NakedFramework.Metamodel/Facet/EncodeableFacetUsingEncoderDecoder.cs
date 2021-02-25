// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedFramework.Architecture.Adapter;
using NakedFramework.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta.SemanticsProvider;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public sealed class EncodeableFacetUsingEncoderDecoder<T> : FacetAbstract, IEncodeableFacet {
        private readonly IValueSemanticsProvider<T> encoderDecoder;

        public EncodeableFacetUsingEncoderDecoder(IValueSemanticsProvider<T> encoderDecoder, ISpecification holder)
            : base(typeof(IEncodeableFacet), holder) =>
            this.encoderDecoder = encoderDecoder;

        public static string EncodedNull => "NULL";

        #region IEncodeableFacet Members

        public INakedObjectAdapter FromEncodedString(string encodedData, INakedObjectManager manager) => EncodedNull.Equals(encodedData) ? null : manager.CreateAdapter(encoderDecoder.FromEncodedString(encodedData), null, null);

        public string ToEncodedString(INakedObjectAdapter nakedObjectAdapter) => nakedObjectAdapter == null ? EncodedNull : encoderDecoder.ToEncodedString(nakedObjectAdapter.GetDomainObject<T>());

        #endregion

        protected override string ToStringValues() => encoderDecoder.ToString();
    }
}