// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

namespace NakedObjects.Capabilities {
    public abstract class ValueSemanticsProviderImpl<T> : IValueSemanticsProvider<T> {
        // Defaults IsImmutable to true  and IsEqualByContent to true also.
        protected ValueSemanticsProviderImpl()
            : this(true, true) {}

        protected ValueSemanticsProviderImpl(bool immutable, bool equalByContent) {
            IsImmutable = immutable;
            IsEqualByContent = equalByContent;
        }

        #region IValueSemanticsProvider<T> Members

        public IEncoderDecoder<T> EncoderDecoder {
            get { return this as IEncoderDecoder<T>; }
        }

        public IParser<T> Parser {
            get { return this as IParser<T>; }
        }

        public IDefaultsProvider<T> DefaultsProvider {
            get { return this as IDefaultsProvider<T>; }
        }

        public IFromStream FromStream {
            get { return null; }
        }

        // Defaults to true if no-arg constructor is used.
        public bool IsEqualByContent { get; private set; }

        //Defaults to true if no-arg constructor is used.
        public bool IsImmutable { get; private set; }

        #endregion
    }
}