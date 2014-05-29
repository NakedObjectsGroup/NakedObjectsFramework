// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Reflector.I18n.Resourcebundle {
    public class NonTranslator : ITranslator {
        private readonly string translatedLanguage;

        public NonTranslator(string translatedLanguage) {
            this.translatedLanguage = translatedLanguage;
        }

        public string Translate(string phrase) {
            return phrase;
        }

        public string TranslatedLanguage {
            get { return translatedLanguage; }
        }
    }
}