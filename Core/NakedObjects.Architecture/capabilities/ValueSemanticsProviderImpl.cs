// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

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