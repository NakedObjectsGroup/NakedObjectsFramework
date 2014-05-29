// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

namespace NakedObjects.Reflector.I18n.Resourcebundle {
    public interface ITranslator {
        string TranslatedLanguage { get; }
        string Translate(string phrase);
    }
}